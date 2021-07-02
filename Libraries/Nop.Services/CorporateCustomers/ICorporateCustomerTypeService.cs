using Nop.Core.Domain.CorporateCustomer;
using System.Collections.Generic;

namespace Nop.Services.CorporateCustomers
{
    public interface ICorporateCustomerTypeService
    {
        public CorporateCustomerTypes GetCorporateCustomerTypeById(int id);

        public IList<CorporateCustomerTypes> GetAllCorporateCustomerTypes();
    }
}
