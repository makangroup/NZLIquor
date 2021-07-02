using System;
using System.Threading.Tasks;
using Fenchurch.Web.Factories;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Messages;
using Nop.Services.Orders;

namespace Fenchurch.Web.Controllers
{
    public class EmailSchedularApiController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IMessageTemplateService _messageService;
        private readonly IMessageSchedularFactory _messageSchedularFactory;
        public EmailSchedularApiController(
            IOrderService orderService,
            IMessageTemplateService messageService,
            IMessageSchedularFactory messageSchedularFactory)
        {
            _orderService = orderService;
            _messageService = messageService;
            _messageSchedularFactory = messageSchedularFactory;
        }
        
        public IActionResult SendMessageAfter4Days()
        {
            string Result = "0";
            string Message = "Something Went Wrong";
            try
            {
                _messageSchedularFactory.SendLast4DaysFeedbackMessage();

                Result = "1";
                Message = "Emails Sent Successfully!!";
            }
            catch (Exception ex)
            {
                Result = "0";
                Message = ex.Message;
            }

            return Ok(new { result = Result, message = Message });
        }
    }
}
