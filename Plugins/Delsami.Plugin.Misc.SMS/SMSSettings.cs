using Nop.Core.Configuration;

namespace Delsami.Plugin.Misc.SMS
{
    public class SMSSettings : ISettings
    {
        public string Apikey { get; set; }
        public string SenderNumber { get; set; }
        public int MaximumLengthOfSMS { get; set; }
        public string BaseUrl { get; set; }
    }
}
