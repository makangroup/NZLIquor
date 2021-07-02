using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Nop.Core.Domain.CorporateCustomer;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Services.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Test.Orders
{
    [TestClass]
    public class OrderServiceTest
    {
        private Mock<IRepository<CorporateCustomer>> _corporateCustomerRepository;
        private Mock<IRepository<Order>> _orderRepository;
        private CorporateCustomer _corporateCustomer;
        private Order _order;
        private OrderService _orderService;

        [TestInitialize]
        public void Setup()
        {
            //Initilize entities
            _corporateCustomer = new CorporateCustomer
            {
                Customer_Id = 10,
                CreditLimit = 10000,
                CreditPeriod = 14,
            };
            _order = new Order
            {
                CreatedOnUtc = DateTime.Now,
                CustomerId = _corporateCustomer.Customer_Id,
            };

            //Initilize repositories
            _corporateCustomerRepository = new Mock<IRepository<CorporateCustomer>>();
            _orderRepository = new Mock<IRepository<Order>>();
            var corporateCustomers = new List<CorporateCustomer>() { _corporateCustomer };
            _corporateCustomerRepository.Setup(p => p.Table).Returns(corporateCustomers.AsQueryable());
            var orders = new List<Order>() { _order };
            _orderRepository.Setup(p => p.Table).Returns(orders.AsQueryable());

            //Initilize the service classes
            _orderService = new OrderService(null,
            null,
            null,
            null,
            _corporateCustomerRepository.Object,
            null,
            _orderRepository.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
        }

        #region SearchOverDueOrders
        [TestMethod("Verify SearchOverDueOrders for non-overdue orders")]
        public void SearchOverDueOrdersTest1()
        {
            var pages = _orderService.SearchOverDueOrders();
            Assert.AreEqual(0, pages.Count, "SearchOverDueOrders is returning not overdue orders");
        }

        [TestMethod("Verify SearchOverDueOrders for overdue orders")]
        public void SearchOverDueOrdersTest2()
        {
            var overDueOrder = new Order
            {
                CreatedOnUtc = DateTime.Now.AddDays(-_corporateCustomer.CreditPeriod - 1),
                CustomerId = _corporateCustomer.Customer_Id,
            };

            var orders = new List<Order>() { overDueOrder };
            _orderRepository.Setup(p => p.Table).Returns(orders.AsQueryable());

            var pages = _orderService.SearchOverDueOrders();
            Assert.AreEqual(1, pages.Count, "SearchOverDueOrders method doesn't return overdue orders");
        }

        [TestMethod("Verify SearchOverDueOrders with higher From Date")]
        public void SearchOverDueOrdersTest3()
        {
            var overDueOrder = new Order
            {
                CreatedOnUtc = DateTime.Now.AddDays(-_corporateCustomer.CreditPeriod - 1),
                CustomerId = _corporateCustomer.Customer_Id,
            };

            var orders = new List<Order>() { overDueOrder };
            _orderRepository.Setup(p => p.Table).Returns(orders.AsQueryable());

            var pages = _orderService.SearchOverDueOrders(createdFromUtc: overDueOrder.CreatedOnUtc.AddDays(-1));
            Assert.AreEqual(1, pages.Count, "Doesn't return orders when fromDate is higher.");
        }

        [TestMethod("Verify SearchOverDueOrders with lower From Date")]
        public void SearchOverDueOrdersTest4()
        {
            var overDueOrder = new Order
            {
                CreatedOnUtc = DateTime.Now.AddDays(-_corporateCustomer.CreditPeriod - 1),
                CustomerId = _corporateCustomer.Customer_Id,
            };

            var orders = new List<Order>() { overDueOrder };
            _orderRepository.Setup(p => p.Table).Returns(orders.AsQueryable());

            var pages = _orderService.SearchOverDueOrders(createdFromUtc: overDueOrder.CreatedOnUtc.AddDays(1));
            Assert.AreEqual(0, pages.Count, "Doesn't return orders when fromDate is lower.");
        }
        #endregion

        #region GetCreditOrdersTotal
        [TestMethod]
        public void GetCreditOrdersTotal()
        {

        }
        #endregion
    }
}
