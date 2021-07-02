using System;
using System.Collections.Generic;
using System.Linq;
using Fenchurch.Web.Models.Customer;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.CorporateCustomers;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Models.Common;
using Nop.Web.Models.Customer;
using Nop.Web.Models.Order;

namespace Nop.Web.Factories
{
    /// <summary>
    /// Represents the customer model factory
    /// </summary>
    public partial class CustomerModelFactory : ICustomerModelFactory
    {
        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly CommonSettings _commonSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ForumSettings _forumSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly IAddressModelFactory _addressModelFactory;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly ICountryService _countryService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IProductService _productService;
        private readonly IReturnRequestService _returnRequestService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly MediaSettings _mediaSettings;
        private readonly OrderSettings _orderSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly SecuritySettings _securitySettings;
        private readonly TaxSettings _taxSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IAddressService _addressservice;

        #endregion

        #region Ctor

        public CustomerModelFactory(AddressSettings addressSettings,
            CaptchaSettings captchaSettings,
            CatalogSettings catalogSettings,
            CommonSettings commonSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            ExternalAuthenticationSettings externalAuthenticationSettings,
            ForumSettings forumSettings,
            GdprSettings gdprSettings,
            IAddressModelFactory addressModelFactory,
            IAuthenticationPluginManager authenticationPluginManager,
            ICountryService countryService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICorporateCustomerService corporateCustomerService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IProductService productService,
            IReturnRequestService returnRequestService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            MediaSettings mediaSettings,
            OrderSettings orderSettings,
            RewardPointsSettings rewardPointsSettings,
            SecuritySettings securitySettings,
            TaxSettings taxSettings,
            VendorSettings vendorSettings,
            IOrderProcessingService orderProcessingService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IAddressService addressservice)
        {
            _addressSettings = addressSettings;
            _captchaSettings = captchaSettings;
            _catalogSettings = catalogSettings;
            _commonSettings = commonSettings;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _externalAuthenticationService = externalAuthenticationService;
            _externalAuthenticationSettings = externalAuthenticationSettings;
            _forumSettings = forumSettings;
            _gdprSettings = gdprSettings;
            _addressModelFactory = addressModelFactory;
            _authenticationPluginManager = authenticationPluginManager;
            _countryService = countryService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _corporateCustomerService = corporateCustomerService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _productService = productService;
            _returnRequestService = returnRequestService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeMappingService = storeMappingService;
            _urlRecordService = urlRecordService;
            _workContext = workContext;
            _mediaSettings = mediaSettings;
            _orderSettings = orderSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _securitySettings = securitySettings;
            _taxSettings = taxSettings;
            _vendorSettings = vendorSettings;
            _orderProcessingService = orderProcessingService;
            _currencyService = currencyService;
            _priceFormatter = priceFormatter;
            _addressservice = addressservice;
        }

        #endregion

        #region Utilities

        protected virtual GdprConsentModel PrepareGdprConsentModel(GdprConsent consent, bool accepted)
        {
            if (consent == null)
                throw new ArgumentNullException(nameof(consent));

            var requiredMessage = _localizationService.GetLocalized(consent, x => x.RequiredMessage);
            return new GdprConsentModel
            {
                Id = consent.Id,
                Message = _localizationService.GetLocalized(consent, x => x.Message),
                IsRequired = consent.IsRequired,
                RequiredMessage = !string.IsNullOrEmpty(requiredMessage) ? requiredMessage : $"'{consent.Message}' is required",
                Accepted = accepted
            };
        }
        #endregion

        #region Methods

        /// <summary>
        /// Prepare the custom customer attribute models
        /// </summary>
        /// <param name="customer">Customer</param>
        /// <param name="overrideAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>List of the customer attribute model</returns>
        public virtual IList<CustomerAttributeModel> PrepareCustomCustomerAttributes(Customer customer, string overrideAttributesXml = "")
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            var result = new List<CustomerAttributeModel>();

            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = _localizationService.GetLocalized(attribute, x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = _localizationService.GetLocalized(attributeValue, x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !string.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CustomCustomerAttributes);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!string.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Any())
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.ImageSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                result.Add(attributeModel);
            }

