using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Credit.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.Credit.Components
{
    [ViewComponent(Name = "Credit")]
    public class CreditViewComponent : NopViewComponent
    {
        private readonly CreditPaymentSettings _checkMoneyOrderPaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public CreditViewComponent(CreditPaymentSettings checkMoneyOrderPaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _checkMoneyOrderPaymentSettings = checkMoneyOrderPaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel
            {
                DescriptionText = ""
            };

            return View("~/Plugins/Payments.Credit/Views/PaymentInfo.cshtml", model);
        }
    }
}