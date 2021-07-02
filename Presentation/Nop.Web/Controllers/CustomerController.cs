using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Fenchurch.Web.Models.Customer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Nop.Core;
using Nop.Core.Domain;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Http;
using Nop.Services.Authentication;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.CorporateCustomers;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Tax;
using Nop.Web.Extensions;
using Nop.Web.Factories;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Validators;
using Nop.Web.Models.Customer;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using iTextSharp.text.pdf;
using iTextSharp.text;
using Nop.Web.Models.Order;
using Fenchurch.Web.Factories;
using System.Net.Http.Headers;
using System.Net.Http;
using Nop.Services.Security;

namespace Nop.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CustomerController : BasePublicController
    {
        #region Fields

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
        private readonly IProductService _productService;
        private readonly IRequestOutputFactory _requestOutputFactory;
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
        private readonly ICorporateCustomerService _corporatecutomers;
        private readonly IWebHostEnvironment _HostEnvironment;
        private readonly IOrderProcessingService _orderProcessingService;

        #endregion

        #region Ctor

        public CustomerController(AddressSettings addressSettings,
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
            IProductService productService,
            IRequestOutputFactory requestOutputFactory,
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
            TaxSettings taxSettings, ICorporateCustomerService corporatecutomers,
            IWebHostEnvironment HostEnvironment,
            IOrderProcessingService orderProcessingService)
        {

            _HostEnvironment = HostEnvironment;
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
            _productService = productService;
            _requestOutputFactory = requestOutputFactory;
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
            _corporatecutomers = corporatecutomers;
            _orderProcessingService = orderProcessingService;
        }

        #endregion

        #region Utilities

        protected virtual void ValidateRequiredConsents(List<GdprConsent> consents, IFormCollection form)
        {
            foreach (var consent in consents)
            {
                var controlId = $"consent{consent.Id}";
                var cbConsent = form[controlId];
                if (StringValues.IsNullOrEmpty(cbConsent) || !cbConsent.ToString().Equals("on"))
                {
                    ModelState.AddModelError("", consent.RequiredMessage);
                }
            }
        }

        protected virtual string ParseCustomCustomerAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = "";
            var attributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in attributes)
            {
                var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var cblAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cblAttributes))
                            {
                                foreach (var item in cblAttributes.ToString().Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    var selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                            {
                                var enteredText = ctrlAttributes.ToString().Trim();
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.FileUpload:
                    //not supported customer attributes
                    default:
                        break;
                }
            }

            return attributesXml;
        }

        protected virtual void LogGdpr(Customer customer, CustomerInfoModel oldCustomerInfoModel,
            CustomerInfoModel newCustomerInfoModel, IFormCollection form)
        {
            try
            {
                //consents
                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var previousConsentValue = _gdprService.IsConsentAccepted(consent.Id, _workContext.CurrentCustomer.Id);
                    var controlId = $"consent{consent.Id}";
                    var cbConsent = form[controlId];
                    if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                    {
                        //agree
                        if (!previousConsentValue.HasValue || !previousConsentValue.Value)
                        {
                            _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                        }
                    }
                    else
                    {
                        //disagree
                        if (!previousConsentValue.HasValue || previousConsentValue.Value)
                        {
                            _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                        }
                    }
                }

                //newsletter subscriptions
                if (_gdprSettings.LogNewsletterConsent)
                {
                    if (oldCustomerInfoModel.Newsletter && !newCustomerInfoModel.Newsletter)
                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentDisagree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                    if (!oldCustomerInfoModel.Newsletter && newCustomerInfoModel.Newsletter)
                        _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                }

                //user profile changes
                if (!_gdprSettings.LogUserProfileChanges)
                    return;

                if (oldCustomerInfoModel.Gender != newCustomerInfoModel.Gender)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Gender")} = {newCustomerInfoModel.Gender}");

                if (oldCustomerInfoModel.FirstName != newCustomerInfoModel.FirstName)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.FirstName")} = {newCustomerInfoModel.FirstName}");

                if (oldCustomerInfoModel.LastName != newCustomerInfoModel.LastName)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.LastName")} = {newCustomerInfoModel.LastName}");

                if (oldCustomerInfoModel.ParseDateOfBirth() != newCustomerInfoModel.ParseDateOfBirth())
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.DateOfBirth")} = {newCustomerInfoModel.ParseDateOfBirth()}");

                if (oldCustomerInfoModel.Email != newCustomerInfoModel.Email)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Email")} = {newCustomerInfoModel.Email}");

                if (oldCustomerInfoModel.Company != newCustomerInfoModel.Company)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Company")} = {newCustomerInfoModel.Company}");

                if (oldCustomerInfoModel.StreetAddress != newCustomerInfoModel.StreetAddress)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StreetAddress")} = {newCustomerInfoModel.StreetAddress}");

                if (oldCustomerInfoModel.StreetAddress2 != newCustomerInfoModel.StreetAddress2)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StreetAddress2")} = {newCustomerInfoModel.StreetAddress2}");

                if (oldCustomerInfoModel.ZipPostalCode != newCustomerInfoModel.ZipPostalCode)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.ZipPostalCode")} = {newCustomerInfoModel.ZipPostalCode}");

                if (oldCustomerInfoModel.City != newCustomerInfoModel.City)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.City")} = {newCustomerInfoModel.City}");

                if (oldCustomerInfoModel.County != newCustomerInfoModel.County)
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.County")} = {newCustomerInfoModel.County}");

                if (oldCustomerInfoModel.CountryId != newCustomerInfoModel.CountryId)
                {
                    var countryName = _countryService.GetCountryById(newCustomerInfoModel.CountryId)?.Name;
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.Country")} = {countryName}");
                }

                if (oldCustomerInfoModel.StateProvinceId != newCustomerInfoModel.StateProvinceId)
                {
                    var stateProvinceName = _stateProvinceService.GetStateProvinceById(newCustomerInfoModel.StateProvinceId)?.Name;
                    _gdprService.InsertLog(customer, 0, GdprRequestType.ProfileChanged, $"{_localizationService.GetResource("Account.Fields.StateProvince")} = {stateProvinceName}");
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception.Message, exception, customer);
            }
        }

        #endregion

        #region Methods

        #region Login / logout

        //[HttpsRequirement]
        //available even when a store is closed....
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(bool? checkoutAsGuest)
        {
            var model = _customerModelFactory.PrepareLoginModel(checkoutAsGuest);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled
                                ? _customerService.GetCustomerByUsername(model.Username)
                                : _customerService.GetCustomerByEmail(model.Email);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //raise event       
                            _eventPublisher.Publish(new CustomerLoggedinEvent(customer));

                            //activity log
                            _customerActivityService.InsertActivity(customer, "PublicStore.Login",
                                _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                                return RedirectToRoute("Homepage");

                            return Redirect(returnUrl);
                        }
                    case CustomerLoginResults.CustomerNotExist:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.CustomerNotExist"));
                        break;
                    case CustomerLoginResults.Deleted:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.Deleted"));
                        break;
                    case CustomerLoginResults.NotActive:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotActive"));
                        break;
                    case CustomerLoginResults.NotRegistered:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.NotRegistered"));
                        break;
                    case CustomerLoginResults.LockedOut:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.LockedOut"));
                        break;
                    case CustomerLoginResults.MigratedUser:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials.MigratedUser"));
                        break;

                    case CustomerLoginResults.WrongPassword:
                    default:
                        ModelState.AddModelError("", _localizationService.GetResource("Account.Login.WrongCredentials"));
                        break;
                }
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareLoginModel(model.CheckoutAsGuest);
            return View(model);
        }

        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Logout()
        {
            if (_workContext.OriginalCustomerIfImpersonated != null)
            {
                //activity log
                _customerActivityService.InsertActivity(_workContext.OriginalCustomerIfImpersonated, "Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.StoreOwner"),
                        _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id),
                    _workContext.CurrentCustomer);

                _customerActivityService.InsertActivity("Impersonation.Finished",
                    string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Finished.Customer"),
                        _workContext.OriginalCustomerIfImpersonated.Email, _workContext.OriginalCustomerIfImpersonated.Id),
                    _workContext.OriginalCustomerIfImpersonated);

                //logout impersonated customer
                _genericAttributeService
                    .SaveAttribute<int?>(_workContext.OriginalCustomerIfImpersonated, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, null);

                //redirect back to customer details page (admin area)
                return RedirectToAction("Edit", "Customer", new { id = _workContext.CurrentCustomer.Id, area = AreaNames.Admin });
            }

            //activity log
            _customerActivityService.InsertActivity(_workContext.CurrentCustomer, "PublicStore.Logout",
                _localizationService.GetResource("ActivityLog.PublicStore.Logout"), _workContext.CurrentCustomer);

            //standard logout 
            _authenticationService.SignOut();

            //raise logged out event       
            _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

            //EU Cookie
            if (_storeInformationSettings.DisplayEuCookieLawWarning)
            {
                //the cookie law message should not pop up immediately after logout.
                //otherwise, the user will have to click it again...
                //and thus next visitor will not click it... so violation for that cookie law..
                //the only good solution in this case is to store a temporary variable
                //indicating that the EU cookie popup window should not be displayed on the next page open (after logout redirection to homepage)
                //but it'll be displayed for further page loads
                TempData[$"{NopCookieDefaults.Prefix}{NopCookieDefaults.IgnoreEuCookieLawWarning}"] = true;
            }

            return RedirectToRoute("Homepage");
        }

        #endregion

        #region Password recovery

        //[HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecovery()
        {
            var model = new PasswordRecoveryModel();
            model = _customerModelFactory.PreparePasswordRecoveryModel(model);

            return View(model);
        }

        [ValidateCaptcha]
        [HttpPost, ActionName("PasswordRecovery")]
        [FormValueRequired("send-email")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoverySend(PasswordRecoveryModel model, bool captchaValid)
        {
            // validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var customer = _customerService.GetCustomerByEmail(model.Email);
                if (customer != null && customer.Active && !customer.Deleted)
                {
                    //save token and current date
                    var passwordRecoveryToken = Guid.NewGuid();
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute,
                        passwordRecoveryToken.ToString());
                    DateTime? generatedDateTime = DateTime.UtcNow;
                    _genericAttributeService.SaveAttribute(customer,
                        NopCustomerDefaults.PasswordRecoveryTokenDateGeneratedAttribute, generatedDateTime);

                    //send email
                    _workflowMessageService.SendCustomerPasswordRecoveryMessage(customer,
                        _workContext.WorkingLanguage.Id);

                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailHasBeenSent");
                }
                else
                {
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.EmailNotFound");
                }
            }

            model = _customerModelFactory.PreparePasswordRecoveryModel(model);
            return View(model);
        }

        //[HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirm(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(_genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute)))
            {
                return base.View(new PasswordRecoveryConfirmModel
                {
                    DisablePasswordChanging = true,
                    Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordAlreadyHasBeenChanged")
                });
            }

            var model = _customerModelFactory.PreparePasswordRecoveryConfirmModel();

            //validate token
            if (!_customerService.IsPasswordRecoveryTokenValid(customer, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
            }

            //validate token expiration date
            if (_customerService.IsPasswordRecoveryLinkExpired(customer))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
            }

            return View(model);
        }

        [HttpPost, ActionName("PasswordRecoveryConfirm")]
        [FormValueRequired("set-password")]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult PasswordRecoveryConfirmPOST(string token, string email, Guid guid, PasswordRecoveryConfirmModel model)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            //validate token
            if (!_customerService.IsPasswordRecoveryTokenValid(customer, token))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.WrongToken");
                return View(model);
            }

            //validate token expiration date
            if (_customerService.IsPasswordRecoveryLinkExpired(customer))
            {
                model.DisablePasswordChanging = true;
                model.Result = _localizationService.GetResource("Account.PasswordRecovery.LinkExpired");
                return View(model);
            }

            if (ModelState.IsValid)
            {
                var response = _customerRegistrationService.ChangePassword(new ChangePasswordRequest(customer.Email,
                    false, _customerSettings.DefaultPasswordFormat, model.NewPassword));
                if (response.Success)
                {
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PasswordRecoveryTokenAttribute, "");

                    model.DisablePasswordChanging = true;
                    model.Result = _localizationService.GetResource("Account.PasswordRecovery.PasswordHasBeenChanged");
                }
                else
                {
                    model.Result = response.Errors.FirstOrDefault();
                }

                return View(model);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion     

        #region Register

        //[HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Register()
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            var model = new RegisterModel();
            model = _customerModelFactory.PrepareRegisterModel(model, false, setDefaultValues: true);

            return View(model);
        }


        [HttpPost]
        [ValidateCaptcha]
        [ValidateHoneypot]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult Register(RegisterModel model, string returnUrl, bool captchaValid, IFormFile uploadedFile1, IFormFile uploadedFile2, IFormCollection form)
        {
            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("RegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_customerService.IsRegistered(_workContext.CurrentCustomer))
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService
                    .GetAllConsents().Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            //Add validation if file is not present and fail model
            //if (uploadedFile1 == null && uploadedFile2 == null)
            //{
            //    ModelState.AddModelError("", _localizationService.GetResource("Common.MissingDucumentId"));
            // }
            //else
            //{
            //    var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
            //    var avatarMaxSize1 = _customerSettings.AvatarMaximumSizeBytes;
            //    if (uploadedFile2.Length > avatarMaxSize1 && uploadedFile1.Length > avatarMaxSize)
            //        ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MB"));
            //    else if (uploadedFile2.Length > avatarMaxSize1 && uploadedFile1.Length < avatarMaxSize)
            //             ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MBDocument2"));
            //    else if(uploadedFile2.Length < avatarMaxSize1 && uploadedFile1.Length > avatarMaxSize)
            //             ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MBDocument1"));
            //}

            if (uploadedFile1 == null)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.MissingDucumentId"));
            }
            else if (uploadedFile1 != null)
            {
                //var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                //var avatarMaxSize1 = _customerSettings.AvatarMaximumSizeBytes;
                var avatarMaxSize = 5048576;

                if (uploadedFile1.Length > avatarMaxSize)
                    ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MB"));
                //else if ( uploadedFile1.Length < avatarMaxSize)
                //         ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MBDocument2"));
                else if (uploadedFile1.Length > avatarMaxSize)
                    ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MBDocument1"));
            }
            if (uploadedFile1 != null)
            {
                if (uploadedFile1.ContentType == "application/pdf")
                {
                    ModelState.AddModelError("", _localizationService.GetResource("Please select .jpg, .jpeg, .png file."));
                }
            }

            //Validate the customer age is 18+
            DateTime age = DateTime.Now.AddYears(-18);
            DateTime birthDay = model.ParseDateOfBirth() ?? DateTime.Now;
            if (birthDay >= age)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.UnderAge"));
            }

            if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }

                var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.Standard;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                    model.Email,
                    _customerSettings.UsernamesEnabled ? model.Username : model.Email,
                    model.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //
                    if (uploadedFile1 != null && !string.IsNullOrEmpty(uploadedFile1.FileName))
                    {
                        //var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        //if (uploadedFile1.Length > avatarMaxSize)
                        //    ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MB"));

                        var customerPictureBinary = _downloadService.GetDownloadBits(uploadedFile1);
                        var customerIdentification = _pictureService.InsertPicture(customerPictureBinary, uploadedFile1.ContentType, null);
                        if (customerIdentification != null)
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerIdentification.Id);
                    }
                    //if (uploadedFile2 != null && !string.IsNullOrEmpty(uploadedFile2.FileName))
                    //{
                    //    var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                    //    //if (uploadedFile2.Length > avatarMaxSize)
                    //    //    ModelState.AddModelError("", _localizationService.GetResource("Common.LessThan2MB"));

                    //    var customerPictureBinary = _downloadService.GetDownloadBits(uploadedFile2);
                    //    var customerIdentification = _pictureService.InsertPicture(customerPictureBinary, uploadedFile1.ContentType, null);
                    //    if (customerIdentification != null)
                    //        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.Avatar2PictureIdAttribute, customerIdentification.Id);
                    //}

                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);

                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out _, out var vatAddress);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (_customerSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute,
                            model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                    }

                    if (_customerSettings.AcceptPrivacyPolicyEnabled)
                    {
                        //privacy policy is required
                        //GDPR
                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                        {
                            _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                    {
                        var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                        LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                        Email = customer.Email,
                        Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                        CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute)
                            : null,
                        StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)
                            : null,
                        County = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute),
                        City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute),
                        Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                        Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                        ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                        PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                        FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (_addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        //customer.Addresses.Add(defaultAddress);

                        _addressService.InsertAddress(defaultAddress);

                        _customerService.InsertCustomerAddress(customer, defaultAddress);

                        customer.BillingAddressId = defaultAddress.Id;
                        customer.ShippingAddressId = defaultAddress.Id;

                        _customerService.UpdateCustomer(customer);
                    }

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                            _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.AdminApproval });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                //raise event       
                                _eventPublisher.Publish(new CustomerActivatedEvent(customer));

                                var redirectUrl = Url.RouteUrl("RegisterResult",
                                    new { resultId = (int)UserRegistrationType.Standard, returnUrl }, _webHelper.CurrentRequestProtocol);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("Homepage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareRegisterModel(model, true, customerAttributesXml);
            return View(model);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult RegisterResult(int resultId)
        {
            var model = _customerModelFactory.PrepareRegisterResultModel(resultId);
            return View(model);
        }

        public virtual IActionResult CorporateCustomerFinalResult(int resultId)
        {
            return View();
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult CorporateCustomerFinalResult(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("Homepage");

            return Redirect(returnUrl);
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CorporateRegisterResult(int resultId)
        {
            var model = _corporatecutomers.GetCustomerByCorporateCustomerId(resultId);
            CorporateRegisterFileDownload(resultId);
            return View(model);
        }

        [HttpPost]
        [ValidateCaptcha]
        [ValidateHoneypot]
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CorporateRegisterResult(CorporateCustomer model, IFormFile[] uploadedFileCreditForm, IFormFile uploadedFileLiqorLicense)
        {
            var CorpCustmodel = _corporatecutomers.GetCustomerByCorporateCustomerId(model.Id);

            string CreditformFileName = string.Empty;
            string webrootpath = _HostEnvironment.WebRootPath;
            string root = webrootpath + "\\CorporateCustomers";
            string subdirLiqorLicense = root + "\\LiqorLicense\\";
            string subdirCreditForms = root + "\\CreditForms\\";

            if (uploadedFileLiqorLicense == null && uploadedFileCreditForm.Length == 0)
            {
                ModelState.AddModelError("CustomError", "Please upload Liquor licence & credit application form and each file size would be less than 5MB.");
                return View(model);
            }
            else if (uploadedFileLiqorLicense == null && uploadedFileCreditForm.Length != 0)
            {
                ModelState.AddModelError("CustomError", "Please upload liquor licence.");
                return View(model);
            }
            else if (uploadedFileLiqorLicense != null && uploadedFileCreditForm.Length == 0)
            {
                ModelState.AddModelError("CustomError", "Please upload credit application form.");
                return View(model);
            }
            if (uploadedFileCreditForm.Length == 1)
            {
                  if (uploadedFileCreditForm[0] != null && !string.IsNullOrEmpty(uploadedFileCreditForm[0].FileName))
                {
                  var FileMaxSize = 5048576;
                    if (uploadedFileLiqorLicense.Length > FileMaxSize)
                    {
                        ModelState.AddModelError("CustomError", "Liquor licence file size would be less than 5MB");
                        return View(model);
                    }

                    CreditformFileName = "Credit_Account_Application_" + CorpCustmodel.CompanyNo + Path.GetExtension(uploadedFileCreditForm[0].FileName);
                    if (uploadedFileCreditForm[0].Length > FileMaxSize)
                    {
                        ModelState.AddModelError("CustomError", "Credit application form size would be less than 5MB");
                        return View(model);
                    }
                    else
                    {
                        var filePath = Path.Combine(subdirCreditForms, CreditformFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            uploadedFileCreditForm[0].CopyTo(fileStream);
                        }
                    }
                }
            }
            else
            {
                if (Path.GetExtension(uploadedFileCreditForm[0].FileName).ToLower() == ".pdf")
                {
                    List<byte[]> combinedPdfs = new List<byte[]>();
                    foreach (var file in uploadedFileCreditForm)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                combinedPdfs.Add(fileBytes);
                            }
                        }
                    }

                    byte[] combinedpdfs = ConcatenatePdfs(combinedPdfs);
                    CreditformFileName = "Credit_Account_Application_" + CorpCustmodel.CompanyNo + ".pdf";
                    var filePath = Path.Combine(subdirCreditForms, CreditformFileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        fileStream.Write(combinedpdfs, 0, combinedpdfs.Length);
                        fileStream.Close();
                    }
                }
                else
                {
                    string contentRootPath = _HostEnvironment.ContentRootPath;
                    var htmlText = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "CreditPdfHtml/CreditAccountApplicationMERGE.html"));
                    StringBuilder SB = new StringBuilder(htmlText);
                    int i = 1;
                    foreach (var file in uploadedFileCreditForm)
                    {
                        if (file.Length > 0)
                        {
                            using (var ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                var fileBytes = ms.ToArray();
                                string s = Convert.ToBase64String(fileBytes);
                                // act on the Base64 data
                                if (i == 1)
                                {
                                    SB.Replace("$IMAGENOONE$", s);
                                }
                                else if (i == 2)
                                {
                                    SB.Replace("$IMAGENOTWO$", s);
                                }
                                else if (i == 3)
                                {
                                    SB.Replace("$IMAGENOTHREE$", s);
                                }
                                else if (i == 4)
                                {
                                    SB.Replace("$IMAGENOFOUR$", s);
                                }
                                i++;
                            }
                        }
                    }

                    SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                    converter.Options.MaxPageLoadTime = 240;
                    SelectPdf.PdfDocument doc = converter.ConvertHtmlString(SB.ToString());
                    CreditformFileName = "Credit_Account_Application_" + CorpCustmodel.CompanyNo + ".pdf";
                    doc.Save(Path.Combine(subdirCreditForms, CreditformFileName));
                    doc.Close();
                }
            }

            if (uploadedFileLiqorLicense != null && !string.IsNullOrEmpty(uploadedFileLiqorLicense.FileName))
            {
                var FileMaxSize = 5048576;
                if (uploadedFileLiqorLicense.Length > FileMaxSize)
                {
                    ModelState.AddModelError("CustomError", "Liquor licence file size would be less than 5MB");
                    return View(model);
                }


                //if (uploadedFileLiqorLicense.Length > 0)
                //{
                //    var filePath = Path.Combine(subdirLiqorLicense, "Liquor_License_" + CorpCustmodel.CompanyNo + Path.GetExtension(uploadedFileLiqorLicense.FileName));
                //    using (var fileStream = new FileStream(filePath, FileMode.Create))
                //    {
                //        uploadedFileLiqorLicense.CopyTo(fileStream);
                //    }
                //}
                if (uploadedFileLiqorLicense.Length > FileMaxSize)
                {
                    ModelState.AddModelError("CustomError", "Application form size would be less than 5MB");
                    return View(model);
                }
                else
                {
                    var filePath = Path.Combine(subdirLiqorLicense, "Liquor_License_" + CorpCustmodel.CompanyNo + Path.GetExtension(uploadedFileLiqorLicense.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadedFileLiqorLicense.CopyTo(fileStream);
                    }
                }
            }

            CorpCustmodel.CreditFormFile = CreditformFileName;
            CorpCustmodel.LiquoreLicenseFile = "Liquor_License_" + CorpCustmodel.CompanyNo + Path.GetExtension(uploadedFileLiqorLicense.FileName);
            _corporatecutomers.UpdateCorporateCustomer(CorpCustmodel);

            return RedirectToAction("CorporateCustomerFinalResult");
            //return View(model);
        }

        public byte[] ConcatenatePdfs(List<byte[]> documents)
        {
            using (var ms = new MemoryStream())
            {
                var outputDocument = new Document();
                var writer = new PdfCopy(outputDocument, ms);
                outputDocument.Open();

                foreach (var doc in documents)
                {
                    var reader = new PdfReader(doc);
                    for (var i = 1; i <= reader.NumberOfPages; i++)
                    {
                        writer.AddPage(writer.GetImportedPage(reader, i));
                    }
                    writer.FreeReader(reader);
                    reader.Close();
                }

                writer.Close();
                outputDocument.Close();
                var allPagesContent = ms.GetBuffer();
                ms.Flush();

                return allPagesContent;
            }
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CorporateRegisterFileDownload(int resultId)
        {
            var model = _corporatecutomers.GetCustomerByCorporateCustomerId(resultId);
            string webrootpath = _HostEnvironment.WebRootPath;
            string root = webrootpath + "\\CorporateCustomers";
            string subdirLiqorLicense = root + "\\LiqorLicense\\";
            string subdirCreditForms = root + "\\CreditForms\\";
            string filePath = Path.Combine(subdirCreditForms, "Credit_Account_Application_" + model.CompanyNo + ".pdf");
            return PhysicalFile(filePath, "application/pdf", Path.GetFileName(filePath));
        }

        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult RegisterResult(string returnUrl)
        {
            if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl))
                return RedirectToRoute("Homepage");

            return Redirect(returnUrl);
        }

        [HttpPost]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CheckUsernameAvailability(string username)
        {
            var usernameAvailable = false;
            var statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.NotAvailable");

            if (!UsernamePropertyValidator.IsValid(username, _customerSettings))
            {
                statusText = _localizationService.GetResource("Account.Fields.Username.NotValid");
            }
            else if (_customerSettings.UsernamesEnabled && !string.IsNullOrWhiteSpace(username))
            {
                if (_workContext.CurrentCustomer != null &&
                    _workContext.CurrentCustomer.Username != null &&
                    _workContext.CurrentCustomer.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase))
                {
                    statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.CurrentUsername");
                }
                else
                {
                    var customer = _customerService.GetCustomerByUsername(username);
                    if (customer == null)
                    {
                        statusText = _localizationService.GetResource("Account.CheckUsernameAvailability.Available");
                        usernameAvailable = true;
                    }
                }
            }

            return Json(new { Available = usernameAvailable, Text = statusText });
        }

        //[HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult AccountActivation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.AccountActivationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
                return
                    View(new AccountActivationModel
                    {
                        Result = _localizationService.GetResource("Account.AccountActivation.AlreadyActivated")
                    });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            //activate user account
            customer.Active = true;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, "");
            //send welcome message
            _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

            //raise event       
            _eventPublisher.Publish(new CustomerActivatedEvent(customer));

            var model = new AccountActivationModel
            {
                Result = _localizationService.GetResource("Account.AccountActivation.Activated")
            };
            return View(model);
        }


        #endregion

        #region Payments
        public IActionResult Payments()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = new CorporateCustomerPaymentsModel();
            model = _customerModelFactory.PrepareCorporateCustomerPayments(model, _workContext.CurrentCustomer);
            model.FromDate = DateTime.Now.AddMonths(-3);
            model.ToDate = DateTime.Now;
            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public JsonResult CreditPaymentList(CreditPaymentPostModel searchmodel)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return AccessDeniedDataTablesJson();

            var model = new List<OrderDetailsModel>();
            model = _customerModelFactory.PrepareCorporateCustomerCreditPaymentList(Convert.ToDateTime(searchmodel.FromDate), Convert.ToDateTime(searchmodel.ToDate), _workContext.CurrentCustomer);

            return Json(new { recordsTotal = model.Count(), data = model });
        }        

        [HttpPost]
        public IActionResult Payments(CorporateCustomerPaymentsModel model)
        {
            //string PxPayUserId = "FenchurchLiquorREST_Dev";
            //string PxPayKey = "a0155a53c4b40174500df6b893fd4bcdc34dd08847f555919312dbaa55148be2";

            //PxPay WS = new PxPay(PxPayUserId, PxPayKey);

            //RequestInput input = new RequestInput();

            //input.AmountInput = "1";
            //input.CurrencyInput = "NZD";
            //input.MerchantReference = Guid.NewGuid().ToString().Substring(0,10);
            //input.TxnType = "Purchase";
            //input.UrlFail = "http://localhost:15536/customer/WindCaveResponse";
            //input.UrlSuccess = "http://localhost:15536/customer/WindCaveResponse";

            //// TODO: GUID representing unique identifier for the transaction within the shopping cart (normally would be an order ID or similar)
            //Guid orderId = Guid.NewGuid();
            //input.TxnId = orderId.ToString().Substring(0, 16);

            //RequestOutput output = WS.GenerateRequest(input);

            //if (output.valid == "1")
            //{
            //    // Redirect user to payment page

            //    Response.Redirect(output.Url);
            //}
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (string.IsNullOrEmpty(model.SelectedCreditPayments))
                ModelState.AddModelError("", "There was no credit order selected.");

            if (ModelState.IsValid)
            {
                var output = _requestOutputFactory.GenerateRequestOutput(_workContext.CurrentCustomer, model.SelectedCreditPayments);
                if (output.valid == "1")
                {
                    // Redirect user to payment page
                    Response.Redirect(output.Url);
                }
            }           

            return View(model);
        }

        public IActionResult CreditOrderPaymentStatu(string result)
        {
            var ids = _orderProcessingService.ProcessCreditOrderPayment(result);
            ViewBag.ProcessOrderIds = string.Join(", ", ids);
            return View();
        }        

        /// <summary>
        /// Database lookup to check the status of the order or shopping cart
        /// </summary>
        /// <param name="TxnId"></param>
        /// <returns></returns>
        protected bool isProcessed(string TxnId)
        {
            // TODO: Check database if order relating to TxnId has alread been processed
            return false;
        }
        #endregion

        #region My account / Info

        //[HttpsRequirement]
        public virtual IActionResult Info()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = new CustomerInfoModel();
            model = _customerModelFactory.PrepareCustomerInfoModel(model, _workContext.CurrentCustomer, false);

            return View(model);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public virtual IActionResult Info(CustomerInfoModel model, IFormCollection form)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var oldCustomerModel = new CustomerInfoModel();

            var customer = _workContext.CurrentCustomer;

            //get customer info model before changes for gdpr log
            if (_gdprSettings.GdprEnabled & _gdprSettings.LogUserProfileChanges)
                oldCustomerModel = _customerModelFactory.PrepareCustomerInfoModel(oldCustomerModel, customer, false);

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService
                    .GetAllConsents().Where(consent => consent.DisplayOnCustomerInfoPage && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            try
            {
                if (ModelState.IsValid)
                {
                    //username 
                    if (_customerSettings.UsernamesEnabled && _customerSettings.AllowUsersToChangeUsernames)
                    {
                        if (!customer.Username.Equals(model.Username.Trim(), StringComparison.InvariantCultureIgnoreCase))
                        {
                            //change username
                            _customerRegistrationService.SetUsername(customer, model.Username.Trim());

                            //re-authenticate
                            //do not authenticate users in impersonation mode
                            if (_workContext.OriginalCustomerIfImpersonated == null)
                                _authenticationService.SignIn(customer, true);
                        }
                    }
                    //email
                    if (!customer.Email.Equals(model.Email.Trim(), StringComparison.InvariantCultureIgnoreCase))
                    {
                        //change email
                        var requireValidation = _customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation;
                        _customerRegistrationService.SetEmail(customer, model.Email.Trim(), requireValidation);

                        //do not authenticate users in impersonation mode
                        if (_workContext.OriginalCustomerIfImpersonated == null)
                        {
                            //re-authenticate (if usernames are disabled)
                            if (!_customerSettings.UsernamesEnabled && !requireValidation)
                                _authenticationService.SignIn(customer, true);
                        }
                    }

                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute,
                            model.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute,
                            model.VatNumber);
                        if (prevVatNumber != model.VatNumber)
                        {
                            var vatNumberStatus = _taxService.GetVatNumberStatus(model.VatNumber, out _, out var vatAddress);
                            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                            //send VAT number admin notification
                            if (!string.IsNullOrEmpty(model.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                                _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer,
                                    model.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                        }
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.FirstName);
                    if (_customerSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }
                    if (_customerSettings.CompanyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Newsletter)
                            {
                                var wasActive = newsletter.Active;
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);
                            }
                            else
                            {
                                _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            }
                        }
                        else
                        {
                            if (model.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = customer.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });
                            }
                        }
                    }

                    if (_forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.SignatureAttribute, model.Signature);

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                        LogGdpr(customer, oldCustomerModel, model, form);

                    return RedirectToRoute("CustomerInfo");
                }
            }
            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerInfoModel(model, customer, true, customerAttributesXml);
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult RemoveExternalAssociation(int id)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            //ensure it's our record
            var ear = _externalAuthenticationService.GetExternalAuthenticationRecordById(id);

            if (ear == null)
            {
                return Json(new
                {
                    redirect = Url.Action("Info"),
                });
            }

            _externalAuthenticationService.DeleteExternalAuthenticationRecord(ear);

            return Json(new
            {
                redirect = Url.Action("Info"),
            });
        }

        //[HttpsRequirement]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult EmailRevalidation(string token, string email, Guid guid)
        {
            //For backward compatibility with previous versions where email was used as a parameter in the URL
            var customer = _customerService.GetCustomerByEmail(email);
            if (customer == null)
                customer = _customerService.GetCustomerByGuid(guid);

            if (customer == null)
                return RedirectToRoute("Homepage");

            var cToken = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute);
            if (string.IsNullOrEmpty(cToken))
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource("Account.EmailRevalidation.AlreadyChanged")
                });

            if (!cToken.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                return RedirectToRoute("Homepage");

            if (string.IsNullOrEmpty(customer.EmailToRevalidate))
                return RedirectToRoute("Homepage");

            if (_customerSettings.UserRegistrationType != UserRegistrationType.EmailValidation)
                return RedirectToRoute("Homepage");

            //change email
            try
            {
                _customerRegistrationService.SetEmail(customer, customer.EmailToRevalidate, false);
            }
            catch (Exception exc)
            {
                return View(new EmailRevalidationModel
                {
                    Result = _localizationService.GetResource(exc.Message)
                });
            }
            customer.EmailToRevalidate = null;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.EmailRevalidationTokenAttribute, "");

            //re-authenticate (if usernames are disabled)
            if (!_customerSettings.UsernamesEnabled)
            {
                _authenticationService.SignIn(customer, true);
            }

            var model = new EmailRevalidationModel()
            {
                Result = _localizationService.GetResource("Account.EmailRevalidation.Changed")
            };
            return View(model);
        }

        #endregion

        #region My account / Addresses

        //[HttpsRequirement]
        public virtual IActionResult Addresses()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = _customerModelFactory.PrepareCustomerAddressListModel();
            return View(model);
        }

        [HttpPost]
        //[HttpsRequirement]
        public virtual IActionResult AddressDelete(int addressId)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var customer = _workContext.CurrentCustomer;

            //find address (ensure that it belongs to the current customer)
            var address = _customerService.GetCustomerAddress(customer.Id, addressId);
            if (address != null)
            {
                _customerService.RemoveCustomerAddress(customer, address);
                _customerService.UpdateCustomer(customer);
                //now delete the address record
                _addressService.DeleteAddress(address);
            }

            //redirect to the address list page
            return Json(new
            {
                redirect = Url.RouteUrl("CustomerAddresses"),
            });
        }

        //[HttpsRequirement]
        public virtual IActionResult AddressAdd()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = new CustomerAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressAdd(CustomerAddressEditModel model, IFormCollection form)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;


                _addressService.InsertAddress(address);

                _customerService.InsertCustomerAddress(_workContext.CurrentCustomer, address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);

            return View(model);
        }

        //[HttpsRequirement]
        public virtual IActionResult AddressEdit(int addressId)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = _customerService.GetCustomerAddress(customer.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            var model = new CustomerAddressEditModel();
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult AddressEdit(CustomerAddressEditModel model, int addressId, IFormCollection form)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = _customerService.GetCustomerAddress(customer.Id, addressId);
            if (address == null)
                //address is not found
                return RedirectToRoute("CustomerAddresses");

            //custom address attributes
            var customAttributes = _addressAttributeParser.ParseCustomAddressAttributes(form);
            var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            foreach (var error in customAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                return RedirectToRoute("CustomerAddresses");
            }

            //If we got this far, something failed, redisplay form
            _addressModelFactory.PrepareAddressModel(model.Address,
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id),
                overrideAttributesXml: customAttributes);
            return View(model);
        }

        #endregion

        #region My account / Downloadable products

        //[HttpsRequirement]
        public virtual IActionResult DownloadableProducts()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (_customerSettings.HideDownloadableProductsTab)
                return RedirectToRoute("CustomerInfo");

            var model = _customerModelFactory.PrepareCustomerDownloadableProductsModel();
            return View(model);
        }

        public virtual IActionResult UserAgreement(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return RedirectToRoute("Homepage");

            var product = _productService.GetProductById(orderItem.ProductId);

            if (product == null || !product.HasUserAgreement)
                return RedirectToRoute("Homepage");

            var model = _customerModelFactory.PrepareUserAgreementModel(orderItem, product);
            return View(model);
        }

        #endregion

        #region My account / Change password

        //[HttpsRequirement]
        public virtual IActionResult ChangePassword()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var model = _customerModelFactory.PrepareChangePasswordModel();

            //display the cause of the change password 
            if (_customerService.PasswordIsExpired(_workContext.CurrentCustomer))
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Account.ChangePassword.PasswordIsExpired"));

            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ChangePassword(ChangePasswordModel model)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return View(model);
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        #endregion

        #region My account / Avatar

        //[HttpsRequirement]
        public virtual IActionResult Avatar()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var model = new CustomerAvatarModel();
            model = _customerModelFactory.PrepareCustomerAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("upload-avatar")]
        public virtual IActionResult UploadAvatar(CustomerAvatarModel model, IFormFile uploadedFile, IFormFile uploadedFile2)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                try
                {
                    var customerAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
                    if (uploadedFile != null && !string.IsNullOrEmpty(uploadedFile.FileName))
                    {
                        var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile.Length > avatarMaxSize)
                            throw new NopException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        var customerPictureBinary = _downloadService.GetDownloadBits(uploadedFile);
                        if (customerAvatar != null)
                            customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile.ContentType, null);
                        else
                            customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile.ContentType, null);
                    }

                    var customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AvatarPictureIdAttribute, customerAvatarId);
                    model.AvatarUrl = _pictureService.GetPictureUrl(
                        _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute),
                        _mediaSettings.AvatarPictureSize,
                        false);

                    //Save the picture 2
                    customerAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.Avatar2PictureIdAttribute));
                    if (uploadedFile2 != null && !string.IsNullOrEmpty(uploadedFile2.FileName))
                    {
                        var avatarMaxSize = _customerSettings.AvatarMaximumSizeBytes;
                        if (uploadedFile2.Length > avatarMaxSize)
                            throw new NopException(string.Format(_localizationService.GetResource("Account.Avatar.MaximumUploadedFileSize"), avatarMaxSize));

                        var customerPictureBinary = _downloadService.GetDownloadBits(uploadedFile2);
                        if (customerAvatar != null)
                            customerAvatar = _pictureService.UpdatePicture(customerAvatar.Id, customerPictureBinary, uploadedFile2.ContentType, null);
                        else
                            customerAvatar = _pictureService.InsertPicture(customerPictureBinary, uploadedFile2.ContentType, null);
                    }

                    customerAvatarId = 0;
                    if (customerAvatar != null)
                        customerAvatarId = customerAvatar.Id;

                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.Avatar2PictureIdAttribute, customerAvatarId);

                    model.Avatar2Url = _pictureService.GetPictureUrl(
                        _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.Avatar2PictureIdAttribute),
                        _mediaSettings.AvatarPictureSize,
                        false);
                    return View(model);
                }
                catch (Exception exc)
                {
                    ModelState.AddModelError("", exc.Message);
                }
            }

            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCustomerAvatarModel(model);
            return View(model);
        }

        [HttpPost, ActionName("Avatar")]
        [FormValueRequired("remove-avatar")]
        public virtual IActionResult RemoveAvatar(CustomerAvatarModel model)
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_customerSettings.AllowCustomersToUploadAvatars)
                return RedirectToRoute("CustomerInfo");

            var customer = _workContext.CurrentCustomer;

            var customerAvatar = _pictureService.GetPictureById(_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.AvatarPictureIdAttribute));
            if (customerAvatar != null)
                _pictureService.DeletePicture(customerAvatar);
            _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AvatarPictureIdAttribute, 0);

            return RedirectToRoute("CustomerAvatar");
        }

        #endregion

        #region GDPR tools

        //[HttpsRequirement]
        public virtual IActionResult GdprTools()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            var model = _customerModelFactory.PrepareGdprToolsModel();
            return View(model);
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("export-data")]
        public virtual IActionResult GdprToolsExport()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            //log
            _gdprService.InsertLog(_workContext.CurrentCustomer, 0, GdprRequestType.ExportData, _localizationService.GetResource("Gdpr.Exported"));

            //export
            var bytes = _exportManager.ExportCustomerGdprInfoToXlsx(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id);

            return File(bytes, MimeTypes.TextXlsx, "customerdata.xlsx");
        }

        [HttpPost, ActionName("GdprTools")]
        [FormValueRequired("delete-account")]
        public virtual IActionResult GdprToolsDelete()
        {
            if (!_customerService.IsRegistered(_workContext.CurrentCustomer))
                return Challenge();

            if (!_gdprSettings.GdprEnabled)
                return RedirectToRoute("CustomerInfo");

            //log
            _gdprService.InsertLog(_workContext.CurrentCustomer, 0, GdprRequestType.DeleteCustomer, _localizationService.GetResource("Gdpr.DeleteRequested"));

            var model = _customerModelFactory.PrepareGdprToolsModel();
            model.Result = _localizationService.GetResource("Gdpr.DeleteRequested.Success");
            return View(model);
        }

        #endregion

        #region Check gift card balance

        //check gift card balance page
        //[HttpsRequirement]
        //available even when a store is closed
        [CheckAccessClosedStore(true)]
        public virtual IActionResult CheckGiftCardBalance()
        {
            if (!(_captchaSettings.Enabled && _customerSettings.AllowCustomersToCheckGiftCardBalance))
            {
                return RedirectToRoute("CustomerInfo");
            }

            var model = _customerModelFactory.PrepareCheckGiftCardBalanceModel();
            return View(model);
        }

        [HttpPost, ActionName("CheckGiftCardBalance")]
        [FormValueRequired("checkbalancegiftcard")]
        [ValidateCaptcha]
        public virtual IActionResult CheckBalance(CheckGiftCardBalanceModel model, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            }

            if (ModelState.IsValid)
            {
                var giftCard = _giftCardService.GetAllGiftCards(giftCardCouponCode: model.GiftCardCode).FirstOrDefault();
                if (giftCard != null && _giftCardService.IsGiftCardValid(giftCard))
                {
                    var remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_giftCardService.GetGiftCardRemainingAmount(giftCard), _workContext.WorkingCurrency);
                    model.Result = _priceFormatter.FormatPrice(remainingAmount, true, false);
                }
                else
                {
                    model.Message = _localizationService.GetResource("CheckGiftCardBalance.GiftCardCouponCode.Invalid");
                }
            }

            return View(model);
        }

        #endregion

        #region Corporate Customer

        //[HttpsRequirement]
        //available even when navigation is not allowed
        //[CheckAccessPublicStore(true)]

        public void GenerateCorporateCutomerPDF(int CorporateCustomerId)
        {

            var CorporateCustomerModel = _corporatecutomers.GetCustomerByCorporateCustomerId(CorporateCustomerId);
            if (CorporateCustomerModel != null)
            {
                var CustomerModel = _customerService.GetCustomerById(CorporateCustomerModel.Customer_Id);
                var model = new CustomerInfoModel();
                if (CustomerModel != null)
                {
                    model = _customerModelFactory.PrepareCustomerInfoModel(model, CustomerModel, false);
                }
                string contentRootPath = _HostEnvironment.ContentRootPath;
                var htmlText = System.IO.File.ReadAllText(Path.Combine(contentRootPath, "CreditPdfHtml/CreditAccountApplication.doc.html"));
                StringBuilder SB = new StringBuilder(htmlText);
                SB.Replace("$FULLLEGALNAME$", model.FirstName + " " + model.LastName);
                SB.Replace("$TRADINGNAME$", string.IsNullOrEmpty(model.Company) ? model.FirstName + " " + model.LastName : model.Company);
                SB.Replace("$PHYSICALADDRESS$", model.StreetAddress + " " + model.StreetAddress2 + " " + model.City);
                SB.Replace("$PHYSICALPOSTCODE$", model.ZipPostalCode);
                SB.Replace("$BILLINGADDRESS$", model.StreetAddress + " " + model.StreetAddress + " " + model.City);
                SB.Replace("$BILLINGPOSTCODE$", model.ZipPostalCode);
                SB.Replace("$EMAILADDRESS$", model.Email);
                SB.Replace("$PHONENO$", model.Phone);
                SB.Replace("$DOB$", string.Format("{0:MM-dd-yyyy}", model.ParseDateOfBirth()));
                SB.Replace("$DRIVERSLICENCENO$", CorporateCustomerModel.DriversLicenceNo);
                SB.Replace("$FAXNO$", model.Fax);
                SB.Replace("$MOBILENO$", model.Phone);
                SB.Replace("$COMPANYNO$", CorporateCustomerModel.CompanyNo);
                SB.Replace("$DATEINCORP$", CorporateCustomerModel.DateIncorporated.HasValue ? string.Format("{0:MM-dd-yyyy}", CorporateCustomerModel.DateIncorporated.Value.ToShortDateString()) : "");
                SB.Replace("$NATUREOFBUSINESS$", CorporateCustomerModel.NatureOfBusiness);
                SB.Replace("$GSTNO$", CorporateCustomerModel.GSTNo);
                SB.Replace("$PAIDUPCAPITAL$", Math.Round(CorporateCustomerModel.PaidUpCapital ?? 0, 2).ToString());
                SB.Replace("$MONTHLYPURCHASE$", Math.Round(CorporateCustomerModel.EstimatedMonthlyPurchase ?? 0, 2).ToString());
                SB.Replace("$CREDITLIMTREQUIRED$", Math.Round(CorporateCustomerModel.CreditLimtRequired ?? 0, 2).ToString());
                SB.Replace("$LICENSEEFULLNAME$", CorporateCustomerModel.LiquoredFullNameLicensee);
                SB.Replace("$LIQUORECOMPANYNO$", CorporateCustomerModel.LiquoredCompanyNo);
                SB.Replace("$LIQUORELICENCENO$", CorporateCustomerModel.LiquoredLicenceNo);
                SB.Replace("$LIQOREEXPIRYDATE$", CorporateCustomerModel.LiquoredLicenceExpiryDate == DateTime.MinValue ? "" : string.Format("{0:MM-dd-yyyy}", CorporateCustomerModel.LiquoredLicenceExpiryDate.ToShortDateString()));
                //SB.Replace("$TOBBACOLICENSENO$", CorporateCustomerModel.LiquoredTobaccoLicenceNo);
                //SB.Replace("$TOBBACOEXPIRYDATE$", CorporateCustomerModel.LiquoredTobaccoLicenceExpiryDate == DateTime.MinValue ? "" : string.Format("{0:MM-dd-yyyy}", CorporateCustomerModel.LiquoredTobaccoLicenceExpiryDate.ToShortDateString()));
                SB.Replace("$PREMISESADDRESS$", CorporateCustomerModel.LiquoredStreet);
                SB.Replace("$LIQUOREPREMISESPOSTCODE$", CorporateCustomerModel.LiquoredPostCode);
                SB.Replace("$PREVIOUSLICENSEDETAILS$", CorporateCustomerModel.LiquoredPreviousLicenceDetails);
                SB.Replace("$LIQUOREPHONE$", CorporateCustomerModel.LiquoredPhone);
                SB.Replace("$FACSIMILENO$", CorporateCustomerModel.LiquoredFacsimilieNumber);
                SB.Replace("$LANDLORDNAME$", CorporateCustomerModel.LiquoredLandlordName);
                SB.Replace("$DATELEASEDFROM$", CorporateCustomerModel.LiquoredDateLeasedFrom == DateTime.MinValue ? "" : string.Format("{0:MM-dd-yyyy}", CorporateCustomerModel.LiquoredDateLeasedFrom.ToShortDateString()));
                SB.Replace("$DATELEASEDTO$", CorporateCustomerModel.LiquoredDateLeasedTo == DateTime.MinValue ? "" : string.Format("{0:MM-dd-yyyy}", CorporateCustomerModel.LiquoredDateLeasedTo.ToShortDateString()));
                SB.Replace("$ACCOUNTEMAILADDRESS$", CorporateCustomerModel.AccountsEmailAddress);
                SB.Replace("$ACCOUNTCONTACT$", CorporateCustomerModel.AccountsContact);
                SB.Replace("$ACCOUNTPHONE$", CorporateCustomerModel.PhoneNo);
                SB.Replace("$ACCOUNTBANK$", CorporateCustomerModel.Bank);
                SB.Replace("$ACCOUNTBRANCH$", CorporateCustomerModel.Branch);
                SB.Replace("$ACCOUNTNO$", CorporateCustomerModel.AccountNo);

                if (CorporateCustomerModel.CustomerType == 1)
                {
                    SB.Replace("$BARS&RES$", "&#x2611;");
                    SB.Replace("$SPORTSCLUBS$", "&#x2610;");
                    SB.Replace("$FRIENDS&FAMILY$", "&#x2610;");
                    SB.Replace("$PRIVATEFUNCTIONS$", "&#x2610;");
                }
                else if (CorporateCustomerModel.CustomerType == 2)
                {
                    SB.Replace("$BARS&RES$", "&#x2610;");
                    SB.Replace("$SPORTSCLUBS$", "&#x2611;");
                    SB.Replace("$FRIENDS&FAMILY$", "&#x2610;");
                    SB.Replace("$PRIVATEFUNCTIONS$", "&#x2610;");
                }
                else if (CorporateCustomerModel.CustomerType == 3)
                {
                    SB.Replace("$BARS&RES$", "&#x2610;");
                    SB.Replace("$SPORTSCLUBS$", "&#x2610;");
                    SB.Replace("$FRIENDS&FAMILY$", "&#x2611;");
                    SB.Replace("$PRIVATEFUNCTIONS$", "&#x2610;");
                }
                else if (CorporateCustomerModel.CustomerType == 4)
                {
                    SB.Replace("$BARS&RES$", "&#x2610;");
                    SB.Replace("$SPORTSCLUBS$", "&#x2610;");
                    SB.Replace("$FRIENDS&FAMILY$", "&#x2610;");
                    SB.Replace("$PRIVATEFUNCTIONS$", "&#x2611;");
                }

                if (CorporateCustomerModel.PrincipalPlaceOfBusiness == 1)
                {
                    SB.Replace("$RENTED$", "&#9745;");
                    SB.Replace("$OWENED$", "&#9744;");
                    SB.Replace("$MORTGAGED$", "&#9744;");
                }
                else if (CorporateCustomerModel.PrincipalPlaceOfBusiness == 2)
                {
                    SB.Replace("$RENTED$", "&#9744;");
                    SB.Replace("$OWENED$", "&#9745;");
                    SB.Replace("$MORTGAGED$", "&#9744;");
                }
                else if (CorporateCustomerModel.PrincipalPlaceOfBusiness == 3)
                {
                    SB.Replace("$RENTED$", "&#9744;");
                    SB.Replace("$OWENED$", "&#9744;");
                    SB.Replace("$MORTGAGED$", "&#9745;");
                }

                if (CorporateCustomerModel.AccountTerms == 1)
                {
                    SB.Replace("$TWENTYDAYS$", "&#9745;");
                    SB.Replace("$COD$", "&#9744;");
                    SB.Replace("$OTHER$", "&#9744;");
                }
                else if (CorporateCustomerModel.AccountTerms == 2)
                {
                    SB.Replace("$TWENTYDAYS$", "&#9744;");
                    SB.Replace("$COD$", "&#9745;");
                    SB.Replace("$OTHER$", "&#9744;");
                }
                else if (CorporateCustomerModel.AccountTerms == 3)
                {
                    SB.Replace("$TWENTYDAYS$", "&#9744;");
                    SB.Replace("$COD$", "&#9744;");
                    SB.Replace("$OTHER$", "&#9745;");
                }

                if (CorporateCustomerModel.AccountsToBeEmailed)
                {
                    SB.Replace("$EMAILNO$", "&#9744;");
                    SB.Replace("$EMAILYES$", "&#9745;");
                }
                else
                {
                    SB.Replace("$EMAILNO$", "&#9745;");
                    SB.Replace("$EMAILYES$", "&#9744;");
                }

                if (CorporateCustomerModel.PurchaseOrderRequired)
                {
                    SB.Replace("$PURCHASEORDERNO$", "&#x2610");
                    SB.Replace("$PURCHASEORDERYES$", "&#x1F5F9;");
                }
                else
                {
                    SB.Replace("$PURCHASEORDERNO$", "&#x1F5F9;");
                    SB.Replace("$PURCHASEORDERYES$", "&#x2610;");
                }

                StringBuilder CompanyOwners = new StringBuilder();
                StringBuilder CompanyTradeReferencess = new StringBuilder();
                int OwnerCount = 1;
                foreach (var owner in _corporatecutomers.GetCompanyOwnersById(CorporateCustomerId))
                {
                    CompanyOwners.Append("<tr class=\"c1\"><td class=\"c48\" colspan=\"5\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">(" + OwnerCount + ") Full Name: " + owner.FullName + "</span></p></td><td class=\"c57\" colspan=\"5\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">D.O.B. " + string.Format("{0:MM-dd-yyyy}", owner.DateOfBirth) + "</span></p></td></tr>" +
                    "<tr class=\"c1\"><td class=\"c24\" colspan=\"9\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">Private Address: " + owner.PrivateAddress + "</span></p></td><td class=\"c35\" colspan=\"1\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">Postcode: " + owner.Postcode + "</span></p></td></tr>" +
                    "<tr class=\"c1\"><td class=\"c79\" colspan=\"3\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">Driver&rsquo;s Licence No: " + owner.DriversLicenceNo + "</span></p></td><td class=\"c54\" colspan=\"2\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">Phone No: " + owner.PhoneNo + "</span></p></td><td class=\"c57\" colspan=\"5\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">Mobile No: " + owner.MobileNo + "</span></p></td></tr>");
                    OwnerCount++;
                }
                int TradeReferenceCount = 1;
                foreach (var Reference in _corporatecutomers.GetCompanyReferencesById(CorporateCustomerId))
                {

                    CompanyTradeReferencess.Append("<tr class=\"c1\"><td class=\"c52\" colspan=\"1\" rowspan=\"1\"><p class=\"c20\"><span class=\"c0\">" + TradeReferenceCount + ". " + Reference.Name + "</span></p></td><td class=\"c43\" colspan=\"5\" rowspan=\"1\"><p class=\"c16\"><span class=\"c0\">" + Reference.Address + "</span></p></td><td class=\"c60\" colspan=\"4\" rowspan=\"1\"><p class=\"c16\"><span class=\"c0\">" + Reference.PhoneFaxEmail + "</span></p></td></tr>");

                    TradeReferenceCount++;
                }
                SB.Replace("$COMPANYOWNERS$", CompanyOwners.ToString());
                SB.Replace("$TRADEREFERENCES$", CompanyTradeReferencess.ToString());

                //SelectPdf.HtmlToPdf converter = new SelectPdf.HtmlToPdf();
                //converter.Options.MaxPageLoadTime = 240;
                //SelectPdf.PdfDocument doc = converter.ConvertHtmlString(SB.ToString());
                string webrootpath = _HostEnvironment.WebRootPath;
                string root = webrootpath + "\\CorporateCustomers";
                string subdirLiqorLicense = root + "\\LiqorLicense\\";
                string subdirCreditForms = root + "\\CreditForms\\";
                // If directory does not exist, create it. 
                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
                if (!Directory.Exists(subdirLiqorLicense))
                {
                    Directory.CreateDirectory(subdirLiqorLicense);
                }
                if (!Directory.Exists(subdirCreditForms))
                {
                    Directory.CreateDirectory(subdirCreditForms);
                }
                //doc.Save(Path.Combine(subdirCreditForms, "Credit_Account_Application_" + CorporateCustomerModel.CompanyNo + ".pdf"));
                //doc.Close();
                string pdfFileName = Path.Combine($"{subdirCreditForms}Credit_Account_Application_{CorporateCustomerModel.CompanyNo}.pdf");
                ConvertHtmlToPDF(pdfFileName, SB.ToString());
            }
        }

        private async void ConvertHtmlToPDF(string fileName, string html)
        {
            using (var client = new HttpClient())
            {
                string key = "5f08178bec521ea9e0052b133d4548fe";
                string pdfApiUrl = "https://api.pdflayer.com/";
                //Save the html into a HTML file
                string htmlFileName = string.Format("{0}.html", Guid.NewGuid());
                string htmlFilePath = Path.Combine(_HostEnvironment.WebRootPath, htmlFileName);
                System.IO.File.WriteAllText(htmlFilePath, html);

                //Convert the HTML into PDF using PDF Flayer API
                string url = $"{_storeContext.CurrentStore.Url}/{htmlFileName}";
                client.BaseAddress = new Uri(pdfApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = await client.GetAsync($"api/convert?access_key={key}&document_url={url}").ConfigureAwait(false);
                
                //If the request is successfly, then extract the data and write into the PDF file
                if (response.IsSuccessStatusCode)
                {  
                    var result = response.Content.ReadAsByteArrayAsync().Result;
                    System.IO.File.WriteAllBytes(fileName, result);
                }

                //delete the HTML file
                System.IO.File.Delete(htmlFilePath);
            }
        }

        public virtual IActionResult CorporateCustomerRegistration()
        {

            var model = new CorporateCustomerRegistrationModel();

            model = _customerModelFactory.PrepareCorporateCustomerRegistrationModel(model, false, setDefaultValues: true);
            return View(model);
        }


        [HttpPost]
        [ValidateCaptcha]
        [ValidateHoneypot]
        //available even when navigation is not allowed
        [CheckAccessPublicStore(true)]
        public virtual IActionResult CorporateCustomerRegistration(CorporateCustomerRegistrationModel model, string returnUrl, bool captchaValid, IFormFile uploadedFile, IFormCollection form)
        {


            //check whether registration is allowed
            if (_customerSettings.UserRegistrationType == UserRegistrationType.Disabled)
                return RedirectToRoute("CorporateRegisterResult", new { resultId = (int)UserRegistrationType.Disabled });

            if (_customerService.IsRegistered(_workContext.CurrentCustomer))
            {
                //Already registered customer. 
                _authenticationService.SignOut();

                //raise logged out event       
                _eventPublisher.Publish(new CustomerLoggedOutEvent(_workContext.CurrentCustomer));

                //Save a new record
                _workContext.CurrentCustomer = _customerService.InsertGuestCustomer();
            }
            var customer = _workContext.CurrentCustomer;
            customer.RegisteredInStoreId = _storeContext.CurrentStore.Id;



            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
            foreach (var error in customerAttributeWarnings)
            {
                ModelState.AddModelError("", error);
            }

            ////validate CAPTCHA
            //if (_captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage && !captchaValid)
            //{
            //    ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptchaMessage"));
            //}

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService
                    .GetAllConsents().Where(consent => consent.DisplayDuringRegistration && consent.IsRequired).ToList();

                ValidateRequiredConsents(consents, form);
            }

            //if (ModelState.IsValid)
            //{
                if (_customerSettings.UsernamesEnabled && model.Customer.Username != null)
                {
                    model.Customer.Username = model.Customer.Username.Trim();
                }

                var isApproved = _customerSettings.UserRegistrationType == UserRegistrationType.AdminApproval;
                var registrationRequest = new CustomerRegistrationRequest(customer,
                    model.Customer.Email,
                    _customerSettings.UsernamesEnabled ? model.Customer.Username : model.Customer.Email,
                    model.Customer.Password,
                    _customerSettings.DefaultPasswordFormat,
                    _storeContext.CurrentStore.Id,
                    isApproved);
                var registrationResult = _customerRegistrationService.RegisterCustomer(registrationRequest);
                if (registrationResult.Success)
                {
                    //properties
                    if (_dateTimeSettings.AllowCustomersToSetTimeZone)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.TimeZoneIdAttribute, model.Customer.TimeZoneId);
                    }
                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.Customer.VatNumber);

                        var vatNumberStatus = _taxService.GetVatNumberStatus(model.Customer.VatNumber, out _, out var vatAddress);
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberStatusIdAttribute, (int)vatNumberStatus);
                        //send VAT number admin notification
                        if (!string.IsNullOrEmpty(model.Customer.VatNumber) && _taxSettings.EuVatEmailAdminWhenNewVatSubmitted)
                            _workflowMessageService.SendNewVatSubmittedStoreOwnerNotification(customer, model.Customer.VatNumber, vatAddress, _localizationSettings.DefaultAdminLanguageId);
                    }

                    //form fields
                    if (_customerSettings.GenderEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.GenderAttribute, model.Customer.Gender);
                    if (_customerSettings.FirstNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FirstNameAttribute, model.Customer.FirstName);
                    if (_customerSettings.LastNameEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.LastNameAttribute, model.Customer.LastName);
                    if (_customerSettings.DateOfBirthEnabled)
                    {
                        var dateOfBirth = model.Customer.ParseDateOfBirth();
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.DateOfBirthAttribute, dateOfBirth);
                    }

                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Customer.Company);
                    if (_customerSettings.StreetAddressEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.Customer.StreetAddress);
                    if (_customerSettings.StreetAddress2Enabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.Customer.StreetAddress2);
                    if (_customerSettings.ZipPostalCodeEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.Customer.ZipPostalCode);
                    if (_customerSettings.CityEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.Customer.City);
                    if (_customerSettings.CountyEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.Customer.County);
                    if (_customerSettings.CountryEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.Customer.CountryId);
                    if (_customerSettings.CountryEnabled && _customerSettings.StateProvinceEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute,
                            model.Customer.StateProvinceId);
                    if (_customerSettings.PhoneEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Customer.Phone);
                    if (_customerSettings.FaxEnabled)
                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Customer.Fax);

                    //newsletter
                    if (_customerSettings.NewsletterEnabled)
                    {
                        //save newsletter value
                        var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(model.Customer.Email, _storeContext.CurrentStore.Id);
                        if (newsletter != null)
                        {
                            if (model.Customer.Newsletter)
                            {
                                newsletter.Active = true;
                                _newsLetterSubscriptionService.UpdateNewsLetterSubscription(newsletter);

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                            //else
                            //{
                            //When registering, not checking the newsletter check box should not take an existing email address off of the subscription list.
                            //_newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletter);
                            //}
                        }
                        else
                        {
                            if (model.Customer.Newsletter)
                            {
                                _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                {
                                    NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                    Email = model.Customer.Email,
                                    Active = true,
                                    StoreId = _storeContext.CurrentStore.Id,
                                    CreatedOnUtc = DateTime.UtcNow
                                });

                                //GDPR
                                if (_gdprSettings.GdprEnabled && _gdprSettings.LogNewsletterConsent)
                                {
                                    _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.Newsletter"));
                                }
                            }
                        }
                    }

                    if (_customerSettings.AcceptPrivacyPolicyEnabled)
                    {
                        //privacy policy is required
                        //GDPR
                        if (_gdprSettings.GdprEnabled && _gdprSettings.LogPrivacyPolicyConsent)
                        {
                            _gdprService.InsertLog(customer, 0, GdprRequestType.ConsentAgree, _localizationService.GetResource("Gdpr.Consent.PrivacyPolicy"));
                        }
                    }

                    //GDPR
                    if (_gdprSettings.GdprEnabled)
                    {
                        var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                        foreach (var consent in consents)
                        {
                            var controlId = $"consent{consent.Id}";
                            var cbConsent = form[controlId];
                            if (!StringValues.IsNullOrEmpty(cbConsent) && cbConsent.ToString().Equals("on"))
                            {
                                //agree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentAgree, consent.Message);
                            }
                            else
                            {
                                //disagree
                                _gdprService.InsertLog(customer, consent.Id, GdprRequestType.ConsentDisagree, consent.Message);
                            }
                        }
                    }

                    //save customer attributes
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //login customer now
                    if (isApproved)
                        _authenticationService.SignIn(customer, true);

                    //insert default address (if possible)
                    var defaultAddress = new Address
                    {
                        FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute),
                        LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute),
                        Email = customer.Email,
                        Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute),
                        CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute)
                            : null,
                        StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute) > 0
                            ? (int?)_genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute)
                            : null,
                        County = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute),
                        City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute),
                        Address1 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute),
                        Address2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute),
                        ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute),
                        PhoneNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute),
                        FaxNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute),
                        CreatedOnUtc = customer.CreatedOnUtc
                    };
                    if (_addressService.IsAddressValid(defaultAddress))
                    {
                        //some validation
                        if (defaultAddress.CountryId == 0)
                            defaultAddress.CountryId = null;
                        if (defaultAddress.StateProvinceId == 0)
                            defaultAddress.StateProvinceId = null;
                        //set default address
                        //customer.Addresses.Add(defaultAddress);

                        _addressService.InsertAddress(defaultAddress);

                        _customerService.InsertCustomerAddress(customer, defaultAddress);

                        customer.BillingAddressId = defaultAddress.Id;
                        customer.ShippingAddressId = defaultAddress.Id;

                        _customerService.UpdateCustomer(customer);
                    }

                    //Corporate Customer Insert start
                    CorporateCustomer CorpC = new CorporateCustomer();
                    CorpC.AccountNo = model.CorporateCustomer.AccountNo;

                    CorpC.CustomerType = model.CorporateCustomer.CustomerTypeEnum;
                    CorpC.Customer_Id = customer.Id;
                    CorpC.DateOfBirth = model.CorporateCustomer.DateOfBirth;
                    CorpC.DriversLicenceNo = model.CorporateCustomer.DriversLicenceNo;
                    CorpC.DateIncorporated = model.CorporateCustomer.DateIncorporated;
                    CorpC.CompanyNo = model.CorporateCustomer.CompanyNo;
                    CorpC.NatureOfBusiness = model.CorporateCustomer.NatureOfBusiness;
                    CorpC.GSTNo = model.CorporateCustomer.GSTNo;
                    CorpC.PaidUpCapital = model.CorporateCustomer.PaidUpCapital;
                    CorpC.EstimatedMonthlyPurchase = model.CorporateCustomer.EstimatedMonthlyPurchase;
                    if (model.CorporateCustomer.CreditLimtRequired != 0)
                    {
                        CorpC.CreditLimtRequired = model.CorporateCustomer.CreditLimtRequired;
                    }
                    CorpC.PrincipalPlaceOfBusiness = model.CorporateCustomer.PrincipalPlaceOfBusinessEnum;
                    CorpC.LiquoredFullNameLicensee = model.CorporateCustomer.LiquoredFullNameLicensee;
                    CorpC.LiquoredCompanyNo = model.CorporateCustomer.LiquoredCompanyNo;
                    CorpC.LiquoredLicenceNo = model.CorporateCustomer.LiquoredLicenceNo;
                    CorpC.LiquoredLicenceExpiryDate = model.CorporateCustomer.LiquoredLicenceExpiryDate;
                    CorpC.LiquoredTobaccoLicenceNo = model.CorporateCustomer.LiquoredTobaccoLicenceNo;
                    CorpC.LiquoredTobaccoLicenceExpiryDate = model.CorporateCustomer.LiquoredTobaccoLicenceExpiryDate;
                    CorpC.LiquoredStreet = model.CorporateCustomer.LiquoredStreet;
                    CorpC.LiquoredPhone = model.CorporateCustomer.LiquoredPhone;
                    CorpC.LiquoredSuburb = model.CorporateCustomer.LiquoredSuburb;
                    CorpC.LiquoredPostCode = model.CorporateCustomer.LiquoredPostCode;
                    CorpC.LiquoredFacsimilieNumber = model.CorporateCustomer.LiquoredFacsimilieNumber;
                    CorpC.LiquoredLandlordName = model.CorporateCustomer.LiquoredLandlordName;
                    CorpC.LiquoredDateLeasedFrom = Convert.ToDateTime(model.CorporateCustomer.LiquoredDateLeasedFrom);
                    CorpC.LiquoredDateLeasedTo = Convert.ToDateTime(model.CorporateCustomer.LiquoredDateLeasedTo);
                    CorpC.LiquoredPreviousLicenceDetails = model.CorporateCustomer.LiquoredPreviousLicenceDetails;
                    CorpC.AccountTerms = model.CorporateCustomer.AccountTermsEnum;
                    CorpC.PurchaseOrderRequired = model.CorporateCustomer.PurchaseOrderRequired;
                    CorpC.AccountsToBeEmailed = model.CorporateCustomer.AccountsToBeEmailed;
                    CorpC.AccountsEmailAddress = model.CorporateCustomer.AccountsEmailAddress;
                    CorpC.AccountsContact = model.CorporateCustomer.AccountsContact;
                    CorpC.PhoneNo = model.CorporateCustomer.PhoneNo;
                    CorpC.Bank = model.CorporateCustomer.Bank;
                    CorpC.Branch = model.CorporateCustomer.Branch;
                    CorpC.AccountNo = model.CorporateCustomer.AccountNo;
                    _corporatecutomers.InsertCorporateCustomer(CorpC);
                    //Corporate Customer Insert End

                    //References Insert Start
                    List<CorporateCustomerTradeReferencesModel> TradeReferences = JsonConvert.DeserializeObject<List<CorporateCustomerTradeReferencesModel>>(model.CorporateCustomer.CorporateReferences);

                    foreach (var TradeRef in TradeReferences)
                    {
                        CorporateCustomerTradeReferences TradeReference = new CorporateCustomerTradeReferences();
                        TradeReference.Address = TradeRef.Address;
                        TradeReference.Name = TradeRef.Name;
                        TradeReference.PhoneFaxEmail = TradeRef.PhoneFaxEmail;
                        TradeReference.CorporateCustomer_Id = CorpC.Id;
                        _corporatecutomers.InsertCorporateCustomerReferences(TradeReference);

                    }


                    //References Insert End

                    //Owners Insert Start
                    List<CorporateCustomerCompanyOwnersModel> CompanyOwners = JsonConvert.DeserializeObject<List<CorporateCustomerCompanyOwnersModel>>(model.CorporateCustomer.CorporateOwners);
                    foreach (var CompOwner in CompanyOwners)
                    {
                        CorporateCustomerCompanyOwners CompanyOwner = new CorporateCustomerCompanyOwners();
                        CompanyOwner.FullName = CompOwner.FullName;
                        CompanyOwner.PrivateAddress = CompOwner.PrivateAddress;
                        CompanyOwner.DateOfBirth = CompOwner.DateOfBirth;
                        CompanyOwner.DriversLicenceNo = CompOwner.DriversLicenceNo;
                        CompanyOwner.Postcode = CompOwner.Postcode;
                        CompanyOwner.PhoneNo = CompOwner.PhoneNo;
                        CompanyOwner.MobileNo = CompOwner.MobileNo;
                        CompanyOwner.CorporateCustomer_Id = CorpC.Id;
                        _corporatecutomers.InsertCorporateCustomerOwners(CompanyOwner);
                    }

                    //Owners Insert End

                    GenerateCorporateCutomerPDF(CorpC.Id);

                    //notifications
                    if (_customerSettings.NotifyNewCustomerRegistration)
                        _workflowMessageService.SendCustomerRegisteredNotificationMessage(customer,
                            _localizationSettings.DefaultAdminLanguageId);

                    //raise event       
                    _eventPublisher.Publish(new CustomerRegisteredEvent(customer));

                    switch (_customerSettings.UserRegistrationType)
                    {
                        case UserRegistrationType.EmailValidation:
                            {
                                //email validation message
                                _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.AccountActivationTokenAttribute, Guid.NewGuid().ToString());
                                _workflowMessageService.SendCustomerEmailValidationMessage(customer, _workContext.WorkingLanguage.Id);

                                //result
                                return RedirectToRoute("CorporateRegisterResult",
                                    new { resultId = (int)UserRegistrationType.EmailValidation });
                            }
                        case UserRegistrationType.AdminApproval:
                            {
                                return RedirectToRoute("CorporateRegisterResult",
                                    new { resultId = CorpC.Id });
                            }
                        case UserRegistrationType.Standard:
                            {
                                //send customer welcome message
                                _workflowMessageService.SendCustomerWelcomeMessage(customer, _workContext.WorkingLanguage.Id);

                                //raise event       
                                _eventPublisher.Publish(new CustomerActivatedEvent(customer));

                                var redirectUrl = Url.RouteUrl("CorporateRegisterResult",
                                    new { resultId = CorpC.Id, returnUrl }, _webHelper.CurrentRequestProtocol);
                                return Redirect(redirectUrl);
                            }
                        default:
                            {
                                return RedirectToRoute("Homepage");
                            }
                    }
                }

                //errors
                foreach (var error in registrationResult.Errors)
                    ModelState.AddModelError("", error);
            //}
            //RegisterModel Reg = model.Customer;
            //If we got this far, something failed, redisplay form
            model = _customerModelFactory.PrepareCorporateCustomerRegistrationModel(model, true, customerAttributesXml);
            return View(model);
        }

        #endregion

        #region Email Notification

        public IActionResult CustomerInvoiceEmailNotification(int id)
        {
            var order = _orderService.GetOrderById(id);
            return View(order);
        }
        #endregion

        #endregion
    }
}