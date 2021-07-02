using Microsoft.AspNetCore.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Caching;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Shipping;
using Nop.Web.Areas.Admin.Infrastructure.Cache;
using Nop.Web.Models.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Fenchurch.Web.Factories
{
    public class MessageSchedularFactory : IMessageSchedularFactory
    {

        #region Fields

        private readonly IOrderService _orderService;
        private readonly IMessageTemplateService _messageService;
        private readonly IMessageTokenProvider _messagetokenProvider;
        private readonly ICacheKeyService _cacheKeyService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IProductService _productService;
        private readonly MediaSettings _mediaSettings;
        private readonly ICustomerService _customerService;
        private readonly IWorkflowMessageService _workflowmessageService;
        private readonly IShipmentService _shipmentService;
        private readonly IHttpContextAccessor _httpContextAccessor;



        #endregion

        #region Ctor

        public MessageSchedularFactory(
            ICacheKeyService cacheKeyService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            IProductService productService,
            IStaticCacheManager staticCacheManager,
            IStoreContext storeContext,
            IWebHelper webHelper,
            IWorkContext workContext,
            IOrderService orderService,
            IMessageTemplateService messageService,
            IMessageTokenProvider messagetokenProvider,
            MediaSettings mediaSettings,
            ICustomerService customerService,
            IWorkflowMessageService workflowmessageService,
            IShipmentService shipmentService,
            IHttpContextAccessor httpContextAccessor)
        {


            _cacheKeyService = cacheKeyService;
            _localizationService = localizationService;
            _pictureService = pictureService;
            _staticCacheManager = staticCacheManager;
            _storeContext = storeContext;
            _webHelper = webHelper;
            _workContext = workContext;
            _orderService = orderService;
            _messageService = messageService;
            _messagetokenProvider = messagetokenProvider;
            _productService = productService;
            _pictureService = pictureService;
            _mediaSettings = mediaSettings;
            _customerService = customerService;
            _workflowmessageService = workflowmessageService;
            _shipmentService = shipmentService;
            _httpContextAccessor = httpContextAccessor;

        }
        #endregion

        public void SendLast4DaysFeedbackMessage()
        {
            SendFeedBackMessageInStorePickup(GetLast4DaysOrderListInPickUpStore());
            SendFeedBackMessageShipments(GetLast4DaysOrderListShipments());
        }

        public List<Shipment> GetLast4DaysOrderListShipments()
        {
            var OrdersList = _shipmentService.GetShipmentsBetweenDates(DateTime.UtcNow.AddDays(-4).Date, DateTime.UtcNow.AddDays(-3).Date).ToList();
            return OrdersList;
        }
        public List<int> GetLast4DaysOrderListInPickUpStore()
        {

            int OrderStatusComplete = (int)OrderStatus.Complete;
            var OrdersList = _orderService.SearchOrders(createdFromUtc: DateTime.UtcNow.AddDays(-4).Date, createdToUtc: DateTime.UtcNow.AddDays(-3).Date).Where(x => x.OrderStatusId == OrderStatusComplete && x.PickupInStore == true).Select(x => x.Id).ToList();
            return OrdersList;

        }

        public PictureModel ProductPictureModel(Product product, int? productThumbPictureSize = null)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            var productName = _localizationService.GetLocalized(product, x => x.Name);
            //If a size has been set in the view, we use it in priority
            var pictureSize = productThumbPictureSize ?? _mediaSettings.ProductThumbPictureSize;

            //prepare picture model
            var cacheKey = _cacheKeyService.PrepareKeyForDefaultCache(NopModelCacheDefaults.ProductDefaultPictureModelKey,
                product, pictureSize, true, _workContext.WorkingLanguage, _webHelper.IsCurrentConnectionSecured(),
                _storeContext.CurrentStore);

            var defaultPictureModel = _staticCacheManager.Get(cacheKey, () =>
            {
                var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(ref picture, pictureSize),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(ref picture),
                    //"title" attribute
                    Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute))
                        ? picture.TitleAttribute
                        : string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"),
                            productName),
                    //"alt" attribute
                    AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute))
                        ? picture.AltAttribute
                        : string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"),
                            productName)
                };

                return pictureModel;
            });

            return defaultPictureModel;
        }

        public void SendFeedBackMessageInStorePickup(List<int> orderList)
        {
            if (orderList.Count > 0)
            {
                for (int i = 0; i < orderList.Count; i++)
                {
                    var OrderItemList = _orderService.GetOrderItems(orderList[i]);
                    var Order = _orderService.GetOrderById(orderList[i]);
                    if (Order != null)
                    {
                        var Customer = _customerService.GetAddressesByCustomerId(Order.CustomerId).FirstOrDefault();
                        if (Customer != null)
                        {

                            MessageTemplate Message = _messageService.GetMessageTemplatesByName("FeedbackAfter4Days.CustomerNotification").FirstOrDefault();
                            if (Message != null)
                            {
                                StringBuilder productListSB = new StringBuilder();
                                StringBuilder MessageBody = new StringBuilder(Message.Body);
                                StringBuilder MessageSubject = new StringBuilder(Message.Subject);
                                foreach (var OrderItem in OrderItemList)
                                {
                                    var productDetails = _productService.GetProductById(OrderItem.ProductId);
                                    if (productDetails != null)
                                    {
                                        var productImage = ProductPictureModel(productDetails);
                                        if (productImage != null)
                                        {

                                            var ProductReviewLink = GetLocalStoreurl() + "productreviews/" + productDetails.Id;
                                            var StarLink = GetLocalStoreurl() + "images/star-01.png";
                                            productListSB.Append("<tr>" +
                                                    "<td style=\"display:inline-block;width:100px;float:left\">" +
                                                        "<a href = \"" + ProductReviewLink + "\" style=\"text-decoration:none;\"><img src = \"" + productImage.ImageUrl + "\" style =\"border:1px solid #d6dbe3;width:100px\" class=\"CToWUd\"></a>" +
                                                    "</td>" +
                                                    "<td style=\"padding:0 0 0 10px;vertical-align:top;line-height:17.5px;font-size:11px;font-weight:300\" >" +
                                                        "<a href = \"" + ProductReviewLink + "\" style=\"text-decoration: none;\"><p style=\"margin:0 0 5px 0;color:#666666\">" + productDetails.Name + "</p></a>" +
                                                        "<a href = \"" + ProductReviewLink + "\" style=\"text-decoration:none;\"><p style=\"margin:0 0 5px 0;color:#666666\"><span> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /></span></p></a>" +
                                                    "</td>" +
                                                "</tr>");
                                        }
                                    }
                                }
                                MessageBody.Replace("%Product.List%", productListSB.ToString());
                                MessageBody.Replace("%Order.No%", Order.CustomOrderNumber);
                                MessageBody.Replace("%Customer.Name%", Customer.FirstName + " " + Customer.LastName);
                                MessageBody.Replace("%Store.Name%", _storeContext.CurrentStore.Name);
                                MessageSubject.Replace("%Order.No%", Order.CustomOrderNumber);
                                MessageSubject.Replace("%Store.Name%", _storeContext.CurrentStore.Name);
                                List<MessageTemplate> MessageList = new List<MessageTemplate>();
                                MessageList.Add(Message);
                                List<Token> Tokenlist = new List<Token>();

                                _workflowmessageService.SendCustomerFeedBackMessage(Customer.Email, Customer.FirstName + " " + Customer.LastName, MessageBody.ToString(), MessageSubject.ToString(), Tokenlist, _workContext.WorkingLanguage.Id);
                            }
                        }
                    }
                }
            }
        }
        public void SendFeedBackMessageShipments(List<Shipment> ShipmentList)
        {
            if (ShipmentList.Count > 0)
            {
                for (int i = 0; i < ShipmentList.Count; i++)
                {

                    var OrderItemList = _shipmentService.GetShipmentItemsByShipmentId(ShipmentList[i].Id);
                    var Order = _orderService.GetOrderById(ShipmentList[i].OrderId);
                    if (Order != null)
                    {
                        var Customer = _customerService.GetAddressesByCustomerId(Order.CustomerId).FirstOrDefault();
                        if (Customer != null)
                        {

                            MessageTemplate Message = _messageService.GetMessageTemplatesByName("FeedbackAfter4Days.CustomerNotification").FirstOrDefault();
                            if (Message != null)
                            {
                                StringBuilder productListSB = new StringBuilder();
                                StringBuilder MessageBody = new StringBuilder(Message.Body);
                                StringBuilder MessageSubject = new StringBuilder(Message.Subject);
                                foreach (var OrderItem in OrderItemList)
                                {

                                    var productDetails = _orderService.GetProductByOrderItemId(OrderItem.OrderItemId);
                                    if (productDetails != null)
                                    {
                                        var productImage = ProductPictureModel(productDetails);
                                        if (productImage != null)
                                        {
                                            var ProductReviewLink = GetLocalStoreurl() + "productreviews/" + productDetails.Id;
                                            var StarLink = GetLocalStoreurl() + "images/star-01.png";
                                            productListSB.Append("<tr>" +
                                                    "<td style=\"display:inline-block;width:100px;float:left\">" +
                                                        "<a href = \"" + ProductReviewLink + "\" style=\"text-decoration:none;\"><img src = \"" + productImage.ImageUrl + "\" style =\"border:1px solid #d6dbe3;width:100px\" class=\"CToWUd\"></a>" +
                                                    "</td>" +
                                                    "<td style=\"padding:0 0 0 10px;vertical-align:top;line-height:17.5px;font-size:11px;font-weight:300\" >" +
                                                        "<a href = \"" + ProductReviewLink + "\" style=\"text-decoration:none;\"><p style=\"margin:0 0 5px 0;color:#666666\">" + productDetails.Name + "</p></a>" +
                                                        "<a href = \"" + ProductReviewLink + "\" style=\"text-decoration:none;\"><p style=\"margin:0 0 5px 0;color:#666666\"><img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /> <img src=\"" + StarLink + "\" height=\"30px\" /></p></a></td>" +
                                                "</tr>");
                                        }
                                    }
                                }
                                MessageBody.Replace("%Product.List%", productListSB.ToString());
                                MessageBody.Replace("%Order.No%", Order.CustomOrderNumber);
                                MessageBody.Replace("%Customer.Name%", Customer.FirstName + " " + Customer.LastName);
                                MessageBody.Replace("%Store.Name%", _storeContext.CurrentStore.Name);
                                MessageSubject.Replace("%Order.No%", Order.CustomOrderNumber);
                                MessageSubject.Replace("%Store.Name%", _storeContext.CurrentStore.Name);
                                List<MessageTemplate> MessageList = new List<MessageTemplate>();
                                MessageList.Add(Message);
                                List<Token> Tokenlist = new List<Token>();

                                _workflowmessageService.SendCustomerFeedBackMessage(Customer.Email, Customer.FirstName + " " + Customer.LastName, MessageBody.ToString(), MessageSubject.ToString(), Tokenlist, _workContext.WorkingLanguage.Id);
                            }
                        }
                    }
                }
            }
        }

        public string GetLocalStoreurl()
        {
            var storelocation = _webHelper.GetStoreLocation();
            var pathBase = _httpContextAccessor.HttpContext.Request.PathBase.Value ?? string.Empty;

            var imagesPathUrl = _mediaSettings.UseAbsoluteImagePath ? storelocation : $"{pathBase}/";

            imagesPathUrl = string.IsNullOrEmpty(imagesPathUrl)
                ? storelocation
                : imagesPathUrl;

            return imagesPathUrl;
        }
    }
}
