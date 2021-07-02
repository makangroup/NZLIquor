using Nop.Core.Domain.CorporateCustomer;
using System;
using Nop.Core;
using System.Collections.Generic;

namespace Nop.Services.CorporateCustomers
{
    public partial interface ICorporateCustomerService
    {

        IPagedList<CorporateCustomer> GetAllCorporateCustomers(DateTime? createdFromUtc = null, DateTime? createdToUtc = null,
            int affiliateId = 0, int vendorId = 0, int[] customerRoleIds = null,
            string email = null, string username = null, string firstName = null, string lastName = null,
            int dayOfBirth = 0, int monthOfBirth = 0,
            string company = null, string phone = null, string zipPostalCode = null, string ipAddress = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        void DeleteCorporateCustomer(CorporateCustomer customer);

        CorporateCustomer GetCustomerByCorporateCustomerId(int CorporatecustomerId);
        CorporateCustomer GetCustomerById(int customerId);

        void InsertCorporateCustomer(CorporateCustomer customer);

        void InsertCorporateCustomerOwners(CorporateCustomerCompanyOwners customer);

        void InsertCorporateCustomerReferences(CorporateCustomerTradeReferences customer);

        void UpdateCorporateCustomer(CorporateCustomer customer);

        List<CorporateCustomerTradeReferences> GetCompanyReferencesById(int corporateCustomerId);
        List<CorporateCustomerCompanyOwners> GetCompanyOwnersById(int corporateCustomerId);
    }
}
