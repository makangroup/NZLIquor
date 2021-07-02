using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.CorporateCustomer
{
    /// <summary>
    /// Represent ProfitMarkup to store and retreive proft markp for corporate customers
    /// </summary>
    public class CorporateCustomerProfitMarkups : BaseEntity
    {

        /// <summary>
        /// Get or set the Corporate Customer Id
        /// </summary>
        public int? CorporateCustomerId { get; set; }

        /// <summary>
        /// Get or set the Category Id
        /// </summary>
        public int CategoryId { get; set; }
	
        /// <summary>
        /// Get or set the 
        /// </summary>
        public decimal ProfitMarkup { get; set; }

        /// <summary>
        /// Get or set the type of corporate customer.
        /// </summary>
        public int CorporateCustomerType { get; set; }
    }
}