            return result;
        }

        public virtual CorporateCustomerPaymentsModel PrepareCorporateCustomerPayments(CorporateCustomerPaymentsModel model, Customer customer)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (customer == null)
                throw new ArgumentNullException(nameof(model));

            //Get required data from DB
            var corporateCustomer = _corporateCustomerService.GetCustomerById(customer.Id);

            //Prepare the model with the data.
            model.CreditLimit = corporateCustomer.CreditLimit;
            model.AmountDue = _orderService.GetCustomerOverDueAmount(customer.Id);
            model.AvailableLimit = model.CreditLimit - model.AmountDue;

            return model;
        }

        public virtual List<OrderDetailsModel> PrepareCorporateCustomerCreditPaymentList(DateTime FromDate, DateTime ToDate, Customer Customer)
        {
            /*
            if (searchModel == null)
                throw new ArgumentNullException(nameof(searchModel));

            //get product tags
            var productTags = _productTagService.GetAllProductTags(tagName: searchModel.SearchTagName)
                .OrderByDescending(tag => _productTagService.GetProductCount(tag.Id, storeId: 0, showHidden: true)).ToList()
                .ToPagedList(searchModel);

            //prepare list model
            var model = new ProductTagListModel().PrepareToGrid(searchModel, productTags, () =>
            {
                return productTags.Select(tag =>
                {
                    //fill in model values from the entity
                    var productTagModel = tag.ToModel<ProductTagModel>();

                    //fill in additional values (not existing in the entity)
                    productTagModel.ProductCount = _productTagService.GetProductCount(tag.Id, storeId: 0, showHidden: true);

                    return productTagModel;
                });
            });

            return model;*/


            var model = new List<OrderDetailsModel>();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, createdFromUtc: FromDate, createdToUtc: ToDate.AddDays(1));

            foreach (var order in orders.Where(x => x.PaymentStatusId == 60))
            {
                var _billingAddress = _addressservice.GetAddressById(order.BillingAddressId);
                AddressModel BillingAddress = new AddressModel
                {
                    Address1 = _billingAddress.Address1,
                    Address2 = _billingAddress.Address2,
                    City = _billingAddress.City,
                    Email = _billingAddress.Email,
                    FirstName = _billingAddress.FirstName,
                    LastName = _billingAddress.LastName,
                    PhoneNumber = _billingAddress.PhoneNumber,
                    ZipPostalCode = _billingAddress.ZipPostalCode
                };
                Address _shippingAddress = new Address();
                AddressModel ShippingAddress = new AddressModel();
                if (order.ShippingAddressId.HasValue)
                {
                    _shippingAddress = _addressservice.GetAddressById(order.ShippingAddressId.Value);
                    ShippingAddress = new AddressModel
                    {
                        Address1 = _billingAddress.Address1,
                        Address2 = _billingAddress.Address2,
                        City = _billingAddress.City,
                        Email = _billingAddress.Email,
                        FirstName = _billingAddress.FirstName,
                        LastName = _billingAddress.LastName,
                        PhoneNumber = _billingAddress.PhoneNumber,
                        ZipPostalCode = _billingAddress.ZipPostalCode
                    };
                }

                var orderModel = new OrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    ShippingStatus = _localizationService.GetLocalizedEnum(order.ShippingStatus),
                    PaymentStatus = order.PaymentStatus,
                    CustomOrderNumber = order.CustomOrderNumber,
                    BillingAddress = BillingAddress,
                    ShippingAddress = ShippingAddress,
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage.Id);

                model.Add(orderModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the customer info model
        /// </summary>
        /// <param name="model">Customer info model</param>
        /// <param name="customer">Customer</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <returns>Customer info model</returns>
        public virtual CustomerInfoModel PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer,
            bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.VatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);
                model.FirstName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                model.LastName = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.LastNameAttribute);
                model.Gender = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.GenderAttribute);
                var dateOfBirth = _genericAttributeService.GetAttribute<DateTime?>(customer, NopCustomerDefaults.DateOfBirthAttribute);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Company = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CompanyAttribute);
                model.StreetAddress = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddressAttribute);
                model.StreetAddress2 = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.StreetAddress2Attribute);
                model.ZipPostalCode = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.ZipPostalCodeAttribute);
                model.City = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CityAttribute);
                model.County = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.CountyAttribute);
                model.CountryId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.CountryIdAttribute);
                model.StateProvinceId = _genericAttributeService.GetAttribute<int>(customer, NopCustomerDefaults.StateProvinceIdAttribute);
                model.Phone = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.PhoneAttribute);
                model.Fax = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.FaxAttribute);

                //newsletter
                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.SignatureAttribute);

                model.Email = customer.Email;
                model.Username = customer.Username;
            }
            else
            {
                if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            if (_customerSettings.UserRegistrationType == UserRegistrationType.EmailValidation)
                model.EmailToRevalidate = customer.EmailToRevalidate;

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetLocalized(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = _localizationService.GetLocalizedEnum((VatNumberStatus)_genericAttributeService
                .GetAttribute<int>(customer, NopCustomerDefaults.VatNumberStatusIdAttribute));
            model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            model.LastNameEnabled = _customerSettings.LastNameEnabled;
            model.FirstNameRequired = _customerSettings.FirstNameRequired;
            model.LastNameRequired = _customerSettings.LastNameRequired;
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.CompanyRequired = _customerSettings.CompanyRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
            model.CountyEnabled = _customerSettings.CountyEnabled;
            model.CountyRequired = _customerSettings.CountyRequired;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.CountryRequired = _customerSettings.CountryRequired;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
            model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            //external authentication
            model.AllowCustomersToRemoveAssociations = _externalAuthenticationSettings.AllowCustomersToRemoveAssociations;
            model.NumberOfExternalAuthenticationProviders = _authenticationPluginManager
                .LoadActivePlugins(_workContext.CurrentCustomer, _storeContext.CurrentStore.Id)
                .Count;
            foreach (var record in _externalAuthenticationService.GetCustomerExternalAuthenticationRecords(customer))
            {
                var authMethod = _authenticationPluginManager
                    .LoadPluginBySystemName(record.ProviderSystemName, _workContext.CurrentCustomer, _storeContext.CurrentStore.Id);
                if (!_authenticationPluginManager.IsPluginActive(authMethod))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
                {
                    Id = record.Id,
                    Email = record.Email,
                    ExternalIdentifier = !string.IsNullOrEmpty(record.ExternalDisplayIdentifier)
                        ? record.ExternalDisplayIdentifier : record.ExternalIdentifier,
                    AuthMethodName = _localizationService.GetLocalizedFriendlyName(authMethod, _workContext.WorkingLanguage.Id)
                });
            }

            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(customer, overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.CustomerAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayOnCustomerInfoPage).ToList();
                foreach (var consent in consents)
                {
                    var accepted = _gdprService.IsConsentAccepted(consent.Id, _workContext.CurrentCustomer.Id);
                    model.GdprConsents.Add(PrepareGdprConsentModel(consent, accepted.HasValue && accepted.Value));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the customer register model
        /// </summary>
        /// <param name="model">Customer register model</param>
        /// <param name="excludeProperties">Whether to exclude populating of model properties from the entity</param>
        /// <param name="overrideCustomCustomerAttributesXml">Overridden customer attributes in XML format; pass null to use CustomCustomerAttributes of customer</param>
        /// <param name="setDefaultValues">Whether to populate model properties by default values</param>
        /// <returns>Customer register model</returns>
        public virtual RegisterModel PrepareRegisterModel(RegisterModel model, bool excludeProperties,
            string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            model.LastNameEnabled = _customerSettings.LastNameEnabled;
            model.FirstNameRequired = _customerSettings.FirstNameRequired;
            model.LastNameRequired = _customerSettings.LastNameRequired;
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.CompanyRequired = _customerSettings.CompanyRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
            model.CountyEnabled = _customerSettings.CountyEnabled;
            model.CountyRequired = _customerSettings.CountyRequired;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.CountryRequired = _customerSettings.CountryRequired;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
            model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
            model.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;
            if (setDefaultValues)
            {
                //enable newsletter by default
                model.Newsletter = _customerSettings.NewsletterTickedByDefault;
            }

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });

                foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = _localizationService.GetLocalized(c, x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId, _workContext.WorkingLanguage.Id).ToList();
                    if (states.Any())
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetLocalized(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        var anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }

            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(_workContext.CurrentCustomer, overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.CustomerAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                foreach (var consent in consents)
                {
                    model.GdprConsents.Add(PrepareGdprConsentModel(consent, false));
                }
            }

            return model;
        }

        /// <summary>
        /// Prepare the login model
        /// </summary>
        /// <param name="checkoutAsGuest">Whether to checkout as guest is enabled</param>
        /// <returns>Login model</returns>
        public virtual LoginModel PrepareLoginModel(bool? checkoutAsGuest)
        {
            var model = new LoginModel
            {
                UsernamesEnabled = _customerSettings.UsernamesEnabled,
                RegistrationType = _customerSettings.UserRegistrationType,
                CheckoutAsGuest = checkoutAsGuest.GetValueOrDefault(),
                DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage
            };
            return model;
        }

        /// <summary>
        /// Prepare the password recovery model
        /// </summary>
        /// <param name="model">Password recovery model</param>
        /// <returns>Password recovery model</returns>
        public virtual PasswordRecoveryModel PreparePasswordRecoveryModel(PasswordRecoveryModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnForgotPasswordPage;

            return model;
        }

        /// <summary>
        /// Prepare the password recovery confirm model
        /// </summary>
        /// <returns>Password recovery confirm model</returns>
        public virtual PasswordRecoveryConfirmModel PreparePasswordRecoveryConfirmModel()
        {
            var model = new PasswordRecoveryConfirmModel();
            return model;
        }

        /// <summary>
        /// Prepare the register result model
        /// </summary>
        /// <param name="resultId">Value of UserRegistrationType enum</param>
        /// <returns>Register result model</returns>
        public virtual RegisterResultModel PrepareRegisterResultModel(int resultId)
        {
            var resultText = "";
            switch ((UserRegistrationType)resultId)
            {
                case UserRegistrationType.Disabled:
                    resultText = _localizationService.GetResource("Account.Register.Result.Disabled");
                    break;
                case UserRegistrationType.Standard:
                    resultText = _localizationService.GetResource("Account.Register.Result.Standard");
                    break;
                case UserRegistrationType.AdminApproval:
                    resultText = _localizationService.GetResource("Account.Register.Result.AdminApproval");
                    break;
                case UserRegistrationType.EmailValidation:
                    resultText = _localizationService.GetResource("Account.Register.Result.EmailValidation");
                    break;
                default:
                    break;
            }
            var model = new RegisterResultModel
            {
                Result = resultText
            };
            return model;
        }

        /// <summary>
        /// Prepare the customer navigation model
        /// </summary>
        /// <param name="selectedTabId">Identifier of the selected tab</param>
        /// <returns>Customer navigation model</returns>
        public virtual CustomerNavigationModel PrepareCustomerNavigationModel(int selectedTabId = 0)
        {
            var model = new CustomerNavigationModel();

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerInfo",
                Title = _localizationService.GetResource("Account.CustomerInfo"),
                Tab = CustomerNavigationEnum.Info,
                ItemClass = "customer-info"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerAddresses",
                Title = _localizationService.GetResource("Account.CustomerAddresses"),
                Tab = CustomerNavigationEnum.Addresses,
                ItemClass = "customer-addresses"
            });

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerOrders",
                Title = _localizationService.GetResource("Account.CustomerOrders"),
                Tab = CustomerNavigationEnum.Orders,
                ItemClass = "customer-orders"
            });

            if (_workContext.IsCorporateCustomer == true)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerPayments",
                    Title = _localizationService.GetResource("Account.CustomerPayments"),
                    Tab = CustomerNavigationEnum.CustomerPayments,
                    ItemClass = "customer-payments"
                });
            }

            if (_orderSettings.ReturnRequestsEnabled &&
                _returnRequestService.SearchReturnRequests(_storeContext.CurrentStore.Id,
                    _workContext.CurrentCustomer.Id, pageIndex: 0, pageSize: 1).Any())
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerReturnRequests",
                    Title = _localizationService.GetResource("Account.CustomerReturnRequests"),
                    Tab = CustomerNavigationEnum.ReturnRequests,
                    ItemClass = "return-requests"
                });
            }

            if (!_customerSettings.HideDownloadableProductsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerDownloadableProducts",
                    Title = _localizationService.GetResource("Account.DownloadableProducts"),
                    Tab = CustomerNavigationEnum.DownloadableProducts,
                    ItemClass = "downloadable-products"
                });
            }

            if (!_customerSettings.HideBackInStockSubscriptionsTab)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerBackInStockSubscriptions",
                    Title = _localizationService.GetResource("Account.BackInStockSubscriptions"),
                    Tab = CustomerNavigationEnum.BackInStockSubscriptions,
                    ItemClass = "back-in-stock-subscriptions"
                });
            }

            if (_rewardPointsSettings.Enabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerRewardPoints",
                    Title = _localizationService.GetResource("Account.RewardPoints"),
                    Tab = CustomerNavigationEnum.RewardPoints,
                    ItemClass = "reward-points"
                });
            }

            model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
            {
                RouteName = "CustomerChangePassword",
                Title = _localizationService.GetResource("Account.ChangePassword"),
                Tab = CustomerNavigationEnum.ChangePassword,
                ItemClass = "change-password"
            });

            if (_customerSettings.AllowCustomersToUploadAvatars)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerAvatar",
                    Title = _localizationService.GetResource("Account.Avatar"),
                    Tab = CustomerNavigationEnum.Avatar,
                    ItemClass = "customer-avatar"
                });
            }

            if (_forumSettings.ForumsEnabled && _forumSettings.AllowCustomersToManageSubscriptions)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerForumSubscriptions",
                    Title = _localizationService.GetResource("Account.ForumSubscriptions"),
                    Tab = CustomerNavigationEnum.ForumSubscriptions,
                    ItemClass = "forum-subscriptions"
                });
            }
            if (_catalogSettings.ShowProductReviewsTabOnAccountPage)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerProductReviews",
                    Title = _localizationService.GetResource("Account.CustomerProductReviews"),
                    Tab = CustomerNavigationEnum.ProductReviews,
                    ItemClass = "customer-reviews"
                });
            }
            if (_vendorSettings.AllowVendorsToEditInfo && _workContext.CurrentVendor != null)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CustomerVendorInfo",
                    Title = _localizationService.GetResource("Account.VendorInfo"),
                    Tab = CustomerNavigationEnum.VendorInfo,
                    ItemClass = "customer-vendor-info"
                });
            }
            if (_gdprSettings.GdprEnabled)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "GdprTools",
                    Title = _localizationService.GetResource("Account.Gdpr"),
                    Tab = CustomerNavigationEnum.GdprTools,
                    ItemClass = "customer-gdpr"
                });
            }

            if (_captchaSettings.Enabled && _customerSettings.AllowCustomersToCheckGiftCardBalance)
            {
                model.CustomerNavigationItems.Add(new CustomerNavigationItemModel
                {
                    RouteName = "CheckGiftCardBalance",
                    Title = _localizationService.GetResource("CheckGiftCardBalance"),
                    Tab = CustomerNavigationEnum.CheckGiftCardBalance,
                    ItemClass = "customer-check-gift-card-balance"
                });
            }

            model.SelectedTab = (CustomerNavigationEnum)selectedTabId;

            return model;
        }

        /// <summary>
        /// Prepare the customer address list model
        /// </summary>
        /// <returns>Customer address list model</returns>
        public virtual CustomerAddressListModel PrepareCustomerAddressListModel()
        {
            var addresses = _customerService.GetAddressesByCustomerId(_workContext.CurrentCustomer.Id)
                //enabled for the current store
                .Where(a => a.CountryId == null || _storeMappingService.Authorize(_countryService.GetCountryByAddress(a)))
                .ToList();

            var model = new CustomerAddressListModel();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                _addressModelFactory.PrepareAddressModel(addressModel,
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    loadCountries: () => _countryService.GetAllCountries(_workContext.WorkingLanguage.Id));
                model.Addresses.Add(addressModel);
            }
            return model;
        }

        /// <summary>
        /// Prepare the customer downloadable products model
        /// </summary>
        /// <returns>Customer downloadable products model</returns>
        public virtual CustomerDownloadableProductsModel PrepareCustomerDownloadableProductsModel()
        {
            var model = new CustomerDownloadableProductsModel();
            var items = _orderService.GetDownloadableOrderItems(_workContext.CurrentCustomer.Id);
            foreach (var item in items)
            {
                var order = _orderService.GetOrderById(item.OrderId);
                var product = _productService.GetProductById(item.ProductId);

                var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = order.Id,
                    CustomOrderNumber = order.CustomOrderNumber,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    ProductName = _localizationService.GetLocalized(product, x => x.Name),
                    ProductSeName = _urlRecordService.GetSeName(product),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductId
                };
                model.Items.Add(itemModel);

                if (_orderService.IsDownloadAllowed(item))
                    itemModel.DownloadId = product.DownloadId;

                if (_orderService.IsLicenseDownloadAllowed(item))
                    itemModel.LicenseId = item.LicenseDownloadId ?? 0;
            }

            return model;
        }

        /// <summary>
        /// Prepare the user agreement model
        /// </summary>
        /// <param name="orderItem">Order item</param>
        /// <param name="product">Product</param>
        /// <returns>User agreement model</returns>
        public virtual UserAgreementModel PrepareUserAgreementModel(OrderItem orderItem, Product product)
        {
            if (orderItem == null)
                throw new ArgumentNullException(nameof(orderItem));

            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var model = new UserAgreementModel
            {
                UserAgreementText = product.UserAgreementText,
                OrderItemGuid = orderItem.OrderItemGuid
            };

            return model;
        }

        /// <summary>
        /// Prepare the change password model
        /// </summary>
        /// <returns>Change password model</returns>
        public virtual ChangePasswordModel PrepareChangePasswordModel()
        {
            var model = new ChangePasswordModel();
            return model;
        }

        /// <summary>
        /// Prepare the customer avatar model
        /// </summary>
        /// <param name="model">Customer avatar model</param>
        /// <returns>Customer avatar model</returns>
        public virtual CustomerAvatarModel PrepareCustomerAvatarModel(CustomerAvatarModel model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.AvatarUrl = _pictureService.GetPictureUrl(
                _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.AvatarPictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                false);

            model.Avatar2Url = _pictureService.GetPictureUrl(
                _genericAttributeService.GetAttribute<int>(_workContext.CurrentCustomer, NopCustomerDefaults.Avatar2PictureIdAttribute),
                _mediaSettings.AvatarPictureSize,
                false);

            return model;
        }

        /// <summary>
        /// Prepare the GDPR tools model
        /// </summary>
        /// <returns>GDPR tools model</returns>
        public virtual GdprToolsModel PrepareGdprToolsModel()
        {
            var model = new GdprToolsModel();
            return model;
        }

        /// <summary>
        /// Prepare the check gift card balance madel
        /// </summary>
        /// <returns>Check gift card balance madel</returns>
        public virtual CheckGiftCardBalanceModel PrepareCheckGiftCardBalanceModel()
        {
            var model = new CheckGiftCardBalanceModel();
            return model;
        }



        public virtual CorporateCustomerRegistrationModel PrepareCorporateCustomerRegistrationModel(CorporateCustomerRegistrationModel model, bool excludeProperties,
            string overrideCustomCustomerAttributesXml = "", bool setDefaultValues = false)

        {

            if (model == null)
                throw new ArgumentNullException(nameof(model));

            model.Customer.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.Customer.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.Customer.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            model.Customer.DisplayVatNumber = _taxSettings.EuVatEnabled;
            //form fields
            model.Customer.FirstNameEnabled = _customerSettings.FirstNameEnabled;
            model.Customer.LastNameEnabled = _customerSettings.LastNameEnabled;
            model.Customer.FirstNameRequired = _customerSettings.FirstNameRequired;
            model.Customer.LastNameRequired = _customerSettings.LastNameRequired;
            model.Customer.GenderEnabled = _customerSettings.GenderEnabled;
            model.Customer.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.Customer.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.Customer.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.Customer.CompanyRequired = _customerSettings.CompanyRequired;
            model.Customer.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.Customer.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.Customer.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.Customer.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.Customer.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.Customer.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.Customer.CityEnabled = _customerSettings.CityEnabled;
            model.Customer.CityRequired = _customerSettings.CityRequired;
            model.Customer.CountyEnabled = _customerSettings.CountyEnabled;
            model.Customer.CountyRequired = _customerSettings.CountyRequired;
            model.Customer.CountryEnabled = _customerSettings.CountryEnabled;
            model.Customer.CountryRequired = _customerSettings.CountryRequired;
            model.Customer.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.Customer.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.Customer.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.Customer.PhoneRequired = _customerSettings.PhoneRequired;
            model.Customer.FaxEnabled = _customerSettings.FaxEnabled;
            model.Customer.FaxRequired = _customerSettings.FaxRequired;
            model.Customer.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.Customer.AcceptPrivacyPolicyEnabled = _customerSettings.AcceptPrivacyPolicyEnabled;
            model.Customer.AcceptPrivacyPolicyPopup = _commonSettings.PopupForTermsOfServiceLinks;
            model.Customer.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.Customer.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.Customer.HoneypotEnabled = _securitySettings.HoneypotEnabled;
            model.Customer.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnRegistrationPage;
            model.Customer.EnteringEmailTwice = _customerSettings.EnteringEmailTwice;
            if (setDefaultValues)
            {
                //enable newsletter by default
                model.Customer.Newsletter = _customerSettings.NewsletterTickedByDefault;
            }

            //countries and states         
            model.Customer.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });

            foreach (var c in _countryService.GetAllCountries(_workContext.WorkingLanguage.Id))
            {
                model.Customer.AvailableCountries.Add(new SelectListItem
                {
                    Text = _localizationService.GetLocalized(c, x => x.Name),
                    Value = c.Id.ToString(),
                    Selected = c.Id == model.Customer.CountryId
                });
            }

            if (_customerSettings.StateProvinceEnabled)
            {
                //states
                var states = _stateProvinceService.GetStateProvincesByCountryId(model.Customer.CountryId, _workContext.WorkingLanguage.Id).ToList();
                if (states.Any())
                {
                    model.Customer.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                    foreach (var s in states)
                    {
                        model.Customer.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetLocalized(s, x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.Customer.StateProvinceId) });
                    }
                }
                else
                {
                    var anyCountrySelected = model.Customer.AvailableCountries.Any(x => x.Selected);

                    model.Customer.AvailableStates.Add(new SelectListItem
                    {
                        Text = _localizationService.GetResource(anyCountrySelected ? "Address.Other" : "Address.SelectState"),
                        Value = "0"
                    });
                }

            }


            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(_workContext.CurrentCustomer, overrideCustomCustomerAttributesXml);
            foreach (var attribute in customAttributes)
                model.Customer.CustomerAttributes.Add(attribute);

            //GDPR
            if (_gdprSettings.GdprEnabled)
            {
                var consents = _gdprService.GetAllConsents().Where(consent => consent.DisplayDuringRegistration).ToList();
                foreach (var consent in consents)
                {
                    model.Customer.GdprConsents.Add(PrepareGdprConsentModel(consent, false));
                }
            }

            return model;
        }

        #endregion
    }
}