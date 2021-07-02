using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Data;
using Nop.Services.Caching;
using Nop.Services.Caching.Extensions;
using Nop.Services.Common;
using Nop.Services.Events;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Linq;
using System.Text;


namespace Nop.Services.CorporateCustomers
{
    public partial class CorporateCustomerService : ICorporateCustomerService
    {

        private readonly CachingSettings _cachingSettings;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IRepository<CorporateCustomer> _corporatecustomerRepository;
        private readonly IRepository<CorporateCustomerCompanyOwners> _corporatecustomerownersRepository;
        private readonly IRepository<CorporateCustomerTradeReferences> _corporateCustomerTradeReferencesRepository;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;




        #region Ctor
        public CorporateCustomerService()
        {

        }
        public CorporateCustomerService(CachingSettings cachingSettings,
            ICacheKeyService cacheKeyService,
            IEventPublisher eventPublisher,
            IGenericAttributeService genericAttributeService,
            IRepository<CorporateCustomer> corporatecustomerRepository,
            IRepository<CorporateCustomerCompanyOwners> corporatecustomercompanyownersRepository,
            IRepository<CorporateCustomerTradeReferences> corporatecustomertradereferencesRepository,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext)
        {
            _cachingSettings = cachingSettings;
            _cacheKeyService = cacheKeyService;
            _eventPublisher = eventPublisher;
            _genericAttributeService = genericAttributeService;
            _corporatecustomerRepository = corporatecustomerRepository;
            _corporatecustomerownersRepository = corporatecustomercompanyownersRepository;
            _corporateCustomerTradeReferencesRepository = corporatecustomertradereferencesRepository;
            _storeContext = storeContext;
        }
        #endregion
        public virtual void DeleteCorporateCustomer(CorporateCustomer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            UpdateCorporateCustomer(customer);

            //event notification
            _eventPublisher.EntityDeleted(customer);
        }

        public virtual IPagedList<CorporateCustomer> GetAllCorporateCustomers(DateTime? createdFromUtc, DateTime? createdToUtc, int affiliateId, int vendorId, int[] customerRoleIds, string email, string username, string firstName, string lastName, int dayOfBirth, int monthOfBirth, string company, string phone, string zipPostalCode, string ipAddress, int pageIndex, int pageSize, bool getOnlyTotalCount)
        {
            throw new NotImplementedException();
        }
        public virtual CorporateCustomer GetCustomerByCorporateCustomerId(int CorporatecustomerId)
        {
            if (CorporatecustomerId == 0)
                return null;

            var query = _corporatecustomerRepository.Table.Where(e => e.Id == CorporatecustomerId);

            return query.FirstOrDefault();
        }
        public virtual CorporateCustomer GetCustomerById(int customerId)
        {
            if (customerId == 0)
                return null;

            var query = _corporatecustomerRepository.Table.Where(e => e.Customer_Id == customerId);

            return query.FirstOrDefault();
        }

        public virtual void InsertCorporateCustomer(CorporateCustomer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _corporatecustomerRepository.Insert(customer);

            //event notification
            _eventPublisher.EntityInserted(customer);
        }

        public virtual void InsertCorporateCustomerOwners(CorporateCustomerCompanyOwners customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _corporatecustomerownersRepository.Insert(customer);

            //event notification
            _eventPublisher.EntityInserted(customer);
        }

        public virtual void InsertCorporateCustomerReferences(CorporateCustomerTradeReferences customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _corporateCustomerTradeReferencesRepository.Insert(customer);

            //event notification
            _eventPublisher.EntityInserted(customer);
        }

        public virtual void UpdateCorporateCustomer(CorporateCustomer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            _corporatecustomerRepository.Update(customer);

            //event notification
            _eventPublisher.EntityUpdated(customer);
        }


        public List<CorporateCustomerTradeReferences> GetCompanyReferencesById(int corporateCustomerId)
        {
            if (corporateCustomerId == 0)
                return new List<CorporateCustomerTradeReferences>();

            var query = from c in _corporateCustomerTradeReferencesRepository.Table
                        where c.CorporateCustomer_Id == corporateCustomerId
                        select c;
            var customerTradeReferences = query.ToList();

            return customerTradeReferences;
        }

        public List<CorporateCustomerCompanyOwners> GetCompanyOwnersById(int corporateCustomerId)
        {
            if (corporateCustomerId == 0)
                return new List<CorporateCustomerCompanyOwners>();

            var query = from c in _corporatecustomerownersRepository.Table
                        where c.CorporateCustomer_Id == corporateCustomerId
                        select c;
            var customercustomerOwners = query.ToList();

            return customercustomerOwners;
        }
    }
}
