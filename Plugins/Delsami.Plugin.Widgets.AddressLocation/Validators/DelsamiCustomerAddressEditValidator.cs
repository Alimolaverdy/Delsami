using FluentValidation;
using Delsami.Plugin.Widgets.AddressLocation.Models.Addresses;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System.Linq;

namespace Delsami.Plugin.Widgets.AddressLocation.Validators
{
    public class DelsamiCustomerAddressEditValidator : AbstractValidator<DelsamiCustomerAddressEditModel>
    {
        public DelsamiCustomerAddressEditValidator(ILocalizationService localizationService,
            IStateProvinceService stateProvinceService,
            AddressSettings addressSettings,
            CustomerSettings customerSettings)
        {
            RuleFor(x => x.Address.FirstName)
                          .NotEmpty()
                          .WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.FirstName.Required"));
            RuleFor(x => x.Address.LastName)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.LastName.Required"));
            RuleFor(x => x.Address.Email)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.Email.Required"));
            RuleFor(x => x.Address.Email)
                .EmailAddress()
                .WithMessageAwait(localizationService.GetResourceAsync("Common.WrongEmail"));
            if (addressSettings.CountryEnabled)
            {
                RuleFor(x => x.Address.CountryId)
                    .NotNull()
                    .WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.Country.Required"));
                RuleFor(x => x.Address.CountryId)
                    .NotEqual(0)
                    .WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.Country.Required"));
            }
            if (addressSettings.CountryEnabled && addressSettings.StateProvinceEnabled)
            {
                RuleFor(x => x.Address.StateProvinceId).MustAwait(async (x, context) =>
                {
                    //does selected country has states?
                    var countryId = x.Address.CountryId ?? 0;
                    var hasStates = (await stateProvinceService.GetStateProvincesByCountryIdAsync(countryId)).Any();

                    if (hasStates)
                    {
                        //if yes, then ensure that state is selected
                        if (!x.Address.StateProvinceId.HasValue || x.Address.StateProvinceId.Value == 0)
                            return false;
                    }

                    return true;
                }).WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.StateProvince.Required"));
            }
            if (addressSettings.CompanyRequired && addressSettings.CompanyEnabled)
            {
                RuleFor(x => x.Address.Company).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Company.Required"));
            }
            if (addressSettings.StreetAddressRequired && addressSettings.StreetAddressEnabled)
            {
                RuleFor(x => x.Address.Address1).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StreetAddress.Required"));
            }
            if (addressSettings.StreetAddress2Required && addressSettings.StreetAddress2Enabled)
            {
                RuleFor(x => x.Address.Address2).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.StreetAddress2.Required"));
            }
            if (addressSettings.ZipPostalCodeRequired && addressSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.Address.ZipPostalCode).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.ZipPostalCode.Required"));
            }

            if (addressSettings.ZipPostalCodeEnabled)
            {
                RuleFor(x => x.Address.ZipPostalCode).Matches(@"^\d+$").WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.ZipPostalCode.Invalid")); ;
            }

            if (addressSettings.CountyEnabled && addressSettings.CountyRequired)
            {
                RuleFor(x => x.Address.County).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Address.Fields.County.Required"));
            }
            if (addressSettings.CityRequired && addressSettings.CityEnabled)
            {
                RuleFor(x => x.Address.CityId).NotEqual(0).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.City.Required"));
            }
            if (addressSettings.PhoneRequired && addressSettings.PhoneEnabled)
            {
                RuleFor(x => x.Address.PhoneNumber).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.Required"));
            }
            if (addressSettings.PhoneEnabled)
            {
                RuleFor(x => x.Address.PhoneNumber).IsPhoneNumber(customerSettings).WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Phone.NotValid"));
            }
            if (addressSettings.FaxRequired && addressSettings.FaxEnabled)
            {
                RuleFor(x => x.Address.FaxNumber).NotEmpty().WithMessageAwait(localizationService.GetResourceAsync("Account.Fields.Fax.Required"));
            }
        }
    }
}
