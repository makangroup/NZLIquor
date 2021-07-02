using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Shipping;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fenchurch.Web.Factories
{
    public interface IMessageSchedularFactory
    {
        public List<int> GetLast4DaysOrderListInPickUpStore();
        public List<Shipment> GetLast4DaysOrderListShipments();
        public void SendFeedBackMessageInStorePickup(List<int> orderList);
        public void SendFeedBackMessageShipments(List<Shipment> ShipmentList);
        public void SendLast4DaysFeedbackMessage();
        PictureModel ProductPictureModel(Product product, int? productThumbPictureSize = null);

        public string GetLocalStoreurl();

    }
}
