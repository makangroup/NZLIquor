using Nop.Core;
using Nop.Core.Domain.CorporateCustomer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.CorporateCustomers
{
    public interface IProfitMarkupsService
    {
        IPagedList<CorporateCustomerProfitMarkups> GetAllProfitMarkups(int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Get CorporateCustomerProfitMarkups for corporate customers
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="getOnlyTotalCount"></param>
        /// <returns></returns>
        public IPagedList<CorporateCustomerProfitMarkups> GetAllProfitMarkupsByCorporateCustomerId(int customerId, int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false);

        /// <summary>
        /// Insert a profit markup
        /// </summary>
        /// <param name="model"></param>
        public void InsertProfitMarkups(CorporateCustomerProfitMarkups model);

        /// <summary>
        /// Find the CorporateCustomerProfitMarkups with the given information.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public CorporateCustomerProfitMarkups Find(CorporateCustomerProfitMarkups entity);

        /// <summary>
        /// Update profit markups
        /// </summary>
        /// <param name="model"></param>
        public void Update(CorporateCustomerProfitMarkups model);

        /// <summary>
        /// Find a CorporateCustomerProfitMarkups using profit markup id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>CorporateCustomerProfitMarkups</returns>
        public CorporateCustomerProfitMarkups Find(int id);

        /// <summary>
        /// Find a CorporateCustomerProfitMarkups by corporate customer Id and productId Id
        /// </summary>
        /// <param name="corporateCustId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        public CorporateCustomerProfitMarkups Find(int corporateCustId = 0, int productId = 0);


        /// <summary>
        /// Delete a CorporateCustomerProfitMarkups
        /// </summary>
        /// <param name="model">CorporateCustomerProfitMarkups</param>
        public void Delete(CorporateCustomerProfitMarkups model);

        public bool GetProductPrice(int corporateCustId, int productId, out decimal productPrice);
    }
}
