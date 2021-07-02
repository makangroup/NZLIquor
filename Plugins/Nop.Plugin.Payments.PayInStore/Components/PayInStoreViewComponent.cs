using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.PayInStore.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Payments.PayInStore.Components
{
    [ViewComponent(Name = "PayInStore")]
    public class PayInStoreViewComponent : NopViewComponent
    {
        private readonly PayInStorePaymentSettings _payInStorePaymentSettings;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;

        public PayInStoreViewComponent(PayInStorePaymentSettings payInStorePaymentSettings,
            ILocalizationService localizationService,
            IStoreContext storeContext,
            IWorkContext workContext)
        {
            _payInStorePaymentSettings = payInStorePaymentSettings;
            _localizationService = localizationService;
            _storeContext = storeContext;
            _workContext = workContext;
        }

        public IViewComponentResult Invoke()
        {
            var model = new PaymentInfoModel
            {
                DescriptionText = _localizationService.GetLocalizedSetting(_payInStorePaymentSettings,
                    x => x.DescriptionText, _workContext.WorkingLanguage.Id, _storeContext.CurrentStore.Id)
            };

            return View("~/Plugins/Payments.PayInStore/Views/PaymentInfo.cshtml", model);
        }
    }
}