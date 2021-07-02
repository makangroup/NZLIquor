using System.Collections.Generic;
using System.Linq;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using static Nop.Web.Models.Catalog.TopMenuModel;

namespace Fenchurch.Web.Models.Catalog
{
    public class MobileHeader : BaseNopModel
    {
        public IList<CategorySimpleModel> Categories { get; set; }
        public IList<TopicModel> Topics { get; set; }

        public MobileHeader() 
        {
            Categories = new List<CategorySimpleModel>();
            Topics = new List<TopicModel>();
        }
    }
}
