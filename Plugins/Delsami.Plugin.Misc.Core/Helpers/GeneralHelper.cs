using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Delsami.Plugin.Misc.Core.Enums;

namespace Delsami.Plugin.Misc.Core.Helpers
{
    public static class GeneralHelper
    {
        public static string NormalizeString(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;

            var replacements = new Dictionary<char, char>
            {
                // Arabic to Persian letters
                ['ي'] = 'ی',
                ['ك'] = 'ک',
                ['ئ'] = 'ی',
                ['ة'] = 'ه',
                ['ؤ'] = 'و',
                ['إ'] = 'ا',
                ['أ'] = 'ا',
                ['ٱ'] = 'ا',

                // Arabic digits (U+0660 to U+0669) → English
                ['٠'] = '0',
                ['١'] = '1',
                ['٢'] = '2',
                ['٣'] = '3',
                ['٤'] = '4',
                ['٥'] = '5',
                ['٦'] = '6',
                ['٧'] = '7',
                ['٨'] = '8',
                ['٩'] = '9',

                // Persian digits (U+06F0 to U+06F9) → English
                ['۰'] = '0',
                ['۱'] = '1',
                ['۲'] = '2',
                ['۳'] = '3',
                ['۴'] = '4',
                ['۵'] = '5',
                ['۶'] = '6',
                ['۷'] = '7',
                ['۸'] = '8',
                ['۹'] = '9'
            };

            var normalized = new StringBuilder(input.Length);

            foreach (char c in input)
            {
                normalized.Append(replacements.ContainsKey(c) ? replacements[c] : c);
            }

            return normalized.ToString();
        }

        public static string GenerateUrlSafeString(int length = 12)
        {
            using var rng = RandomNumberGenerator.Create();
            byte[] bytes = new byte[length];
            rng.GetBytes(bytes);

            // Convert to base64 and make URL-safe
            return Convert.ToBase64String(bytes)
                .Replace('+', '-')  // '+' becomes '-'
                .Replace('/', '_')  // '/' becomes '_'
                .Replace("=", "");  // Remove padding
        }

        public static MonitoringProvider GetMonitoringProviderBasedOnPaymentMethodSystemName(string paymentMethodSystemName)
        {
            switch (paymentMethodSystemName)
            {
                case "ZarIran.Payments.Digipay": return MonitoringProvider.Digipay;
                case "Payments.BankPasargad": return MonitoringProvider.Pasargad;
                case "ZarIran.Payments.Saman": return MonitoringProvider.Saman;
                case "ZarIran.Payments.Tara": return MonitoringProvider.Tara;
                case "ZarIran.Payment.Tejarat": return MonitoringProvider.Tejarat;
                case "ZarIran.Payments.TorobPay": return MonitoringProvider.Torobpay;
                case "ZarIran.Payments.Barook": return MonitoringProvider.Barook;
                case "Payments.Mellat": return MonitoringProvider.Mellat;
                default:
                    return MonitoringProvider.None;
            }
        }
    }
}