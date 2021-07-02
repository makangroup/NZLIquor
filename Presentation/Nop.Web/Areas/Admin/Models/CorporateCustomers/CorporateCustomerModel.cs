using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;
using Nop.Web.Areas.Admin.Models.Customers;
using System.ComponentModel;

namespace Fenchurch.Web.Areas.Admin.Models.CorporateCustomers
{
    /// <summary>
    /// Represents a corporate customer model
    /// </summary>
    public class CorporateCustomerModel : CustomerModel
    {
        public ProfitMarkupsModel ProfitMarkups { get; set; }

        public ProfitMarkupsSearchModel ProfitMarkupsSearchModel { get; set; }

        /// <summary>
        /// Get or set the type of corporate customer.
        /// </summary>
        [DisplayName("Corporate Customer Type")]
        public int CorporateCustomerType { get; set; }

        /// <summary>
        /// Get or set the Corporate Customer Types
        /// </summary>
        public IList<SelectListItem> CorporateCustomerTypes { get; set; }
        [Display(Name ="Liquor License File")]
        public string LiquorLicenseFileLink { get; set; }
        [Display(Name = "Credit Application Form File")]
        public string CreditApplicationFileLink { get; set; }

        /// <summary>
        /// Get or set the credit limit of the corporate customer.
        /// </summary>
        [Display(Name = "Credit Limit")]
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Get or set the Available Credit Limit of the corporate customer.
        /// </summary>
        [Display(Name = "Available Credit Limit")]
        public decimal AvailableCreditLimit { get; set; }

        /// <summary>
        /// Get or set the due amount of the corporate customer
        /// </summary>
        [Display(Name = "Amount Due")]
        public decimal AmountDue { get; set; }

        /// <summary>
        /// Get or set the credit period of the customer
        /// </summary>
        [Display(Name = "Credit Period")]
        public int CreditPeriod { get; set; }
    }
}