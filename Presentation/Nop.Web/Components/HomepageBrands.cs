using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Fenchurch.Web.Components
{
    public class HomepageBrandsViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public HomepageBrandsViewComponent(ICommonModelFactory commonModelFactory) 
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
