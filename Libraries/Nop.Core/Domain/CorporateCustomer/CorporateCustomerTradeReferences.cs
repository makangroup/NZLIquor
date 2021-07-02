using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.CorporateCustomer
{
    public partial class CorporateCustomerTradeReferences : BaseEntity
    {
        public int CorporateCustomer_Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneFaxEmail { get; set; }
    }
}
