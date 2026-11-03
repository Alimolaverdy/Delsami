using System;
using System.Globalization;

namespace Delsami.Plugin.Misc.Core.Helpers
{
    public class DateTimeHelper
    {
        private static PersianCalendar _pc = new();

        #region Methods

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

        public static string ToLocalGregorianDateTime(DateTime shamsiDate)
        {
            // Create a Gregorian CultureInfo
            CultureInfo enCulture = new CultureInfo("en-US");
            enCulture.DateTimeFormat.Calendar = new GregorianCalendar();

            // Format the date using Gregorian calendar
            string formatted = shamsiDate.ToLocalTime().ToString("yyyy-MM-ddTHH:mm:sszzz", enCulture);

            return formatted;
        }

        #endregion
    }

}