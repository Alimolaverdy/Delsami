using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Configuration;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace NopShop.Plugin.Misc.LoginWithMobile.Models
{
    public class ConfigurationModel : ISettings
    {
        public ConfigurationModel()
        {
            AvailableSmsProviders = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("فعال سازی")]
        public bool Enable { get; set; }

        [NopResourceDisplayName("نمایش لینک ثبت نام با ایمیل")]
        public bool EnableLoginWithEmail { get; set; }

        [NopResourceDisplayName("تولید خودکار ایمیل")]
        public bool AutoGenerateEmail { get; set; }

        [NopResourceDisplayName("نام دامین")]
        public string StoreUrl { get; set; }

        [NopResourceDisplayName("رفتن به صفحه ویرایش اطلاعات کاربر بعد از ورود")]
        public bool RedirectToCustomerInfo { get; set; }

        public IList<SelectListItem> AvailableSmsProviders { get; set; }

        [NopResourceDisplayName("سرویس دهنده های پیامک")]
        public int SmsProviderId { get; set; }

        [NopResourceDisplayName("API Key")]
        public string SmsToken { get; set; }

        [NopResourceDisplayName("نام الگوی پیامک در کاوه نگار")]
        public string SharedTemplate { get; set; }

        [NopResourceDisplayName("استفاده از خط اختصاصی")]
        public bool UseDedicatedLine { get; set; }

        [NopResourceDisplayName("شماره خط اختصاصی")]
        public string SmsStaticNumber { get; set; }

        [NopResourceDisplayName("قالب پیامک خط اختصاصی")]
        public string MessageTemplate { get; set; }

        [NopResourceDisplayName("نمایش کپچا در صفحه ورود شماره موبایل")]
        public bool ShowCaptchaOnLoginWithMobilePage { get; set; }

        [NopResourceDisplayName("نمایش کپچا در صفحه تغییر شماره موبایل")]
        public bool ShowCaptchaOnChangePhoenumberPage { get; set; }

        [NopResourceDisplayName("نمایش کپچا در صفحه ورود کد فعال سازی")]
        public bool ShowCaptchaOnActivationCodePage { get; set; }

        [NopResourceDisplayName("نمایش کپچا در صفحه ورود کد فعال سازی برای تغییر شماره موبایل")]
        public bool ShowCaptchaOnChangePhoenumberActivationCodePage { get; set; }

        [NopResourceDisplayName("مدت زمان اعتبار کد فعال سازی")]
        public int ActivationCodeValidityPeriod { get; set; }

        [NopResourceDisplayName("تعداد دفعات مجاز ارسال کد فعال سازی")]
        public int ActivationCodeCountOfSend { get; set; }

        [NopResourceDisplayName("توضیحات صفحه ورود")]
        public string LoginDescription { get; set; }

        [NopResourceDisplayName("نام کاربری فرا پیامک")]
        public string FaraPayamakUsername { get; set; }

        [NopResourceDisplayName("رمز عبور فرا پیامک")]
        public string FaraPayamakPassword { get; set; }

        [NopResourceDisplayName("آیدی قالب فراپیامک")]
        public int FaraPayamakTemplateId { get; set; }

        [NopResourceDisplayName("استفاده از خط اختصاصی فراپیامک")]
        public bool FaraPayamakUseDedicatedLine { get; set; }

        [NopResourceDisplayName("شماره خط اختصاصی فراپیامک")]
        public int FaraPayamakDedicatedLineNumber { get; set; }

        [NopResourceDisplayName("قالب پیام اختصاصی فراپیامک")]
        public string FaraPayamakTemplateMessage { get; set; }

        [NopResourceDisplayName("امکان لاگین با 12345")]
        public bool EnableLoginWithOneToFive { get; set; }

    }
}
