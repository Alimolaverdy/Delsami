using Force.DeepCloner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static void CopyPropertiesTo<T>(this object source, T destination)
        {
            var sourceProps = source.GetType().GetProperties();
            var destProps = typeof(T).GetProperties();
            foreach (var s in sourceProps)
            {
                var d = destProps.FirstOrDefault(x => x.Name == s.Name && x.PropertyType == s.PropertyType);
                if (d != null && d.CanWrite)
                {
                    d.SetValue(destination, s.GetValue(source, null), null);
                }
            }
        }


        public static T MyDeepClone<T>(this T source) => source.DeepClone();

    }


}