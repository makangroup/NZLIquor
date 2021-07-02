using Microsoft.AspNetCore.Mvc;
using Nop.Services.Customers;
using Nop.Web.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Controllers
{
    [AutoValidateAntiforgeryToken]
    public partial class CorporateCustomerController : BasePublicController
    {
        #region Member Fields
        private readonly ICustomerService _customerService;
        #endregion

        public CorporateCustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        public IActionResult Payments()
        {
            return View();
        }
    }
}
