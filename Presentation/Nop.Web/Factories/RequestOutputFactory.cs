using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Windcave;
using Nop.Services.Configuration;
using Nop.Services.Orders;
using Nop.Services.Windcave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Factories
{
    public class RequestOutputFactory : IRequestOutputFactory
    {
        protected readonly IWindcavePaymentService _paymentService;
        protected readonly IOrderService _orderService;
        protected readonly ICreditOrderPaymentService _creditOrderPaymentService;
        protected readonly ISettingService _settingService;
        protected readonly IStoreContext _storeContext;

        public RequestOutputFactory(IWindcavePaymentService paymentService,
                                    IOrderService orderService,
                                    ICreditOrderPaymentService creditOrderPaymentService,
                                    ISettingService settingService,
                                    IStoreContext storeContext)
        {
            _orderService = orderService;
            _paymentService = paymentService;
            _creditOrderPaymentService = creditOrderPaymentService;
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public RequestOutput GenerateRequestOutput(Customer customer, string ids)
        {
            if (_storeContext.CurrentStore == null || string.IsNullOrEmpty(_storeContext.CurrentStore.Url))
                throw new ArgumentNullException("CurrentStore is null or CurrentStore.Url not available");

            //Get the credit payment amount
            var orderids = JsonConvert.DeserializeObject<int[]>(ids);
            var amount = _orderService.GetCreditOrdersTotal(customer, orderids);
            //save the data on DB
            var creditOrderPayment = new CreditOrderPayment()
            {
                Amount = amount,
                OrderIds = ids,
                CreatedDate = DateTime.Now,
                LastUpdatedDate = DateTime.Now
            };
            _creditOrderPaymentService.InsertCreditOrderPayment(creditOrderPayment);          
            
            RequestInput input = new RequestInput();
            input.CurrencyInput = "NZD";
            input.MerchantReference = creditOrderPayment.Id.ToString();
            input.TxnType = "Purchase";
            input.UrlFail = string.Concat(_storeContext.CurrentStore.Url, "/Customer/WindCaveResponse");
            input.UrlSuccess = string.Concat(_storeContext.CurrentStore.Url, "/Customer/WindCaveResponse");
            input.AmountInput = _orderService.GetCreditOrdersTotal(customer, orderids).ToString("0.00");

            return _paymentService.GenerateRequest(input);
        }
    }
}
