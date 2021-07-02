using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Areas.Admin.Models.Orders
{
    public class OverDueOrderModel : BaseNopEntityModel
    {
        [DisplayName("Order #")]
        public string CustomOrderNumber { get; set; }

        [DisplayName("Order #")]
        public int DPD { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Contact Person Name")]
        public string ContactPersonName { get; set; }

        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("Phone #")]
        public string PhoneNo { get; set; }

        [DisplayName("Create On")]
        public DateTime? CreateOn { get; set; }

        [DisplayName("Order Total")]
        public string OrderTotal { get; set; }
        
    }
}
