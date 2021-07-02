using Fenchurch.Web.Areas.Admin.Factories;
using Fenchurch.Web.Areas.Admin.Models.CorporateCustomers;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Services.CorporateCustomers;
using Nop.Services.Customers;
using Nop.Services.Security;
using Nop.Web.Areas.Admin.Controllers;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Core;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Web.Framework.Controllers;
using System.Collections.Generic;
using Nop.Core.Domain.Customers;
using Nop.Services.Messages;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Microsoft.AspNetCore.Http;
using System;
using Nop.Core.Domain.Tax;
using Nop.Services.Common;
using Nop.Services.Helpers;
using Nop.Services.Stores;
using Microsoft.Extensions.Primitives;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Messages;
using Nop.Services.Tax;
using Nop.Services.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace Fenchurch.Web.Areas.Admin.Controllers
{
    public partial class CorporateCustomerController : BaseAdminController
    {
        #region Fields
        private readonly ICustomerModelFactory _customerModelFactory;
        private readonly IProfitMarkupModelFactory _profitMarkupModelFactory;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerService _customerService;
        private readonly IProfitMarkupsService _profitMarkupsService;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly CustomerSettings _customerSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly IStoreService _storeService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkContext _workContext;
        private readonly ITaxService _taxService;
        private readonly ICategoryService _categoryService;
        private readonly ICorporateCustomerModelFactory _corporateCustomerModelFactory;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly IWebHostEnvironment _HostEnvironment;
        #endregion

        public CorporateCustomerController(
            ICustomerModelFactory customerModelFactory,
            IProfitMarkupModelFactory profitMarkupModelFactory,
            IPermissionService permissionService,
            ICustomerRegistrationService customerRegistrationService,
            ICustomerService customerService,
            IProfitMarkupsService profitMarkupsService,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            CustomerSettings customerSettings,
            TaxSettings taxSettings,
            IGenericAttributeService genericAttributeService,
            DateTimeSettings dateTimeSettings,
            IStoreService storeService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IWorkContext workContext,
            ITaxService taxService,
            ICategoryService categoryService,
            ICorporateCustomerModelFactory corporateCustomerModelFactory,
            ICorporateCustomerService corporateCustomerService,
            IWebHostEnvironment HostEnvironment)
        {
            _customerModelFactory = customerModelFactory;
            _profitMarkupModelFactory = profitMarkupModelFactory;
            _permissionService = permissionService;
            _customerRegistrationService = customerRegistrationService;
            _customerService = customerService;
            _profitMarkupsService = profitMarkupsService;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _customerActivityService = customerActivityService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerSettings = customerSettings;
            _taxSettings = taxSettings;
            _genericAttributeService = genericAttributeService;
            _dateTimeSettings = dateTimeSettings;
            _storeService = storeService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _workContext = workContext;
            _taxService = taxService;
            _categoryService = categoryService;
            _corporateCustomerModelFactory = corporateCustomerModelFactory;
            _corporateCustomerService = corporateCustomerService;
            _HostEnvironment = HostEnvironment;
        }

        #region Methods

        public virtual IActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //prepare model
            var model = _customerModelFactory.PrepareCustomerSearchModel(new CustomerSearchModel());

            return View(model);
        }

        [CheckAccessPublicStore(true)]
        public virtual IActionResult FileDownload(string filename, bool IsLiquorFile)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            string webrootpath = _HostEnvironment.WebRootPath;
            string root = webrootpath + "\\CorporateCustomers";
            string subdirLiqorLicense = root + "\\LiqorLicense\\";
            string subdirCreditForms = root + "\\CreditForms\\";
            string filePath = string.Empty;
            if (!IsLiquorFile)
            {
                filePath = Path.Combine(subdirCreditForms, filename);
            }
            else
            {
                filePath = Path.Combine(subdirLiqorLicense, filename);
            }
            

            return PhysicalFile(filePath, "application/" + Path.GetExtension(filePath).Replace(".", ""), Path.GetFileName(filePath));
        }
        [HttpPost]
        public virtual IActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return RedirectToAction("List");

            try
            {
                //prevent attempts to delete the user, if it is the last active administrator
                if (_customerService.IsAdmin(customer) && !SecondAdminAccountExists(customer))
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.DeleteAdministrator"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //ensure that the current customer cannot delete "Administrators" if he's not an admin himself
                if (_customerService.IsAdmin(customer) && !_customerService.IsAdmin(_workContext.CurrentCustomer))
                {
                    _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.OnlyAdminCanDeleteAdmin"));
                    return RedirectToAction("Edit", new { id = customer.Id });
                }

                //delete
                _customerService.DeleteCustomer(customer);

                //remove newsletter subscription (if exists)
                foreach (var store in _storeService.GetAllStores())
                {
                    var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                    if (subscription != null)
                        _newsLetterSubscriptionService.DeleteNewsLetterSubscription(subscription);
                }

                //activity log
                _customerActivityService.InsertActivity("DeleteCustomer",
                    string.Format(_localizationService.GetResource("ActivityLog.DeleteCustomer"), customer.Id), customer);

                _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Deleted"));

                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                _notificationService.ErrorNotification(exc.Message);
                return RedirectToAction("Edit", new { id = customer.Id });
            }
        }
        public virtual IActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null || customer.Deleted)
                return RedirectToAction("List");

            //prepare model
            CorporateCustomerModel model = new CorporateCustomerModel();
            model = _customerModelFactory.PrepareCustomerModel(model, customer) as CorporateCustomerModel;
            model = _corporateCustomerModelFactory.PrepareCustomerModel(model);
            model.ProfitMarkups = _profitMarkupModelFactory.PrepareProfitMarkupModel(new ProfitMarkupsModel());
            model.ProfitMarkupsSearchModel = new ProfitMarkupsSearchModel();
            model.ProfitMarkups.CorporateCustomerId = id;
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("impersonate")]
        public virtual IActionResult Impersonate(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.AllowCustomerImpersonation))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(id);
            if (customer == null)
                return RedirectToAction("List");

            if (!customer.Active)
            {
                _notificationService.WarningNotification(
                    _localizationService.GetResource("Admin.Customers.Customers.Impersonate.Inactive"));
                return RedirectToAction("Edit", customer.Id);
            }

            //ensure that a non-admin user cannot impersonate as an administrator
            //otherwise, that user can simply impersonate as an administrator and gain additional administrative privileges
            if (!_customerService.IsAdmin(_workContext.CurrentCustomer) && _customerService.IsAdmin(customer))
            {
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.NonAdminNotImpersonateAsAdminError"));
                return RedirectToAction("Edit", customer.Id);
            }

            //activity log
            _customerActivityService.InsertActivity("Impersonation.Started",
                string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Started.StoreOwner"), customer.Email, customer.Id), customer);
            _customerActivityService.InsertActivity(customer, "Impersonation.Started",
                string.Format(_localizationService.GetResource("ActivityLog.Impersonation.Started.Customer"), _workContext.CurrentCustomer.Email, _workContext.CurrentCustomer.Id), _workContext.CurrentCustomer);

            //ensure login is not required
            customer.RequireReLogin = false;
            _customerService.UpdateCustomer(customer);
            _genericAttributeService.SaveAttribute<int?>(_workContext.CurrentCustomer, NopCustomerDefaults.ImpersonatedCustomerIdAttribute, customer.Id);

            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public virtual IActionResult Edit(CorporateCustomerModel model, bool continueEditing, IFormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            //try to get a customer with the specified id
            var customer = _customerService.GetCustomerById(model.Id);
            var copCustomer = _corporateCustomerService.GetCustomerById(model.Id);
            if (customer == null || customer.Deleted)
                return RedirectToAction("List");

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();
            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            var customerRolesError = ValidateCustomerRoles(newCustomerRoles, _customerService.GetCustomerRoles(customer));

            if (!string.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError(string.Empty, customerRolesError);
                _notificationService.ErrorNotification(customerRolesError);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null &&
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
            }

            //custom customer attributes
            var customerAttributesXml = ParseCustomCustomerAttributes(form);
            if (newCustomerRoles.Any() && newCustomerRoles.FirstOrDefault(c => c.SystemName == NopCustomerDefaults.RegisteredRoleName) != null)
            {
                var customerAttributeWarnings = _customerAttributeParser.GetAttributeWarnings(customerAttributesXml);
                foreach (var error in customerAttributeWarnings)
                {
                    ModelState.AddModelError(string.Empty, error);
                }
            }

            //Validate the credit limit
            if (model.CreditLimit <= 0)
            {
                ModelState.AddModelError(string.Empty, _localizationService.GetResource("Admin.CorporateCustomers.CorporateCustomers.ValidCreditLimitRequiredRegisteredRole"));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    customer.AdminComment = model.AdminComment;
                    customer.IsTaxExempt = model.IsTaxExempt;

                    //prevent deactivation of the last active administrator
                    if (!_customerService.IsAdmin(customer) || model.Active || SecondAdminAccountExists(customer))
                        customer.Active = model.Active;
                    else
                        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.Deactivate"));

                    //email
                    if (!string.IsNullOrWhiteSpace(model.Email))
                        _customerRegistrationService.SetEmail(customer, model.Email, false);
                    else
                        customer.Email = model.Email;

                    //username
                    if (_customerSettings.UsernamesEnabled)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Username))
                            _customerRegistrationService.SetUsername(customer, model.Username);
                        else
                            customer.Username = model.Username;
                    }

                    //VAT number
                    if (_taxSettings.EuVatEnabled)
                    {
                        var prevVatNumber = _genericAttributeService.GetAttribute<string>(customer, NopCustomerDefaults.VatNumberAttribute);

                        _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.VatNumberAttribute, model.VatNumber);
                        //set VAT number status
                        if (!string.IsNullOrEmpty(model.VatNumber))
                        {
                            if (!model.VatNumber.Equals(prevVatNumber, StringComparison.InvariantCultureIgnoreCase))
                            {
                                _genericAttributeService.SaveAttribute(customer,
                                    NopCustomerDefaults.VatNumberStatusIdAttribute,
                                    (int)_taxService.GetVatNumberStatus(model.VatNumber));
                            }
                        }
                        else
                        {
                            _genericAttributeService.SaveAttribute(customer,
                                NopCustomerDefaults.VatNumberStatusIdAttribute,
                                (int)VatNumberStatus.Empty);
                        }
                    }

                    //vendor
                    customer.VendorId = model.VendorId;

                    //form fields
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CompanyAttribute, model.Company);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddressAttribute, model.StreetAddress);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StreetAddress2Attribute, model.StreetAddress2);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.ZipPostalCodeAttribute, model.ZipPostalCode);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CityAttribute, model.City);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountyAttribute, model.County);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CountryIdAttribute, model.CountryId);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.StateProvinceIdAttribute, model.StateProvinceId);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.PhoneAttribute, model.Phone);
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.FaxAttribute, model.Fax);

                    //custom customer attributes
                    _genericAttributeService.SaveAttribute(customer, NopCustomerDefaults.CustomCustomerAttributes, customerAttributesXml);

                    //newsletter subscriptions
                    if (!string.IsNullOrEmpty(customer.Email))
                    {
                        var allStores = _storeService.GetAllStores();
                        foreach (var store in allStores)
                        {
                            var newsletterSubscription = _newsLetterSubscriptionService
                                .GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, store.Id);
                            if (model.SelectedNewsletterSubscriptionStoreIds != null &&
                                model.SelectedNewsletterSubscriptionStoreIds.Contains(store.Id))
                            {
                                //subscribed
                                if (newsletterSubscription == null)
                                {
                                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(new NewsLetterSubscription
                                    {
                                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                                        Email = customer.Email,
                                        Active = true,
                                        StoreId = store.Id,
                                        CreatedOnUtc = DateTime.UtcNow
                                    });
                                }
                            }
                            else
                            {
                                //not subscribed
                                if (newsletterSubscription != null)
                                {
                                    _newsLetterSubscriptionService.DeleteNewsLetterSubscription(newsletterSubscription);
                                }
                            }
                        }
                    }

                    var currentCustomerRoleIds = _customerService.GetCustomerRoleIds(customer, true);

                    //customer roles
                    foreach (var customerRole in allCustomerRoles)
                    {
                        //ensure that the current customer cannot add/remove to/from "Administrators" system role
                        //if he's not an admin himself
                        if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName &&
                            !_customerService.IsAdmin(_workContext.CurrentCustomer))
                            continue;

                        if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                        {
                            //new role
                            if (currentCustomerRoleIds.All(roleId => roleId != customerRole.Id))
                                _customerService.AddCustomerRoleMapping(new CustomerCustomerRoleMapping { CustomerId = customer.Id, CustomerRoleId = customerRole.Id });
                        }
                        else
                        {
                            //prevent attempts to delete the administrator role from the user, if the user is the last active administrator
                            if (customerRole.SystemName == NopCustomerDefaults.AdministratorsRoleName && !SecondAdminAccountExists(customer))
                            {
                                _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminAccountShouldExists.DeleteRole"));
                                continue;
                            }

                            //remove role
                            if (currentCustomerRoleIds.Any(roleId => roleId == customerRole.Id))
                                _customerService.RemoveCustomerRoleMapping(customer, customerRole);
                        }
                    }

                    _customerService.UpdateCustomer(customer);

                    //Save corporate customer
                    copCustomer.CustomerType = model.CorporateCustomerType;
                    copCustomer.CreditLimit = model.CreditLimit;
                    copCustomer.CreditPeriod = model.CreditPeriod;
                    _corporateCustomerService.UpdateCorporateCustomer(copCustomer);

                    //ensure that a customer with a vendor associated is not in "Administrators" role
                    //otherwise, he won't have access to the other functionality in admin area
                    if (_customerService.IsAdmin(customer) && customer.VendorId > 0)
                    {
                        customer.VendorId = 0;
                        _customerService.UpdateCustomer(customer);
                        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminCouldNotbeVendor"));
                    }

                    //ensure that a customer in the Vendors role has a vendor account associated.
                    //otherwise, he will have access to ALL products
                    if (_customerService.IsVendor(customer) && customer.VendorId == 0)
                    {
                        var vendorRole = _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.VendorsRoleName);
                        _customerService.RemoveCustomerRoleMapping(customer, vendorRole);

                        _notificationService.ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
                    }

                    //activity log
                    _customerActivityService.InsertActivity("EditCustomer",
                        string.Format(_localizationService.GetResource("ActivityLog.EditCustomer"), customer.Id), customer);

                    _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Customers.Customers.Updated"));

                    if (!continueEditing)
                        return RedirectToAction("List");

                    return RedirectToAction("Edit", new { id = customer.Id });
                }
                catch (Exception exc)
                {
                    _notificationService.ErrorNotification(exc.Message);
                }
            }

            //prepare model
            model = _customerModelFactory.PrepareCustomerModel(model, customer, true) as CorporateCustomerModel;
            model = _corporateCustomerModelFactory.PrepareCustomerModel(model);

            //if we got this far, something failed, redisplay form
            return View(model);
        }

        private bool SecondAdminAccountExists(Customer customer)
        {
            var customers = _customerService.GetAllCustomers(customerRoleIds: new[] { _customerService.GetCustomerRoleBySystemName(NopCustomerDefaults.AdministratorsRoleName).Id });

            return customers.Any(c => c.Active && c.Id != customer.Id);
        }

        protected virtual string ParseCustomCustomerAttributes(IFormCollection form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            var attributesXml = string.Empty;
            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var controlId = $"{NopCustomerServicesDefaults.CustomerAttributePrefix}{attribute.Id}";
                StringValues ctrlAttributes;

                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var selectedAttributeId = int.Parse(ctrlAttributes);
                            if (selectedAttributeId > 0)
                                attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                        }

                        break;
                    case AttributeControlType.Checkboxes:
                        var cblAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(cblAttributes))
                        {
                            foreach (var item in cblAttributes.ToString()
                                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                var selectedAttributeId = int.Parse(item);
                                if (selectedAttributeId > 0)
                                    attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }

                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
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

                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        ctrlAttributes = form[controlId];
                        if (!StringValues.IsNullOrEmpty(ctrlAttributes))
                        {
                            var enteredText = ctrlAttributes.ToString().Trim();
                            attributesXml = _customerAttributeParser.AddCustomerAttribute(attributesXml,
                                attribute, enteredText);
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

        protected virtual string ValidateCustomerRoles(IList<CustomerRole> customerRoles, IList<CustomerRole> existingCustomerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException(nameof(customerRoles));

            if (existingCustomerRoles == null)
                throw new ArgumentNullException(nameof(existingCustomerRoles));

            //check ACL permission to manage customer roles
            var rolesToAdd = customerRoles.Except(existingCustomerRoles);
            var rolesToDelete = existingCustomerRoles.Except(customerRoles);
            if (rolesToAdd.Any(role => role.SystemName != NopCustomerDefaults.RegisteredRoleName) || rolesToDelete.Any())
            {
                if (!_permissionService.Authorize(StandardPermissionProvider.ManageAcl))
                    return _localizationService.GetResource("Admin.Customers.Customers.CustomerRolesManagingError");
            }

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            var isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.GuestsRoleName) != null;
            var isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == NopCustomerDefaults.RegisteredRoleName) != null;
            if (isInGuestsRole && isInRegisteredRole)
                return _localizationService.GetResource("Admin.Customers.Customers.GuestsAndRegisteredRolesError");
            if (!isInGuestsRole && !isInRegisteredRole)
                return _localizationService.GetResource("Admin.Customers.Customers.AddCustomerToGuestsOrRegisteredRoleError");

            //no errors
            return string.Empty;
        }

        [HttpPost]
        public virtual IActionResult CustomerList(CustomerSearchModel searchModel)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _customerModelFactory.PrepareCorporateCustomerListModel(searchModel);

            return Json(model);
        }

        public virtual IActionResult ProfitMarkups()
        {
            var model = new ProfitMarkupsModel();
            return View(model);
        }

        [HttpPost]
        public virtual IActionResult ProfitMarkupsList(ProfitMarkupsSearchModel profitMarkupsSearch)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedDataTablesJson();

            //prepare model
            var model = _profitMarkupModelFactory.PrepareProfitMarkupListModelListModel(profitMarkupsSearch);

            return Json(model);
        }

        public IActionResult CreateProfitMarkup(int CorporateCustomerId)
        {
            //Authenticate and authorized the user
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = _profitMarkupModelFactory.PrepareProfitMarkupModel(new ProfitMarkupsModel());
            model.CorporateCustomerId = CorporateCustomerId;
            return View(model);
        }

        /// <summary>
        /// Create profit markup action.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateProfitMarkup(ProfitMarkupsModel model)
        {
            //Authorized the user
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                //Verify whether the the profit markup is already existing.
                var profitMarkupModel = model.ToEntity<CorporateCustomerProfitMarkups>();
                var profitMarkup = _profitMarkupsService.Find(profitMarkupModel);
                if (profitMarkup == null)
                {
                    profitMarkupModel.CorporateCustomerId = null;
                    //Save information on the database                    
                    _profitMarkupsService.InsertProfitMarkups(profitMarkupModel);
                    //if the profit markup creation is from Corporate Customer, then forward it to the eidt page.
                    if (model.CorporateCustomerId > 0)
                        return RedirectToAction("Edit", new { id = model.CorporateCustomerId });

                    //else forward the request to the profit markups list
                    return RedirectToAction("ProfitMarkups");
                }
                else
                {
                    ModelState.AddModelError("", "The profit markup is already existing.");
                }
            }
            model = _profitMarkupModelFactory.PrepareProfitMarkupModel(model);

            return View(model);
        }

        [HttpPost]
        public JsonResult CreateProfitMarkupFromCorporateCustomer(ProfitMarkupsModel model)
        {
            //Authorized the user
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            var profitMarkupModel = model.ToEntity<CorporateCustomerProfitMarkups>();
            var profitMarkup = _profitMarkupsService.Find(profitMarkupModel);
            if (profitMarkup != null)
                ModelState.AddModelError("", "The profit markup is already existing.");

            if (ModelState.IsValid)
                _profitMarkupsService.InsertProfitMarkups(profitMarkupModel);
            else
                return ErrorJson(ModelState.SerializeErrors());

            return Json(new { Result = true });
        }

        [HttpPost]
        public IActionResult ProfitsMarkupUpdate(ProfitMarkupsModel model)
        {
            //Authorized the user
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            var profitMarkup = _profitMarkupsService.Find(model.Id);
            profitMarkup.ProfitMarkup = model.ProfitMarkup;
            _profitMarkupsService.Update(profitMarkup);

            return new NullJsonResult();
        }

        public JsonResult ProfitsMarkupUpdateFromCorporateCustomer(ProfitMarkupsModel model)
        {
            string response = string.Empty;

            return Json(response);
        }

        public IActionResult ProfitsMarkupDelete(int id)
        {
            //Authorized the user
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCustomers))
                return AccessDeniedDataTablesJson();

            var profitMarkup = _profitMarkupsService.Find(id);
            _profitMarkupsService.Delete(profitMarkup);

            return new NullJsonResult();
        }

        #endregion
    }
}
