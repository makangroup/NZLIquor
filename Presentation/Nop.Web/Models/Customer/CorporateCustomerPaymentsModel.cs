using Nop.Web.Framework.Models;
using System;
using System.Collections.Generic;

namespace Fenchurch.Web.Models.Customer
{
    public class CorporateCustomerPaymentsModel : BaseSearchModel
    {
        /// <summary>
        /// Get or set the Corporate Customer Credit Limit
        /// </summary>
        public decimal CreditLimit { get; set; }

        /// <summary>
        /// Get or set the Corporate Customer Amount Due
        /// </summary>
        public decimal AmountDue { get; set; }

        /// <summary>
        /// Get or set the Corporate Customer Available Limit
        /// </summary>
        public decimal AvailableLimit { get; set; }
        /// <summary>
        /// Get or Set Corporate Customer From Date
        /// </summary>
        public DateTime FromDate { get; set; }
        /// <summary>
        /// Get or Set Corporate Customer To Date
        /// </summary>
        public DateTime ToDate { get; set; }

        public string SelectedCreditPayments { get; set; }
    }
}
