using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Helpers
{
    public static class FastDiff<T>
    {
        private static readonly Action<Utf8JsonWriter, T, T>[] _propertyComparers;
        private static readonly Action<Utf8JsonWriter, T, T>[] _complexComparers;

        private static readonly JsonWriterOptions _jsonOptions = new JsonWriterOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            Indented = false
        };

        static FastDiff()
        {
            var simpleComparers = new List<Action<Utf8JsonWriter, T, T>>();
            var complexComparers = new List<Action<Utf8JsonWriter, T, T>>();
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                if (!prop.CanRead) continue;

                if (prop.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                try
                {
                    if (IsComplexType(prop.PropertyType))
                    {
                        complexComparers.Add(BuildComplexTypeDelegate(prop));
                    }
                    else
                    {
                        simpleComparers.Add(BuildSimpleTypeDelegate(prop));
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            _propertyComparers = simpleComparers.ToArray();
            _complexComparers = complexComparers.ToArray();
        }

        public static string GetDiffJson(T newObj, T oldObj)
        {
            var bufferWriter = new ArrayBufferWriter<byte>();

            using (var writer = new Utf8JsonWriter(bufferWriter, _jsonOptions))
            {
                writer.WriteStartObject();

                foreach (var compare in _propertyComparers)
                {
                    compare(writer, oldObj, newObj);
                }

                foreach (var compare in _complexComparers)
                {
                    compare(writer, oldObj, newObj);
                }

                writer.WriteEndObject();
            }

            return Encoding.UTF8.GetString(bufferWriter.WrittenSpan);
        }

        private static bool IsDictionary(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            return underlyingType.GetInterfaces().Any(i => i.IsGenericType &&
                   i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private static bool IsComplexType(Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;

            // 1. دیکشنری‌ها
            if (IsDictionary(underlyingType))
            {
                return true;
            }

            // 2. کالکشن‌های لیست‌مانند
            if (underlyingType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(underlyingType))
            {
                return true;
            }

            // 3. کلاس‌های Nested
            return !underlyingType.IsPrimitive
                && underlyingType != typeof(string)
                && underlyingType != typeof(DateTime)
                && underlyingType != typeof(decimal)
                && underlyingType.IsClass;
        }

        // --- هندلینگ انواع پیچیده و کالکشن‌ها در Runtime ---
        private static Action<Utf8JsonWriter, T, T> BuildComplexTypeDelegate(PropertyInfo property)
        {
            var propertyType = property.PropertyType;
            var isDictionary = IsDictionary(propertyType);
            var isCollection = typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType) && propertyType != typeof(string);

            var getterT = typeof(Func<,>).MakeGenericType(typeof(T), propertyType);
            var getter = (Delegate)property.GetMethod.CreateDelegate(getterT);

            // 🌟 اگر دیکشنری بود: از مقایسه JSON کامل استفاده کن (برای پایداری)
            if (isDictionary)
            {
                return (writer, oldObj, newObj) =>
                {
                    var oldVal = getter.DynamicInvoke(oldObj);
                    var newVal = getter.DynamicInvoke(newObj);

                    if (oldVal == null && newVal == null) return;

                    // مقایسه از طریق JSON
                    string oldJson = (oldVal != null) ? JsonSerializer.Serialize(oldVal) : "null";
                    string newJson = (newVal != null) ? JsonSerializer.Serialize(newVal) : "null";

                    if (oldJson != newJson)
                    {
                        writer.WritePropertyName(property.Name);
                        writer.WriteStartObject();
                        writer.WritePropertyName("OldValue");
                        writer.WriteRawValue(oldJson);
                        writer.WritePropertyName("NewValue");
                        writer.WriteRawValue(newJson);
                        writer.WriteEndObject();
                    }
                };
            }

            // 🌟 اگر کالکشن لیست‌مانند بود (Deep Diff Item-by-Item)
            if (isCollection)
            {
                var itemType = propertyType.IsArray ? propertyType.GetElementType() : propertyType.GetGenericArguments().FirstOrDefault();

                if (itemType == null)
                    return (w, o, n) => { };

                var itemDifferType = typeof(FastDiff<>).MakeGenericType(itemType);
                var getItemDiffMethod = itemDifferType.GetMethod(nameof(GetDiffJson), BindingFlags.Public | BindingFlags.Static);

                if (getItemDiffMethod == null) return (w, o, n) => { };

                return (writer, oldObj, newObj) =>
                {
                    var oldCollection = getter.DynamicInvoke(oldObj) as System.Collections.IEnumerable;
                    var newCollection = getter.DynamicInvoke(newObj) as System.Collections.IEnumerable;

                    if (oldCollection == null && newCollection == null) return;

                    var oldList = oldCollection?.Cast<object>().ToList() ?? new List<object>();
                    var newList = newCollection?.Cast<object>().ToList() ?? new List<object>();

                    var maxCount = Math.Max(oldList.Count, newList.Count);
                    bool hasDiff = false;

                    var bufferWriter = new ArrayBufferWriter<byte>();
                    using (var tempWriter = new Utf8JsonWriter(bufferWriter, _jsonOptions))
                    {
                        tempWriter.WriteStartObject();

                        for (int i = 0; i < maxCount; i++)
                        {
                            var oldItem = i < oldList.Count ? oldList[i] : null;
                            var newItem = i < newList.Count ? newList[i] : null;

                            if (oldItem == null && newItem == null) continue;

                            // هندلینگ آیتم‌های حذف/اضافه شده
                            if (oldItem == null || newItem == null)
                            {
                                hasDiff = true;
                                tempWriter.WritePropertyName($"[{i}]");
                                tempWriter.WriteStartObject();
                                tempWriter.WritePropertyName("Operation");
                                tempWriter.WriteStringValue(oldItem == null ? "Added" : "Removed");
                                tempWriter.WritePropertyName(oldItem == null ? "NewValue" : "OldValue");
                                tempWriter.WriteRawValue(JsonSerializer.Serialize(oldItem ?? newItem));
                                tempWriter.WriteEndObject();
                                continue;
                            }

                            // مقایسه عمیق آیتم‌های موجود
                            var itemDiffJson = (string)getItemDiffMethod.Invoke(null, new[] { oldItem, newItem });

                            if (!string.IsNullOrWhiteSpace(itemDiffJson) && itemDiffJson != "{}")
                            {
                                hasDiff = true;
                                tempWriter.WritePropertyName($"[{i}]");
                                tempWriter.WriteRawValue(itemDiffJson);
                            }
                        }

                        tempWriter.WriteEndObject();
                    }

                    if (hasDiff)
                    {
                        writer.WritePropertyName(property.Name);
                        writer.WriteRawValue(Encoding.UTF8.GetString(bufferWriter.WrittenSpan));
                    }
                };
            }

            // 🌟 اگر کلاس Nested بود
            var nestedDifferType = typeof(FastDiff<>).MakeGenericType(propertyType);
            var getDiffJsonMethod = nestedDifferType.GetMethod(nameof(GetDiffJson), BindingFlags.Public | BindingFlags.Static);

            if (getDiffJsonMethod == null) return (w, o, n) => { };

            return (writer, oldObj, newObj) =>
            {
                var oldVal = getter.DynamicInvoke(oldObj);
                var newVal = getter.DynamicInvoke(newObj);

                if (oldVal == null && newVal == null) return;

                if (oldVal != newVal || (oldVal != null && newVal != null))
                {
                    var jsonDiff = (string)getDiffJsonMethod.Invoke(null, new[] { oldVal, newVal });

                    if (!string.IsNullOrWhiteSpace(jsonDiff) && jsonDiff != "{}")
                    {
                        writer.WritePropertyName(property.Name);
                        writer.WriteRawValue(jsonDiff);
                    }
                }
            };
        }

        // --- هندلینگ انواع ساده با Expression Tree ---
        private static Action<Utf8JsonWriter, T, T> BuildSimpleTypeDelegate(PropertyInfo property)
        {
            var writerParam = Expression.Parameter(typeof(Utf8JsonWriter), "writer");
            var oldObjParam = Expression.Parameter(typeof(T), "oldObj");
            var newObjParam = Expression.Parameter(typeof(T), "newObj");

            var oldValueExp = Expression.Property(oldObjParam, property);
            var newValueExp = Expression.Property(newObjParam, property);
            var type = property.PropertyType;

            var equalityType = typeof(EqualityComparer<>).MakeGenericType(type);
            var equalsMethod = equalityType.GetMethod("Equals", new[] { type, type });

            if (equalsMethod == null)
                return (w, o, n) => { };

            var defaultEquality = Expression.Property(null, equalityType, "Default");
            var areEqual = Expression.Call(defaultEquality, equalsMethod, oldValueExp, newValueExp);

            var writePropName = Expression.Call(writerParam, typeof(Utf8JsonWriter).GetMethod("WritePropertyName", new[] { typeof(string) }), Expression.Constant(property.Name));
            var writeStartObj = Expression.Call(writerParam, typeof(Utf8JsonWriter).GetMethod("WriteStartObject", Type.EmptyTypes));

            var writeOld = GetWriteExpression(writerParam, "OldValue", oldValueExp, type);
            var writeNew = GetWriteExpression(writerParam, "NewValue", newValueExp, type);

            var writeEndObj = Expression.Call(writerParam, typeof(Utf8JsonWriter).GetMethod("WriteEndObject", Type.EmptyTypes));

            var body = Expression.Block(writePropName, writeStartObj, writeOld, writeNew, writeEndObj);
            var ifDiff = Expression.IfThen(Expression.Not(areEqual), body);

            return Expression.Lambda<Action<Utf8JsonWriter, T, T>>(ifDiff, writerParam, oldObjParam, newObjParam).Compile();
        }

        // --- متدهای کمکی GetWriteExpression و GetNonNullWriteExpression ---
        private static Expression GetWriteExpression(ParameterExpression writer, string key, Expression value, Type type)
        {
            var isNullable = !type.IsValueType || Nullable.GetUnderlyingType(type) != null;

            if (isNullable)
            {
                var isNull = Expression.Equal(value, Expression.Constant(null, type));
                var writeNull = Expression.Call(writer, typeof(Utf8JsonWriter).GetMethod("WriteNull", new[] { typeof(string) }), Expression.Constant(key));

                var nonNullExpression = GetNonNullWriteExpression(writer, key, value, type);

                return Expression.IfThenElse(isNull, writeNull, nonNullExpression);
            }
            else
            {
                return GetNonNullWriteExpression(writer, key, value, type);
            }
        }

        private static Expression GetNonNullWriteExpression(ParameterExpression writer, string key, Expression value, Type type)
        {
            Type underlyingType = Nullable.GetUnderlyingType(type) ?? type;
            MethodInfo writeMethod;

            if (underlyingType != type)
            {
                value = Expression.Convert(value, underlyingType);
            }

            if (underlyingType == typeof(string))
            {
                writeMethod = typeof(Utf8JsonWriter).GetMethod("WriteString", new[] { typeof(string), typeof(string) });
            }
            else if (underlyingType == typeof(int) || underlyingType == typeof(long) || underlyingType == typeof(double) || underlyingType == typeof(float) || underlyingType == typeof(decimal))
            {
                writeMethod = typeof(Utf8JsonWriter).GetMethod("WriteNumber", new[] { typeof(string), underlyingType });
            }
            else if (underlyingType == typeof(bool))
            {
                writeMethod = typeof(Utf8JsonWriter).GetMethod("WriteBoolean", new[] { typeof(string), typeof(bool) });
            }
            else if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
            {
                if (underlyingType == typeof(DateTimeOffset))
                {
                    writeMethod = typeof(Utf8JsonWriter).GetMethod("WriteString", new[] { typeof(string), typeof(DateTimeOffset) });
                }
                else
                {
                    writeMethod = typeof(Utf8JsonWriter).GetMethod("WriteString", new[] { typeof(string), typeof(DateTime) });
                }
            }
            else
            {
                var toStringMethod = underlyingType.GetMethod("ToString", Type.EmptyTypes);

                if (toStringMethod == null)
                {
                    toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
                }

                var toStringCall = Expression.Call(value, toStringMethod);
                writeMethod = typeof(Utf8JsonWriter).GetMethod("WriteString", new[] { typeof(string), typeof(string) });
                value = toStringCall;
            }

            return Expression.Call(writer, writeMethod, Expression.Constant(key), value);
        }
    }
}