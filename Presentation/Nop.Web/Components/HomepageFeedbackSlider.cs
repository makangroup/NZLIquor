using Microsoft.AspNetCore.Mvc;
using Nop.Web.Factories;
using Nop.Web.Framework.Components;

namespace Fenchurch.Web.Components
{
    public class HomepageFeedbackSliderViewComponent : NopViewComponent
    {
        private readonly ICommonModelFactory _commonModelFactory;

        public HomepageFeedbackSliderViewComponent(ICommonModelFactory commonModelFactory)
        {
            _commonModelFactory = commonModelFactory;
        }

        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
