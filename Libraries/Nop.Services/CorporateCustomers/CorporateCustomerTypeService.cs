using Nop.Core.Caching;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Nop.Services.CorporateCustomers
{
    public class CorporateCustomerTypeService : ICorporateCustomerTypeService
    {
        private readonly CachingSettings _cachingSettings;
        private readonly IRepository<CorporateCustomerTypes> _corporateCustomerTypesRepository;

        public CorporateCustomerTypeService(
            CachingSettings cachingSettings, 
            IRepository<CorporateCustomerTypes> corporateCustomerTypesRepository)
        {
            _cachingSettings = cachingSettings;
            _corporateCustomerTypesRepository = corporateCustomerTypesRepository;
        }

        public CorporateCustomerTypes GetCorporateCustomerTypeById(int customerId)
        {
            if (customerId == 0)
                return null;

            return _corporateCustomerTypesRepository.ToCachedGetById(customerId, _cachingSettings.ShortTermCacheTime);
        }

        public IList<CorporateCustomerTypes> GetAllCorporateCustomerTypes()
        {
            var query = from o in _corporateCustomerTypesRepository.Table
                        orderby o.Id
                        select o;

            return query.ToList();
        }
    }
}
