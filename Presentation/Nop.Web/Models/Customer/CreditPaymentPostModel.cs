using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Models.Customer
{
    public class CreditPaymentPostModel : BaseNopModel
    {
        public string FromDate { get; set; }
        public string ToDate { get; set; }
    }
}
