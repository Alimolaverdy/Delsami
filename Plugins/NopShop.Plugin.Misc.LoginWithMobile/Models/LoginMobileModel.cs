using Nop.Core.Domain.Customers;
using Nop.Web.Framework.Models;

namespace NopShop.Plugin.Misc.LoginWithMobile.Models
{
    public record LoginMobileModel : BaseNopEntityModel
    {
        public UserRegistrationType RegistrationType { get; set; }

        public bool CheckoutAsGuest { get; set; }

        public bool DisplayCaptcha { get; set; }

        public string Username { get; set; }

        public string ActiveUserName { get; set; }

        public string LoginDescription { get; set; }

        public bool EnableLoginWithEmail { get; set; }

    }
}
