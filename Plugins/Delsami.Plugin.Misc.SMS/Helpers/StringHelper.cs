using System.Linq;

namespace Delsami.Plugin.Misc.SMS.Helpers
{
    public class StringHelper
    {
        public static string Join(string delimeter, string[] items)
        {
            var result = items.Aggregate("", (current, obj) => current + obj + ",");

            return result.Substring(0, result.Length - 1);
        }

        public static string Join(string delimeter, long[] items)
        {
            string result = items.Aggregate("", (current, obj) => current + obj.ToString() + ",");

            return result.Substring(0, result.Length - 1);
        }

        public static string NormalizePhoneNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            string[] persianDigits = new string[] { "۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹" };

            string[] arabicDigits = new string[] { "٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩" };

            for (int i = 0; i < 10; i++)
            {
                input = input.Replace(persianDigits[i], i.ToString());
                input = input.Replace(arabicDigits[i], i.ToString());
            }

            return input.Trim();
        }

    }

}