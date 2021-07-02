using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Windcave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Factories
{
    public interface IRequestOutputFactory
    {
        public RequestOutput GenerateRequestOutput(Customer customer, string orderids);
    }
}
