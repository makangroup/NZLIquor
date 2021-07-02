using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Models.Customer;

namespace Fenchurch.Web.Models.Customer
{
    public class CorporateCustomerRegistrationModel
    {

        public CorporateCustomerRegistrationModel()
        {
            Customer = new RegisterModel();
        }
        public RegisterModel Customer { get; set; }
        public CorporateCustomerModel CorporateCustomer { get; set; }
        public CorporateCustomerCompanyOwnersModel CorporateCustomerCompanyOwners { get; set; }
        public CorporateCustomerTradeReferencesModel CorporateCustomerTradeReferences { get; set; }
    }
}
