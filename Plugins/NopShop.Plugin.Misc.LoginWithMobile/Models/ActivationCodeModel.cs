using System;
using System.ComponentModel.DataAnnotations;

namespace NopShop.Plugin.Misc.LoginWithMobile.Models
{
    public class ActivationCodeModel
    {
        public string Username { get; set; }

        public string MarketerUsername { get; set; }

        [Required(ErrorMessage = "کد فعال‌سازی را وارد کنید.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "کد فعال‌سازی باید ۶ رقم باشد.")]
        public string ActivationCode { get; set; }

        public string ReferralCode { get; set; }

        public string ReturnUrl { get; set; }

        public bool DisplayCaptcha { get; set; }

        public DateTime ActivationCodeExpiredDate { get; set; }

    }
}
