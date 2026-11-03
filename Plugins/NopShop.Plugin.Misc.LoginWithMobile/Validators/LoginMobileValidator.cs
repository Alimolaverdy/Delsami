using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using NopShop.Plugin.Misc.LoginWithMobile.Models;

namespace NopShop.Plugin.Misc.LoginWithMobile.Validators
{
    internal class LoginMobileValidator : BaseNopValidator<LoginMobileModel>
    {
        public LoginMobileValidator(ILocalizationService localizationService)
        {
            //RuleFor(x => x.ThreeRighNumberLicencePlate)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.CustomerCar.Fields.LicencePlate.Required"));

            //RuleFor(x => x.AlphabetLicencePlateStr)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.CustomerCar.Fields.LicencePlate.Required"));

            //RuleFor(x => x.IranNumberLicencePlate)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.CustomerCar.Fields.LicencePlate.Required"));

            //RuleFor(x => x.TwoLeftNumberLicencePlate)
            //    .NotEmpty()
            //    .WithMessageAwait(localizationService.GetResourceAsync("ZarIran.Plugin.Misc.CarInfo.CustomerCar.Fields.LicencePlate.Required"));

            RuleFor(x => x.Username)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.MobileNumber.Required"))
                .Matches(@"^09\d{9}$")
                .WithMessageAwait(localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.MobileNumber.Invalid"));

        }
    }
}