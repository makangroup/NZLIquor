using Fenchurch.Web.Areas.Admin.Models.CorporateCustomers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.OData.Query.SemanticAst;
using Nop.Core;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;
using Nop.Services.CorporateCustomers;
using Nop.Web.Framework.Models.Extensions;
using System;
using System.Linq;

namespace Fenchurch.Web.Areas.Admin.Factories
{
    public class ProfitMarkupModelFactory : IProfitMarkupModelFactory
    {
        #region Fields
        private readonly IProfitMarkupsService _profitMarkupSerrvice;
        private readonly ICategoryService _categoryService;
        private readonly ICorporateCustomerTypeService _corporateCustomerTypeService;
        #endregion

        public ProfitMarkupModelFactory(
            IProfitMarkupsService profitMarkupsService, 
            ICategoryService categoryService,
            ICorporateCustomerTypeService corporateCustomerTypeService)
        {
            _profitMarkupSerrvice = profitMarkupsService;
            _categoryService = categoryService;
            _corporateCustomerTypeService = corporateCustomerTypeService;
        }

        /// <summary>
        /// Prepare Profit Markups List Model List
        /// </summary>
        /// <param name="profitMarkupsSearch"></param>
        /// <returns></returns>
        public ProfitMarkupListModel PrepareProfitMarkupListModelListModel(ProfitMarkupsSearchModel profitMarkupsSearch)
        {
            if (profitMarkupsSearch == null)
                throw new ArgumentNullException();

            //If 
            IPagedList<CorporateCustomerProfitMarkups> profitMarkups = null;
            if (profitMarkupsSearch.Id > 0)
                profitMarkups = _profitMarkupSerrvice.GetAllProfitMarkupsByCorporateCustomerId(profitMarkupsSearch.Id);
            else
                profitMarkups = _profitMarkupSerrvice.GetAllProfitMarkups();

            var model = new ProfitMarkupListModel().PrepareToGrid(profitMarkupsSearch, profitMarkups, () =>
            {
                return profitMarkups.Select(profitMarkup =>
                {
                    var profitMarkupModel = new ProfitMarkupsModel();
                    profitMarkupModel.Id = profitMarkup.Id;
                    profitMarkupModel.ProfitMarkup = profitMarkup.ProfitMarkup;
                    profitMarkupModel.ProductCategorie = _categoryService.GetCategoryById(profitMarkup.CategoryId).Name;
                    profitMarkupModel.CorporateCustomerTypeName = profitMarkup.CorporateCustomerType > 0 ? _corporateCustomerTypeService.GetCorporateCustomerTypeById(profitMarkup.CorporateCustomerType).Name : "";
                    return profitMarkupModel;
                });
            });

            return model;
        }

        public ProfitMarkupsModel PrepareProfitMarkupModel(ProfitMarkupsModel model)
        {
            if (model == null)
                throw new ArgumentNullException();


            model.ProductCategories = _categoryService.GetAllParentCategories().Select(e => new SelectListItem(e.Name, e.Id.ToString())).ToList();
            model.CorporateCustomerTypes = _corporateCustomerTypeService.GetAllCorporateCustomerTypes().Select(e => new SelectListItem(e.Name, e.Id.ToString())).ToList();
            //model.CorporateCustomerTypes =

            return model;
        }
    }
}
