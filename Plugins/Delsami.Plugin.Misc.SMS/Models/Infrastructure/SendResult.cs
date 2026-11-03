using System;
using Delsami.Plugin.Misc.SMS.Helpers;

namespace Delsami.Plugin.Misc.SMS.Models.Infrastructure
{
    public class SendResult
    {
        public long Messageid { get; set; }

        public int Cost { get; set; }

        public DateTime GregorianDate
        {
            get { return NPDateTimeHelper.UnixTimestampToDateTime(Date); }
            set { Date = NPDateTimeHelper.DateTimeToUnixTimestamp(value); }
        }

        public long Date { get; set; }

        public string Message { get; set; }

        public string Receptor { get; set; }

        public string Sender { get; set; }

        public int Status { get; set; }

        public string StatusText { get; set; }
    }
}