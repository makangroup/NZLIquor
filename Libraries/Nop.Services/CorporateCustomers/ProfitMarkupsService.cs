using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Data;
using Nop.Services.Caching.Extensions;
using Nop.Services.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Nop.Services.CorporateCustomers
{
    public class ProfitMarkupsService : IProfitMarkupsService
    {
        #region Member Field
        public readonly IRepository<CorporateCustomerProfitMarkups> _proftMarkups;
        public readonly ICategoryService _categoryService;
        public readonly ICorporateCustomerService _corporateCustomerService;
        protected readonly IRepository<Product> _productRepository;
        //public readonly IProductService _productService;
        #endregion

        #region Constructores
        public ProfitMarkupsService(IRepository<CorporateCustomerProfitMarkups> profitMarkups,
            ICategoryService categoryService,
            ICorporateCustomerService corporateCustomerService,
            IRepository<Product> productRepository)
        {
            _proftMarkups = profitMarkups;
            _categoryService = categoryService;
            _corporateCustomerService = corporateCustomerService;
            _productRepository = productRepository;
        }
        #endregion

        public IPagedList<CorporateCustomerProfitMarkups> GetAllProfitMarkups(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _proftMarkups.Table;
            query = query.Where(c => c.CorporateCustomerId == null);
            query = query.OrderBy(c => c.CorporateCustomerType);
            var customers = new PagedList<CorporateCustomerProfitMarkups>(query, pageIndex, pageSize, getOnlyTotalCount);

            return customers;
        }

        public IPagedList<CorporateCustomerProfitMarkups> GetAllProfitMarkupsByCorporateCustomerId(int customerId, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false)
        {
            var query = _proftMarkups.Table;
            query = query.Where(c => c.CorporateCustomerId == customerId);
            query = query.OrderBy(c => c.CorporateCustomerType);
            var customers = new PagedList<CorporateCustomerProfitMarkups>(query, pageIndex, pageSize, getOnlyTotalCount);

            return customers;
        }

        /// <summary>
        /// Find the CorporateCustomerProfitMarkups
        /// </summary>
        /// <param name="corporateCustomerType">The corporate customer type id</param>
        /// <param name="categoryId">The category id</param>
        /// <returns></returns>
        public CorporateCustomerProfitMarkups Find(CorporateCustomerProfitMarkups entity)
        {
            var query = _proftMarkups.Table;
            if (entity.CategoryId > 0)
                query = query.Where(e => e.CategoryId == entity.CategoryId);
            if (entity.CorporateCustomerType > 0)
                query = query.Where(e => e.CorporateCustomerType == entity.CorporateCustomerType);
            if (entity.CorporateCustomerId > 0)
                query = query.Where(e => e.CorporateCustomerId == entity.CorporateCustomerId);
            
            return query.FirstOrDefault();
        }

        /// <summary>
        /// Find a CorporateCustomerProfitMarkups using profit markup id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>CorporateCustomerProfitMarkups</returns>
        public CorporateCustomerProfitMarkups Find(int id)
        {
            if (id == 0)
                throw new ArgumentNullException();

            return _proftMarkups.ToCachedGetById(id);
        }


        /// <summary>
        /// Update a CorporateCustomerProfitMarkups
        /// </summary>
        /// <param name="model">CorporateCustomerProfitMarkups</param>
        public void Update(CorporateCustomerProfitMarkups model)
        {
            if (model == null)
                throw new ArgumentNullException();

            _proftMarkups.Update(model);
        }

        /// <summary>
        /// Delete a CorporateCustomerProfitMarkups
        /// </summary>
        /// <param name="model">CorporateCustomerProfitMarkups</param>
        public void Delete(CorporateCustomerProfitMarkups model)
        {
            if (model == null)
                throw new ArgumentNullException();

            _proftMarkups.Delete(model);
        }

        /// <summary>
        /// Insert CorporateCustomerProfitMarkups
        /// </summary>
        /// <param name="model">CorporateCustomerProfitMarkups</param>
        public void InsertProfitMarkups(CorporateCustomerProfitMarkups model)
        {
            if (model == null)
                throw new ArgumentNullException();

            _proftMarkups.Insert(model);
        }

        public CorporateCustomerProfitMarkups Find(int corporateCustId = 0, int productId = 0)
        {
            Category currentCategory = null;
            Category parentCategory = null;

            var corporateCustomer = _corporateCustomerService.GetCustomerById(corporateCustId);
            if (corporateCustomer == null)
                return null;

            currentCategory = _categoryService.GetCategoriesByProductId(productId, false);
            if (currentCategory == null)
                return null;

            //Find the parent category for the
            do
            {
                parentCategory = _categoryService.GetCategoryById(currentCategory.ParentCategoryId);
                if (parentCategory != null)
                    currentCategory = parentCategory;
            }
            while (parentCategory != null);

            var query = _proftMarkups.Table.Where(e => e.CorporateCustomerId == corporateCustId)
                .Where(e => e.CategoryId == currentCategory.Id);
            var profitMarkup = query.FirstOrDefault();
            if (profitMarkup != null)
                return profitMarkup;

            var copCust = _corporateCustomerService.GetCustomerById(corporateCustId);
            query = _proftMarkups.Table.Where(e => e.CorporateCustomerType == copCust.CustomerType)
                .Where(e => e.CategoryId == currentCategory.Id);

            return query.FirstOrDefault();
        }

        public bool GetProductPrice(int corporateCustId, int productId, out decimal productPrice)
        {
            Category currentCategory = null;
            Category parentCategory = null;
            productPrice = decimal.Zero;

            var corporateCustomer = _corporateCustomerService.GetCustomerById(corporateCustId);
            var product = _productRepository.GetById(productId);
            if (corporateCustomer == null || product == null)
                return false;

            currentCategory = _categoryService.GetCategoriesByProductId(productId, false);
            if (currentCategory == null)
                return false;

            //Find the parent category for the
            do
            {
                parentCategory = _categoryService.GetCategoryById(currentCategory.ParentCategoryId);
                if (parentCategory != null)
                    currentCategory = parentCategory;
            }
            while (parentCategory != null);

            var query = _proftMarkups.Table.Where(e => e.CorporateCustomerId == corporateCustId)
                .Where(e => e.CategoryId == currentCategory.Id);
            var profitMarkup = query.FirstOrDefault();
            if (profitMarkup != null)
            {
                var profit = product.ProductCost * (profitMarkup.ProfitMarkup / 100);
                productPrice = product.ProductCost + profit;
                return true;
            }

            var copCust = _corporateCustomerService.GetCustomerById(corporateCustId);
            query = _proftMarkups.Table.Where(e => e.CorporateCustomerType == copCust.CustomerType)
                .Where(e => e.CategoryId == currentCategory.Id);
            profitMarkup = query.FirstOrDefault();
            if (profitMarkup != null)
            {
                var profit = product.ProductCost * (profitMarkup.ProfitMarkup / 100);
                productPrice = product.ProductCost + profit;
                return true;
            }

            return false;
        }
    }
}
