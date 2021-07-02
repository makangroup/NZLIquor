using Fenchurch.Web.Areas.Admin.Models.CorporateCustomers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Areas.Admin.Factories
{
    public interface IProfitMarkupModelFactory
    {
        public ProfitMarkupListModel PrepareProfitMarkupListModelListModel(ProfitMarkupsSearchModel profitMarkupsSearch);

        public ProfitMarkupsModel PrepareProfitMarkupModel(ProfitMarkupsModel model);
    }
}
