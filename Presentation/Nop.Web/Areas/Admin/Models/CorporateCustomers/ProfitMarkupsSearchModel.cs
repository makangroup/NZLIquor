using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Areas.Admin.Models.CorporateCustomers
{
    public class ProfitMarkupsSearchModel : BaseSearchModel
    {
        public int Id { get; set; }

        /// <summary>
        /// Get or set the Corporate Customer Id
        /// </summary>
        [NopResourceDisplayName("Admin.CorporateCustomer.CorporateCustomer")]
        public int? CorporateCustomerId { get; set; }

        /// <summary>
        /// Get or set the Category Id
        /// </summary>
        [NopResourceDisplayName("Admin.CorporateCustomer.ProductCategory")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Get or set the type of corporate customer.
        /// </summary>
        [NopResourceDisplayName("Admin.CorporateCustomer.CorporateCustomerType")]
        public int CorporateCustomerType { get; set; }

        /// <summary>
        /// Get or set the club proft rate
        /// </summary>
        [NopResourceDisplayName("Admin.CorporateCustomer.ProfitMarkup")]
        public decimal ProfitMarkup { get; set; }

        /// <summary>
        /// Get or set the Product Categories
        /// </summary>
        public IList<SelectListItem> ProductCategories { get; set; }
    }
}
