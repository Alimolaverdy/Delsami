using System;
using System.Globalization;

namespace Delsami.Plugin.Misc.SMS.Helpers
{
    public class NPDateTimeHelper
    {
        private static PersianCalendar _pc = new();

        public static DateTime UnixTimestampToDateTime(long unixTimeStamp)
        {
            try
            {
                return new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(unixTimeStamp);
            }
            catch (Exception ex)
            {
                return DateTime.MaxValue;
            }
        }
        public static long DateTimeToUnixTimestamp(DateTime idateTime)
        {
            try
            {
                idateTime = new DateTime(idateTime.Year, idateTime.Month, idateTime.Day, idateTime.Hour, idateTime.Minute, idateTime.Second);
                TimeSpan unixTimeSpan = idateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local).ToLocalTime();
                return long.Parse(unixTimeSpan.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static int GetPersianYear(DateTime dateTime)
        {
            return _pc.GetYear(dateTime);
        }

        public static DateTime? GetDateTimeFromPersian(string date, string seperator = "/")
        {
            if (string.IsNullOrWhiteSpace(date))
                return null;

            var splittedDate = date.Split("/");

            return GetDateTimeFromPersian(int.Parse(splittedDate[0]), int.Parse(splittedDate[1]), int.Parse(splittedDate[2]));
        }

        public static DateTime GetDateTimeFromPersian(int year, int month, int day, int hour = 0, int minute = 0, int second = 0, int milliSecond = 0)
        {
            return _pc.ToDateTime(year, month, day, hour, minute, second, milliSecond);
        }

        public static string GetPersianStrFromDateTime(DateTime? dateTime, bool showTime = false)
        {
            if (!dateTime.HasValue)
                return string.Empty;

            var value = $"{_pc.GetYear(dateTime.Value)}/{_pc.GetMonth(dateTime.Value)}/{_pc.GetDayOfMonth(dateTime.Value)}";

            if (showTime)
                value += $"-{dateTime.Value.ToString("HH:mm:ss")}";

            return value;

        }

        public static int GetYearFromPersian(string date, string seperator = "/")
        {
            if (string.IsNullOrEmpty(date))
                return 0;

            var splittedDate = date.Split("/");

            return int.Parse(splittedDate[0]);

        }
    }
}