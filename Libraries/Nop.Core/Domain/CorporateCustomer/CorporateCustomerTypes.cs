using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.CorporateCustomer
{
    /// <summary>
    /// This classe represetns the CorporateCustomerTypes
    /// </summary>
    public class CorporateCustomerTypes : BaseEntity
    {
        /// <summary>
        /// Get or set the Corporate Customer Type Name
        /// </summary>
        public string Name { get; set; }
    }
}
