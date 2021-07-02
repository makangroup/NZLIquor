using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Core.Domain.CorporateCustomer
{
    public partial class CorporateCustomerCompanyOwners : BaseEntity
    {
        public int CorporateCustomer_Id { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string PrivateAddress { get; set; }
        public string Postcode { get; set; }
        public string DriversLicenceNo { get; set; }
        public string PhoneNo { get; set; }
        public string MobileNo { get; set; }
    }
}
