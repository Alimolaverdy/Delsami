using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Events;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Factories;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Themes;
using Nop.Web.Models.Customer;
using NopShop.Plugin.Misc.LoginWithMobile.Models;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Delsami.Plugin.Misc.Core;
using Delsami.Plugin.Misc.SMS.Servicecs;
using Delsami.Plugin.Misc.Core.Services;
using Delsami.Plugin.Misc.Core.Infrastructure;
using Delsami.Plugin.Misc.Core.Domain;

namespace NopShop.Plugin.Misc.LoginWithMobile.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class CustomerController : BasePluginController
    {
        #region Fields

        private readonly IThemeContext _themeContext;
        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IDownloadService _downloadService;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAddressService _addressService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICountryService _countryService;
        private readonly ICurrencyService _currencyService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IExportManager _exportManager;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGiftCardService _giftCardService;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly TaxSettings _taxSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly IRewardPointService _rewardPointService;
        private readonly LoginWithMobileSetting _loginWithMobileSetting;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ISMSSender _smsSender;
        private readonly IPermissionService _permissionService;
        private readonly IOtpService _otpService;
        private readonly IEncryptionService _encryptionService;
        private readonly CoreSettings _coreSettings;

        private const string ViewsBasePath = "~/Plugins/NopShop.Misc.LoginWithMobile/Themes/{0}/Views/Customer/{1}.cshtml";

        #endregion
        public CustomerController(IThemeContext themeContext,
            AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            IDownloadService downloadService,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressAttributeParser addressAttributeParser,
            IAddressModelFactory addressModelFactory,
            IAddressService addressService,
            IAuthenticationService authenticationService,
            ICountryService countryService,
            ICurrencyService currencyService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerModelFactory customerModelFactory,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IEventPublisher eventPublisher,
            IExportManager exportManager,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGiftCardService giftCardService,
            ILocalizationService localizationService,
            ILogger logger,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            ITaxService taxService,
            IWebHelper webHelper,
            IWorkContext workContext,
            IWorkflowMessageService workflowMessageService,
            LocalizationSettings localizationSettings,
            MediaSettings mediaSettings,
            StoreInformationSettings storeInformationSettings,
            TaxSettings taxSettings,
            RewardPointsSettings rewardPointsSettings,
            IRewardPointService rewardPointService,
            LoginWithMobileSetting loginWithMobileSetting,
            IDateTimeHelper dateTimeHelper,
            //IFaraPayamakSmsHelper faraPayamakSmsHelper,
            ISMSSender smsSender,
            IPermissionService permissionService,
            IOtpService otpService,
            IEncryptionService encryptionService,
            CoreSettings coreSettings)
        {
            _themeContext = themeContext;
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _downloadService = downloadService;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _addressAttributeParser = addressAttributeParser;
            _addressModelFactory = addressModelFactory;
            _addressService = addressService;
            _authenticationService = authenticationService;
            _countryService = countryService;
            _currencyService = currencyService;
            _customerActivityService = customerActivityService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerModelFactory = customerModelFactory;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _eventPublisher = eventPublisher;
            _exportManager = exportManager;
            _externalAuthenticationService = externalAuthenticationService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _giftCardService = giftCardService;
            _localizationService = localizationService;
            _logger = logger;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _taxService = taxService;
            _webHelper = webHelper;
            _workContext = workContext;
            _workflowMessageService = workflowMessageService;
            _localizationSettings = localizationSettings;
            _mediaSettings = mediaSettings;
            _storeInformationSettings = storeInformationSettings;
            _taxSettings = taxSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _rewardPointService = rewardPointService;
            _loginWithMobileSetting = loginWithMobileSetting;
            _dateTimeHelper = dateTimeHelper;
            _smsSender = smsSender;
            _permissionService = permissionService;
            _otpService = otpService;
            _encryptionService = encryptionService;
            _coreSettings = coreSettings;
        }

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult LoginWithEmailPasswordRecovery()
        {
            return RedirectToRoute("HomePage");
        }

        [HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult LoginWithEmailRegister()
        {
            return RedirectToAction("LoginWithEmail");
        }

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> LoginWithEmail
            (bool? checkoutAsGuest)
        {
            if (!_loginWithMobileSetting.EnableLoginWithEmail)
                return RedirectToAction("LoginWithMobile");

            var model = await _customerModelFactory.PrepareLoginModelAsync(checkoutAsGuest);

            return View(await GetViewPathAsync("Login"), model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual async Task<IActionResult> LoginWithEmail(LoginModel model, string returnUrl, bool captchaValid)
        {
            if (!_loginWithMobileSetting.EnableLoginWithEmail)
                return RedirectToAction("LoginWithMobile");

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = await _customerRegistrationService.ValidateCustomerAsync(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? await _customerService.GetCustomerByUsernameAsync(model.Username)
                                : await _customerService.GetCustomerByEmailAsync(model.Email);

                            //migrate shopping cart
                            await _shoppingCartService.MigrateShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), customer, true);

                            //sign in new customer
                            await _authenticationService.SignInAsync(customer, model.RememberMe);

                            //raise event       
                            await _eventPublisher.PublishAsync(new CustomerLoggedinEvent(customer));

                            //activity log
                            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.Login",
                                await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login"), customer);

                            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("Homepage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = await _customerModelFactory.PrepareLoginModelAsync(model.CheckoutAsGuest);

            return View(await GetViewPathAsync("Login"), model);
        }

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> LoginWithMobile(bool? checkoutAsGuest)
        {
            if (await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");

            var model = new LoginMobileModel();

            model.DisplayCaptcha = _loginWithMobileSetting.ShowCaptchaOnLoginWithMobilePage;
            model.EnableLoginWithEmail = _loginWithMobileSetting.EnableLoginWithEmail;
            model.LoginDescription = _loginWithMobileSetting.LoginDescription;

            return View(await GetViewPathAsync("LoginWithMobile"), model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> LoginWithMobile(LoginMobileModel model, string returnUrl, bool captchaValid, string referralCode)
        {
            if (await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");
            //validate CAPTCHA
            //_captchaSettings.ShowOnLoginPage &&
            if (_captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnLoginWithMobilePage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (string.IsNullOrWhiteSpace(model.Username))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.MobileNumber.Required"));
            else
                await ValidateMobileNumberAsync(model.Username, "NopShop.Plugin.Misc.LoginWithMobile.MobileNumber.InCorrect");

            var customer = await _customerService.GetCustomerByUsernameAsync(model.Username);

            if (customer is not null)
            {
                var createdOtp = await _otpService.GetOtpByUsernameAsync(model.Username);

                if (createdOtp is not null)
                {
                    var totalSeconds = (DateTime.UtcNow - createdOtp.CreateDateUtc).TotalSeconds;

                    if (createdOtp.ExpireOnUtc > DateTime.UtcNow)
                    {
                        ModelState.AddModelError("", string.Format(await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.TryAgainAfterSeconds"), (_loginWithMobileSetting.ActivationCodeValidityPeriod - (int)totalSeconds)));
                        await _logger.InformationAsync($"{model.Username} Try To Get ActivationCode Before End Of Limitation Time");
                    }

                    if (createdOtp.TryCount > _loginWithMobileSetting.ActivationCodeCountOfSend)
                    {
                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Limitation"));

                        PopulateLoginMobileModelDefaults(model);
                        return View(await GetViewPathAsync("LoginWithMobile"), model);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                if (customer == null)
                {
                    customer = await _workContext.GetCurrentCustomerAsync();
                    customer.Username = model.Username;

                    ////add reward points for customer registration (if enabled)
                    //if (_rewardPointsSettings.Enabled && _rewardPointsSettings.PointsForRegistration > 0)
                    //{
                    //    var endDate = _rewardPointsSettings.RegistrationPointsValidity > 0
                    //        ? (DateTime?)DateTime.UtcNow.AddDays(_rewardPointsSettings.RegistrationPointsValidity.Value) : null;
                    //    await _rewardPointService.AddRewardPointsHistoryEntryAsync(customer, _rewardPointsSettings.PointsForRegistration,
                    //     (await _storeContext.GetCurrentStoreAsync()).Id, await _localizationService.GetResourceAsync("RewardPoints.Message.EarnedForRegistration"), endDate: endDate);
                    //}

                    await _customerService.UpdateCustomerAsync(customer);

                    //await _eventPublisher.PublishAsync(new CustomerRegisteredEvent(customer));
                }

                try
                {
                    var (otp, activationCode, sentToMarketer, recipientUsername) =
                        await CreateAndSendOtpAsync(customer, model.Username, referralCode, false);

                    return RedirectToAction("ActivationCode", new
                    {
                        username = model.Username,
                        returnUrl,
                        marketerUsername = sentToMarketer ? recipientUsername : "",
                        referralCode = referralCode,
                    });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.SMS.Error"));

                    await _logger.ErrorAsync(ex.Message);

                    PopulateLoginMobileModelDefaults(model);
                    return View(await GetViewPathAsync("LoginWithMobile"), model);
                }
            }

            //If we got this far, something failed, redisplay form
            PopulateLoginMobileModelDefaults(model);
            return View(await GetViewPathAsync("LoginWithMobile"), model);
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> GetActivationCode(string phonenumber, string returnUrl, string referralCode, bool isResend = false)
        {
            if (await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return null;

            if (string.IsNullOrWhiteSpace(phonenumber))
                return null;

            if (phonenumber.Length != 11 || !Regex.IsMatch(phonenumber, "^[0-9]*$"))
                return null;

            if (isResend)
            {
                await _logger.InformationAsync($"ActivationCode Resend To {phonenumber}");
            }

            var dateTimeNow = DateTime.UtcNow;

            var customer = await _customerService.GetCustomerByUsernameAsync(phonenumber);
            if (customer == null)
                return null;

            var createdOtp = await _otpService.GetOtpByUsernameAsync(phonenumber);

            if (IsOtpLimitReached(createdOtp))
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Limitation"));
                return null;
            }

            if (createdOtp.ExpireOnUtc > DateTime.UtcNow)
                return Json(new
                {
                    message = await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Wait")
                });

            try
            {
                var (otp, activationCode, sentToMarketer, recipientUsername) =
                    await CreateAndSendOtpAsync(customer, phonenumber, referralCode, isResend);

                var activationCodeExpiredDate = _dateTimeHelper.ConvertToUserTime(otp.ExpireOnUtc, TimeZoneInfo.Utc, TimeZoneInfo.Local);

                if (activationCodeExpiredDate < DateTime.UtcNow)
                    return RedirectToRoute("Login");

                var remainingTime = GetRemainingtime(activationCodeExpiredDate);

                return Json(new
                {
                    message = "success",
                    expireDate = otp.ExpireOnUtc,
                    minutes = remainingTime.Min,
                    seconds = remainingTime.Sec
                });
            }
            catch
            {
                return null;
            }
        }

        [HttpPost]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> GetActivationCodeChange(string phonenumber)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return null;

            var dateTimeNow = DateTime.UtcNow;

            var customer = await _customerService.GetCustomerByUsernameAsync((await _workContext.GetCurrentCustomerAsync()).Username);
            if (customer == null)
                return null;

            var createdOtp = await _otpService.GetOtpByUsernameAsync(phonenumber);

            if (IsOtpLimitReached(createdOtp))
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Limitation"));
                return null;
            }

            if (createdOtp.ExpireOnUtc > DateTime.UtcNow)
                return Json(new
                {
                    message = await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Wait")
                });

            try
            {
                await _logger.InformationAsync($"OTP Request:{customer.Username}");

                var (otp, activationCode) = await CreateAndSendOtpAsync(customer, true);

                var activationCodeExpiredDate = _dateTimeHelper.ConvertToUserTime(otp.ExpireOnUtc, TimeZoneInfo.Utc, TimeZoneInfo.Local);

                if (activationCodeExpiredDate < DateTime.UtcNow)
                    return RedirectToRoute("Login");

                var remainingTime = GetRemainingtime(activationCodeExpiredDate);


                return Json(new
                {
                    message = "success",
                    expireDate = otp.ExpireOnUtc,
                    minutes = remainingTime.Min,
                    seconds = remainingTime.Sec
                });
            }
            catch
            {
                return null;
            }
        }

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> ActivationCode(string username, string marketerUsername, string returnUrl, string referralCode)
        {
            if (await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");

            var model = new ActivationCodeModel();
            model.Username = username;
            model.ReturnUrl = returnUrl;
            model.MarketerUsername = marketerUsername;
            model.ReferralCode = referralCode;
            model.DisplayCaptcha = _loginWithMobileSetting.ShowCaptchaOnActivationCodePage;
            var customer = await _customerService.GetCustomerByUsernameAsync(username);

            if (customer == null)
                return RedirectToRoute("HomePage");

            var createdOtp = await _otpService.GetOtpByUsernameAsync(username);

            model.ActivationCodeExpiredDate = _dateTimeHelper.ConvertToUserTime(createdOtp.ExpireOnUtc, TimeZoneInfo.Utc, TimeZoneInfo.Local);

            if (model.ActivationCodeExpiredDate < DateTime.UtcNow)
                return RedirectToRoute("Login", new { returnUrl });

            var prevDate = model.ActivationCodeExpiredDate;
            var today = DateTime.Now;

            var timeRemaining = prevDate - today;

            if (timeRemaining > TimeSpan.Zero)
            {
                ViewBag.Minutes = timeRemaining.Minutes;
                ViewBag.Secconds = timeRemaining.Seconds;
            }
            return View(await GetViewPathAsync("ActivationCode"), model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> ActivationCode(ActivationCodeModel model, bool captchaValid)
        {
            if (_captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnActivationCodePage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");

            if (model == null)
                throw new ArgumentNullException("ActivationCodeModel");

            var customer = await _customerService.GetCustomerByUsernameAsync(model.Username);
            if (customer == null)
                throw new NullReferenceException("customer");

            var pass = await _customerService.GetCurrentPasswordAsync(customer.Id);
            if (pass == null)
            {
                CustomerPassword customerPassword = new CustomerPassword
                {
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = customer.Id,
                    Password = customer.Username,
                    PasswordFormat = PasswordFormat.Hashed,
                    PasswordSalt = ""
                };
                await _customerService.InsertCustomerPasswordAsync(customerPassword);
            }

            var createdOtp = await _otpService.GetOtpByUsernameAsync(model.Username);

            if (createdOtp.ExpireOnUtc < DateTime.UtcNow)
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Expired"));

            var hasAccessAdmin = await _permissionService.AuthorizeAsync(StandardPermissionProvider.AccessAdminPanel, customer);

            if (!_loginWithMobileSetting.EnableLoginWithOneToFive || model.ActivationCode != "12345" || hasAccessAdmin)
            {
                if (IsOtpLimitReached(createdOtp))
                {
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Limitation"));
                }
                else
                {
                    if (_encryptionService.DecryptText(createdOtp.Value, Constants.OTPEncryptionKey) != model.ActivationCode)
                    {
                        createdOtp.TryCount = ++createdOtp.TryCount;

                        await _otpService.Update(createdOtp);

                        ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.IsNotValid"));
                    }
                    else
                    {
                        createdOtp.Used = true;
                        createdOtp.UsedDateUtc = DateTime.UtcNow;
                        await _otpService.Update(createdOtp);
                    }
                }
            }

            if (!ModelState.IsValid)
            {
                model.DisplayCaptcha = _captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnActivationCodePage;

                model.ActivationCodeExpiredDate = _dateTimeHelper.ConvertToUserTime(createdOtp.ExpireOnUtc, TimeZoneInfo.Utc, TimeZoneInfo.Local);

                if (model.ActivationCodeExpiredDate < DateTime.UtcNow)
                    return RedirectToRoute("Login", new { model.ReturnUrl });

                var timeRemaining = GetRemainingtime(model.ActivationCodeExpiredDate);
                ViewBag.Minutes = timeRemaining.Min;
                ViewBag.Secconds = timeRemaining.Sec;

                return View(await GetViewPathAsync("ActivationCode"), model);
            }

            var customerRoles = await _customerService.GetCustomerRolesAsync(customer);
            if (!customerRoles.Any(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName))
            {
                var registeredRole = await _customerService.GetCustomerRoleBySystemNameAsync(NopCustomerDefaults.RegisteredRoleName);
                if (registeredRole == null)
                    throw new NopException("'Registered' role could not be loaded");

                var customerCustomerRoleMapping = new CustomerCustomerRoleMapping()
                {
                    CustomerId = customer.Id,
                    CustomerRoleId = registeredRole.Id
                };
                await _customerService.AddCustomerRoleMappingAsync(customerCustomerRoleMapping);

                var guestRole = (await _customerService.GetCustomerRolesAsync(customer)).FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName);
                if (guestRole != null)
                {
                    await _customerService.RemoveCustomerRoleMappingAsync(customer, guestRole);
                }
            }

            if (_loginWithMobileSetting.AutoGenerateEmail && string.IsNullOrWhiteSpace(customer.Email))
            {
                customer.Email = customer.Username + "@" + _loginWithMobileSetting.StoreUrl;

                await _customerService.UpdateCustomerAsync(customer);
            }

            //migrate shopping cart
            await _shoppingCartService.MigrateShoppingCartAsync(await _workContext.GetCurrentCustomerAsync(), customer, true);

            //sign in new customer
            await _authenticationService.SignInAsync(customer, true);

            //raise event       
            await _eventPublisher.PublishAsync(new CustomerLoggedinEvent(customer));

            //activity log
            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.Login", await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login"));

            customer.LastLoginDateUtc = DateTime.UtcNow;

            await _customerService.UpdateCustomerAsync(customer);

            if (string.IsNullOrEmpty(model.ReturnUrl) || !Url.IsLocalUrl(model.ReturnUrl)
                || model.ReturnUrl == "/")
            {
                if (_loginWithMobileSetting.RedirectToCustomerInfo)
                    return RedirectToRoute("CustomerInfo");

                return RedirectToRoute("Homepage");
            }

            return Redirect(model.ReturnUrl);
        }

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> ChangePhonenumber()
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");

            var model = new LoginMobileModel();

            model.ActiveUserName = (await _workContext.GetCurrentCustomerAsync()).Username;
            model.DisplayCaptcha = _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberPage;

            return View(await GetViewPathAsync("ChangePhonenumber"), model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> ChangePhonenumber(LoginMobileModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberPage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            Customer customer = null;
            if (string.IsNullOrWhiteSpace(model.Username))
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.MobileNumber.Required"));
            else
            {
                if (model.Username.Length != 11 || !Regex.IsMatch(model.Username, "^[0-9]*$"))
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("Account.Login.Fields.Phonenumber.IsWrong"));

                customer = await _customerService.GetCustomerByUsernameAsync(model.Username);
                if (customer != null)
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.MobileNumber.Duplicate"));
            }

            if (ModelState.IsValid)
            {
                customer = await _workContext.GetCurrentCustomerAsync();

                var createdOtp = await _otpService.GetOtpByUsernameAsync(model.Username);

                if (IsOtpLimitReached(createdOtp))
                {
                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Limitation"));

                    PopulateLoginMobileModelDefaults(model);
                    return View(await GetViewPathAsync("ChangePhonenumber"), model);
                }

                await CreateAndSendOtpAsync(customer, false);

                return RedirectToAction("ChangePhonenumberActivationCode", new { model.Username });
            }

            //If we got this far, something failed, redisplay form
            model.ActiveUserName = (await _workContext.GetCurrentCustomerAsync()).Username;
            PopulateLoginMobileModelDefaults(model);
            return View(await GetViewPathAsync("ChangePhonenumber"), model);
        }

        [HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> ChangePhonenumberActivationCode(string username)
        {
            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");

            var model = new ActivationCodeModel();
            model.Username = username;
            model.DisplayCaptcha = _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberActivationCodePage;

            var customer = await _customerService.GetCustomerByUsernameAsync((await _workContext.GetCurrentCustomerAsync()).Username);
            if (customer == null)
                return RedirectToRoute("HomePage");

            var createdOtp = await _otpService.GetOtpByUsernameAsync(username);

            model.ActivationCodeExpiredDate = _dateTimeHelper.ConvertToUserTime(createdOtp.ExpireOnUtc, TimeZoneInfo.Utc, TimeZoneInfo.Local);

            if (model.ActivationCodeExpiredDate < DateTime.UtcNow)
                return RedirectToRoute("HomePage");

            return View(await GetViewPathAsync("ChangePhonenumberActivationCode"), model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public async Task<IActionResult> ChangePhonenumberActivationCode(ActivationCodeModel model, bool captchaValid)
        {
            if (_captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberActivationCodePage && !captchaValid)
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("Common.WrongCaptchaMessage"));
            }

            if (!await _customerService.IsRegisteredAsync(await _workContext.GetCurrentCustomerAsync()))
                return RedirectToRoute("Homepage");

            if (model == null)
                throw new ArgumentNullException("ActivationCodeModel");

            var customer = await _workContext.GetCurrentCustomerAsync();
            if (customer == null)
                throw new NullReferenceException("customer");

            var createdOtp = await _otpService.GetOtpByUsernameAsync(model.Username);

            if (createdOtp.ExpireOnUtc < DateTime.UtcNow)
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Expired"));

            if (IsOtpLimitReached(createdOtp))
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.Limitation"));
            }
            else
            {
                if (_encryptionService.DecryptText(createdOtp.Value, Constants.OTPEncryptionKey) != model.ActivationCode)
                {
                    createdOtp.TryCount = ++createdOtp.TryCount;

                    await _otpService.Update(createdOtp);

                    ModelState.AddModelError("", await _localizationService.GetResourceAsync("NopShop.Plugin.Misc.LoginWithMobile.ActivationCode.IsNotValid"));
                }
                else
                {
                    createdOtp.Used = true;
                    createdOtp.UsedDateUtc = DateTime.UtcNow;
                    await _otpService.Update(createdOtp);
                }
            }

            if (!ModelState.IsValid)
            {
                model.DisplayCaptcha = _captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnChangePhoenumberActivationCodePage;

                model.ActivationCodeExpiredDate = _dateTimeHelper.ConvertToUserTime(createdOtp.ExpireOnUtc, TimeZoneInfo.Utc, TimeZoneInfo.Local);

                if (model.ActivationCodeExpiredDate < DateTime.UtcNow)
                    return RedirectToAction("ChangePhonenumber");

                var timeRemaining = GetRemainingtime(model.ActivationCodeExpiredDate);
                ViewBag.Minutes = timeRemaining.Min;
                ViewBag.Secconds = timeRemaining.Sec;

                return View(await GetViewPathAsync("ChangePhonenumberActivationCode"), model);
            }

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                try
                {
                    var splitedEmail = customer.Email.Split('@');
                    if (customer.Username == splitedEmail[0])
                        customer.Email = model.Username + "@" + _loginWithMobileSetting.StoreUrl;
                }
                catch
                {
                }
            }

            customer.Username = model.Username;

            await _customerService.UpdateCustomerAsync(customer);

            //sign in new customer
            await _authenticationService.SignInAsync(customer, true);

            //raise event       
            await _eventPublisher.PublishAsync(new CustomerLoggedinEvent(customer));

            //activity log
            await _customerActivityService.InsertActivityAsync(customer, "PublicStore.Login", await _localizationService.GetResourceAsync("ActivityLog.PublicStore.Login"));

            if (String.IsNullOrEmpty(model.ReturnUrl) || !Url.IsLocalUrl(model.ReturnUrl))
                return RedirectToRoute("HomePage");

            return Redirect(model.ReturnUrl);
        }

        protected int GenerateActivationCode()
        {
            var rnd = new Random();
            return rnd.Next(111111, 999999);
        }

        protected string GenerateRandomString()
        {
            int length = 7;

            // creating a StringBuilder object()
            StringBuilder str_build = new StringBuilder();
            Random random = new Random();

            char letter;

            for (int i = 0; i < length; i++)
            {
                double flt = random.NextDouble();
                int shift = Convert.ToInt32(Math.Floor(25 * flt));
                letter = Convert.ToChar(shift + 65);
                str_build.Append(letter);
            }

            return str_build.ToString();
        }

        private async Task<bool> IsFirstLoginAsync(Customer customer)
        {
            return customer == null || await _customerService.IsGuestAsync(customer);
        }

        private async Task<string> GetViewPathAsync(string viewName)
        {
            var currentTheme = await _themeContext.GetWorkingThemeNameAsync();
            return string.Format(ViewsBasePath, currentTheme, viewName);
        }

        private async Task<(OTP Otp, int ActivationCode)> CreateAndSendOtpAsync(Customer customer, bool isResend)
        {
            var activationCode = GenerateActivationCode();

            var otp = new OTP
            {
                Username = customer.Username,
                Used = false,
                Value = _encryptionService.EncryptText(activationCode.ToString(), Constants.OTPEncryptionKey),
                CreateDateUtc = DateTime.UtcNow,
                ExpireOnUtc = DateTime.UtcNow.AddSeconds(_loginWithMobileSetting.ActivationCodeValidityPeriod),
                TryCount = 0,
            };

            await _otpService.Insert(otp);

            var formatedMessage = string.Format(_loginWithMobileSetting.MessageTemplate, activationCode.ToString());
            var queueSMS = await _smsSender.QueueSMSAsync(customer.Username, formatedMessage);
            await _smsSender.SendSMSAsync(queueSMS);

            return (otp, activationCode);
        }

        private async Task<(OTP Otp, int ActivationCode, bool SentToMarketer, string RecipientUsername)> CreateAndSendOtpAsync(Customer customer,
                                                                                                                                     string usernameForFirstLoginCheck,
                                                                                                                                     string referralCode,
                                                                                                                                     bool isResend)
        {

            var customerByUsername = await _customerService.GetCustomerByUsernameAsync(usernameForFirstLoginCheck);


            Customer marketerCustomer = null;

            var activationCode = GenerateActivationCode();

            var otp = new OTP
            {
                Username = customer.Username,
                Used = false,
                Value = _encryptionService.EncryptText(activationCode.ToString(), Constants.OTPEncryptionKey),
                CreateDateUtc = DateTime.UtcNow,
                ExpireOnUtc = DateTime.UtcNow.AddSeconds(_loginWithMobileSetting.ActivationCodeValidityPeriod),
                TryCount = 0,
            };


            var formatedMessage = string.Format(_loginWithMobileSetting.MessageTemplate, activationCode.ToString());

            var sentToMarketer = false;
            var recipientUsername = customer.Username;

            var queueSMS = await _smsSender.QueueSMSAsync(customer.Username, formatedMessage);

            await _otpService.Insert(otp);

            //await _smsSender.SendSMSAsync(queueSMS);

            return (otp, activationCode, sentToMarketer, recipientUsername);
        }

        private bool IsOtpLimitReached(OTP otp)
        {
            return otp.TryCount >= _loginWithMobileSetting.ActivationCodeCountOfSend && otp.ExpireOnUtc > DateTime.UtcNow;
        }

        private async Task<bool> ValidateMobileNumberAsync(string phoneNumber, string invalidResourceKey)
        {
            if (phoneNumber.Length != 11 || !Regex.IsMatch(phoneNumber, "^[0-9]*$"))
            {
                ModelState.AddModelError("", await _localizationService.GetResourceAsync(invalidResourceKey));
                return false;
            }
            return true;
        }

        private void PopulateLoginMobileModelDefaults(LoginMobileModel model)
        {
            model.RegistrationType = _customerSettings.UserRegistrationType;
            model.CheckoutAsGuest = model.CheckoutAsGuest;
            model.DisplayCaptcha = _captchaSettings.Enabled && _loginWithMobileSetting.ShowCaptchaOnLoginWithMobilePage;
            model.LoginDescription = _loginWithMobileSetting.LoginDescription;
        }


        private (int Min, int Sec) GetRemainingtime(DateTime expireTime)
        {
            var timeRemaining = expireTime - DateTime.Now;
            int minutes = 0, seconds = 0;
            if (timeRemaining > TimeSpan.Zero)
            {
                minutes = timeRemaining.Minutes;
                seconds = timeRemaining.Seconds;
            }

            return (minutes, seconds);
        }

    }
}