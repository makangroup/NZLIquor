using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Areas.Admin.Models.Catalog;
using Nop.Web.Areas.Admin.Models.Payments;
using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Fenchurch.Web.Areas.Admin.Models.CorporateCustomers
{
    public class ProfitMarkupsModel : BaseNopEntityModel
    {
        #region Constructores
        public ProfitMarkupsModel()
        {
            ProfitMarkups = new ProfitMarkupsSearchModel();
        }
        #endregion

        #region Properties

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
        /// Get or set the corporate customer type name.
        /// </summary>

        public string CorporateCustomerTypeName { get; set; }

        /// <summary>
        /// Get or set the Corporate Customer Types
        /// </summary>
        public IList<SelectListItem> CorporateCustomerTypes { get; set; }

        /// <summary>
        /// Get or set the Product Categories
        /// </summary>
        public IList<SelectListItem> ProductCategories { get; set; }


        /// <summary>
        /// Get or set the product type
        /// </summary>
        public string ProductCategorie { get; set; }

        /// <summary>
        /// Get or set the Profit Markups
        /// </summary>
        public ProfitMarkupsSearchModel ProfitMarkups { get; set; }
        #endregion
    }
}
