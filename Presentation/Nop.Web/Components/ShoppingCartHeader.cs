using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;


namespace Nop.Web.Components
{
    public class ShoppingCartHeaderViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public ShoppingCartHeaderViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            var model = _commonModelFactory.PrepareShoppingCartHeaderModel();
            return View(model);
        }
    }
}
