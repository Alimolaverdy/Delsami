using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Delsami.Plugin.Misc.Core.Helpers
{
    public class ValidationHelper
    {
        public static bool MobilePhoneNumberIsValid(string mobilePhoneNumber)
        {
            if (string.IsNullOrEmpty(mobilePhoneNumber))
                return false;

            var mobilePhoneNumberRegex = "^(?:(?:(?:\\+?|00)(98))|(0))?((?:90|91|92|93|99)[0-9]{8})$";

            var regex = new Regex(mobilePhoneNumberRegex);

            return regex.IsMatch(mobilePhoneNumber);
        }

        public static bool IsValidNationalCode(string nationalCode)
        {
            // بررسی طول کد ملی
            if (string.IsNullOrWhiteSpace(nationalCode) || nationalCode.Length != 10 || !long.TryParse(nationalCode, out _))
                return false;

            // بررسی یکسان بودن همه ارقام (مانند 1111111111)
            if (new string(nationalCode[0], 10) == nationalCode)
                return false;

            // محاسبه رقم کنترلی
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (nationalCode[i] - '0') * (10 - i);
            }

            int remainder = sum % 11;
            int controlDigit = nationalCode[9] - '0';

            // اعتبارسنجی رقم کنترلی
            return (remainder < 2 && controlDigit == remainder) || (remainder >= 2 && controlDigit == 11 - remainder);
        }

    }
}
