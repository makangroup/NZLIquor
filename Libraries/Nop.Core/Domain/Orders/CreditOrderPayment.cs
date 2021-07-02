using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.Orders
{
    /// <summary>
    /// This class is used to store are retrieve credit orders for the payment
    /// </summary>
    public class CreditOrderPayment : BaseEntity
    {
        /// <summary>
        /// This property is used to get and set the order Ids.
        /// </summary>
        public string OrderIds { get; set; }


        /// <summary>
        /// Get or set the created date of the credit order payment.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Get or set the credit order payment amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Get or set the Windcave file name.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Get or set the last update date and time of the record.
        /// </summary>
        public DateTime LastUpdatedDate { get; set; }
    }
}
