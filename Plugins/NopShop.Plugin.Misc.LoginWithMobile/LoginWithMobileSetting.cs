using Nop.Core.Configuration;

namespace NopShop.Plugin.Misc.LoginWithMobile
{
    public class LoginWithMobileSetting : ISettings
    {
        public bool Enable { get; set; }

        public bool EnableLoginWithEmail { get; set; }

        public bool AutoGenerateEmail { get; set; }

        public string StoreUrl { get; set; }

        public bool RedirectToCustomerInfo { get; set; }

        public string SmsToken { get; set; }

        public bool UseDedicatedLine { get; set; }

        public string SharedTemplate { get; set; }

        public string SmsSharedNumber { get; set; }

        public string SmsStaticNumber { get; set; }

        public string MessageTemplate { get; set; }

        public bool ShowCaptchaOnLoginWithMobilePage { get; set; }

        public bool ShowCaptchaOnActivationCodePage { get; set; }

        public bool ShowCaptchaOnChangePhoenumberPage { get; set; }

        public bool ShowCaptchaOnChangePhoenumberActivationCodePage { get; set; }

        public int ActivationCodeValidityPeriod { get; set; }

        public int ActivationCodeCountOfSend { get; set; }

        public string LoginDescription { get; set; }

        public string FaraPayamakUsername { get; set; }

        public string FaraPayamakPassword { get; set; }

        public int FaraPayamakTemplateId { get; set; }

        public int SmsProviderId { get; set; }

        public bool FaraPayamakUseDedicatedLine { get; set; }

        public int FaraPayamakDedicatedLineNumber { get; set; }

        public string FaraPayamakTemplateMessage { get; set; }

        public bool EnableLoginWithOneToFive { get; set; }

    }
}
