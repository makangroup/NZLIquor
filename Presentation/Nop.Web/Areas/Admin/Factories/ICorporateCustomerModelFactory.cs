using Fenchurch.Web.Areas.Admin.Models.CorporateCustomers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Areas.Admin.Factories
{
    public interface ICorporateCustomerModelFactory
    {
        public CorporateCustomerModel PrepareCustomerModel(CorporateCustomerModel model);
    }
}
