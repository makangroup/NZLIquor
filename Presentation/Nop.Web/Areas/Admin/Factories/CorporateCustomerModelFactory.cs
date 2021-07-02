using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Fenchurch.Web.Areas.Admin.Models.CorporateCustomers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Gdpr;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Services.Affiliates;
using Nop.Services.Authentication.External;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.CorporateCustomers;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Gdpr;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Infrastructure.Mapper.Extensions;
using Nop.Web.Areas.Admin.Models.Common;
using Nop.Web.Areas.Admin.Models.Customers;
using Nop.Web.Areas.Admin.Models.ShoppingCart;
using Nop.Web.Framework.Factories;
using Nop.Web.Framework.Models.Extensions;

namespace Fenchurch.Web.Areas.Admin.Factories
{
    public class CorporateCustomerModelFactory : ICorporateCustomerModelFactory
    {

        #region Fields

        private readonly AddressSettings _addressSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly GdprSettings _gdprSettings;
        private readonly ForumSettings _forumSettings;
        private readonly IAclSupportedModelFactory _aclSupportedModelFactory;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly IAddressAttributeModelFactory _addressAttributeModelFactory;
        private readonly IAffiliateService _affiliateService;
        private readonly IAuthenticationPluginManager _authenticationPluginManager;
        private readonly IBackInStockSubscriptionService _backInStockSubscriptionService;
        private readonly IBaseAdminModelFactory _baseAdminModelFactory;
        private readonly ICountryService _countryService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerService _customerService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IExternalAuthenticationService _externalAuthenticationService;
        private readonly IGdprService _gdprService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IGeoLookupService _geoLookupService;
        private readonly ILocalizationService _localizationService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IOrderService _orderService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IProductService _productService;
        private readonly IRewardPointService _rewardPointService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ITaxService _taxService;
        private readonly MediaSettings _mediaSettings;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly TaxSettings _taxSettings;
        private readonly ICorporateCustomerTypeService _corporateCustomerTypeService;
        private readonly ICorporateCustomerService _corporateCustomerService;
        private readonly ICategoryService _categoryService;
        #endregion

        #region Ctor

        public CorporateCustomerModelFactory(AddressSettings addressSettings,
            CustomerSettings customerSettings,
            DateTimeSettings dateTimeSettings,
            GdprSettings gdprSettings,
            ForumSettings forumSettings,
            IAclSupportedModelFactory aclSupportedModelFactory,
            IAddressAttributeFormatter addressAttributeFormatter,
            IAddressAttributeModelFactory addressAttributeModelFactory,
            IAffiliateService affiliateService,
            IAuthenticationPluginManager authenticationPluginManager,
            IBackInStockSubscriptionService backInStockSubscriptionService,
            IBaseAdminModelFactory baseAdminModelFactory,
            ICountryService countryService,
            ICustomerActivityService customerActivityService,
            ICustomerAttributeParser customerAttributeParser,
            ICustomerAttributeService customerAttributeService,
            ICustomerService customerService,
            IDateTimeHelper dateTimeHelper,
            IExternalAuthenticationService externalAuthenticationService,
            IGdprService gdprService,
            IGenericAttributeService genericAttributeService,
            IGeoLookupService geoLookupService,
            ILocalizationService localizationService,
            INewsLetterSubscriptionService newsLetterSubscriptionService,
            IOrderService orderService,
            IPictureService pictureService,
            IPriceFormatter priceFormatter,
            IProductAttributeFormatter productAttributeFormatter,
            IProductService productService,
            IRewardPointService rewardPointService,
            IShoppingCartService shoppingCartService,
            IStateProvinceService stateProvinceService,
            IStoreContext storeContext,
            IStoreService storeService,
            ITaxService taxService,
            MediaSettings mediaSettings,
            RewardPointsSettings rewardPointsSettings,
            TaxSettings taxSettings,
            ICorporateCustomerTypeService corporateCustomerTypeService,
            ICorporateCustomerService corporateCustomerService,
            ICategoryService categoryService)
        {
            _addressSettings = addressSettings;
            _customerSettings = customerSettings;
            _dateTimeSettings = dateTimeSettings;
            _gdprSettings = gdprSettings;
            _forumSettings = forumSettings;
            _aclSupportedModelFactory = aclSupportedModelFactory;
            _addressAttributeFormatter = addressAttributeFormatter;
            _addressAttributeModelFactory = addressAttributeModelFactory;
            _affiliateService = affiliateService;
            _authenticationPluginManager = authenticationPluginManager;
            _backInStockSubscriptionService = backInStockSubscriptionService;
            _baseAdminModelFactory = baseAdminModelFactory;
            _countryService = countryService;
            _customerActivityService = customerActivityService;
            _customerAttributeParser = customerAttributeParser;
            _customerAttributeService = customerAttributeService;
            _customerService = customerService;
            _dateTimeHelper = dateTimeHelper;
            _externalAuthenticationService = externalAuthenticationService;
            _gdprService = gdprService;
            _genericAttributeService = genericAttributeService;
            _geoLookupService = geoLookupService;
            _localizationService = localizationService;
            _newsLetterSubscriptionService = newsLetterSubscriptionService;
            _orderService = orderService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
            _productAttributeFormatter = productAttributeFormatter;
            _productService = productService;
            _rewardPointService = rewardPointService;
            _shoppingCartService = shoppingCartService;
            _stateProvinceService = stateProvinceService;
            _storeContext = storeContext;
            _storeService = storeService;
            _taxService = taxService;
            _mediaSettings = mediaSettings;
            _rewardPointsSettings = rewardPointsSettings;
            _taxSettings = taxSettings;
            _corporateCustomerTypeService = corporateCustomerTypeService;
            _corporateCustomerService = corporateCustomerService;
            _categoryService = categoryService;
        }

        #endregion

        public virtual CorporateCustomerModel PrepareCustomerModel(CorporateCustomerModel model)
        {
            if (model != null)
            {
                var copCust = _corporateCustomerService.GetCustomerById(model.Id);
                model.CorporateCustomerTypes = _corporateCustomerTypeService.GetAllCorporateCustomerTypes().Select(e => new SelectListItem(e.Name, e.Id.ToString())).ToList();
                model.CorporateCustomerType = copCust.CustomerType;
                model.CreditLimit = copCust.CreditLimit;
                model.CreditPeriod = copCust.CreditPeriod;
                model.AmountDue = _orderService.GetCustomerOverDueAmount(model.Id);
                model.AvailableCreditLimit = model.CreditLimit - model.AmountDue;

                model.ProfitMarkups = new ProfitMarkupsModel();
                model.ProfitMarkupsSearchModel = new ProfitMarkupsSearchModel();
                model.ProfitMarkups.ProductCategories = _categoryService.GetAllParentCategories().Select(e => new SelectListItem(e.Name, e.Id.ToString())).ToList();
                model.CorporateCustomerTypes = _corporateCustomerTypeService.GetAllCorporateCustomerTypes().Select(e => new SelectListItem(e.Name, e.Id.ToString())).ToList();                
                model.CreditApplicationFileLink = copCust.CreditFormFile;
                model.LiquorLicenseFileLink = copCust.LiquoreLicenseFile;
            }
            return model;
        }
    }
}
