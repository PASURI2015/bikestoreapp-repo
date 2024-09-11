using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;

namespace TestProject
{
    [TestFixture]
    public class OrderItemsControllerTests
    {
        private OrderItemsController _controller;
        private RohitBikeStoreContext _context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new RohitBikeStoreContext(options);
            var orderItemService = new OrderItemServices(_context);
            _controller = new OrderItemsController(orderItemService);

            // Seed the database with some test data
            _context.OrderItems.Add(new OrderItem { OrderId = 1, ItemId = 1, ProductId = 1, Quantity = 2, ListPrice = 100, Discount = 0.1m });
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task PostOrderItem_ValidData_ReturnsOkResult()
        {
            // Arrange
            var orderItem = new AddOrderItemDto { OrderId = 2, ItemId = 1, ProductId = 2, Quantity = 1, ListPrice = 200, Discount = 0.05m };

            // Act
            var result = await _controller.PostOrderItem(orderItem);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo("Record Created Successfully"));
        }

        [Test]
        public async Task GetAllOrderItems_ReturnsOkResultWithItems()
        {
            // Act
            var result = await _controller.GetAllOrderItems();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var items = okResult.Value as IEnumerable<OrderItem>;
            Assert.That(items.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task PutOrderItem_ValidData_ReturnsNoContent()
        {
            // Arrange
            var orderItem = new AddOrderItemDto { OrderId = 1, ItemId = 1, ProductId = 1, Quantity = 3, ListPrice = 150, Discount = 0.15m };

            // Act
            var result = await _controller.PutOrderItem(1, orderItem, 1);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task PutOrderItem_InvalidData_ReturnsNotFound()
        {
            // Arrange
            var orderItem = new AddOrderItemDto { OrderId = 999, ItemId = 999, ProductId = 999, Quantity = 1, ListPrice = 100, Discount = 0.1m };

            // Act
            var result = await _controller.PutOrderItem(999, orderItem, 999);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task GetBillAmount_ValidData_ReturnsOkResultWithCorrectAmount()
        {
            // Act
            var result = await _controller.GetBillAmount(1, 1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var amount = (decimal)okResult.Value;
            Assert.That(amount, Is.EqualTo(199.9m)); // 2 * 100 * (1 - 0.1)
        }

        [Test]
        public async Task GetBillAmount_InvalidData_ReturnsStatusCode500()
        {
            // Act
            var result = await _controller.GetBillAmount(999, 999);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
        }

        [Test]
        public async Task GetBillWithoutDiscount_ValidData_ReturnsOkResultWithCorrectAmount()
        {
            // Act
            var result = await _controller.GetBillWithoutDiscount(1, 1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            var amount = (decimal)okResult.Value;
            Assert.That(amount, Is.EqualTo(200)); // 2 * 100
        }

        [Test]
        public async Task GetBillWithoutDiscount_InvalidData_ReturnsStatusCode500()
        {
            // Act
            var result = await _controller.GetBillWithoutDiscount(999, 999);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(200));
        }
    }
}




//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;
//using Moq;
//using Rohit_bike_store.Controllers;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;

//namespace Rohit_bike_store.Tests
//{
//    [TestFixture]
//    public class OrderItemsControllerTests
//    {
//        private Mock<IOrderItem> _mockOrderItemService;
//        private OrderItemsController _controller;

//        [SetUp]
//        public void Setup()
//        {
//            _mockOrderItemService = new Mock<IOrderItem>();
//            _controller = new OrderItemsController(_mockOrderItemService.Object);
//        }

//        [Test]
//        public async Task PostOrderItem_ValidData_ReturnsOkResult()
//        {
//            // Arrange
//            var orderItem = new OrderItem { /* Set properties */ };
//            _mockOrderItemService.Setup(s => s.AddOrderItemAsync(It.IsAny<OrderItem>()))
//                .ReturnsAsync("Record Created Successfully");

//            // Act
//            var result = await _controller.PostOrderItem(orderItem);

//            // Assert
//            Assert.IsInstanceOf<OkObjectResult>(result);
//        }

//        [Test]
//        public async Task PostOrderItem_InvalidData_ReturnsBadRequest()
//        {
//            // Arrange
//            var orderItem = new OrderItem { /* Set invalid properties */ };
//            _mockOrderItemService.Setup(s => s.AddOrderItemAsync(It.IsAny<OrderItem>()))
//                .ReturnsAsync("Validation failed");

//            // Act
//            var result = await _controller.PostOrderItem(orderItem);

//            // Assert
//            Assert.IsInstanceOf<BadRequestObjectResult>(result);
//        }

//        [Test]
//        public async Task GetAllOrderItems_ReturnsOkResult()
//        {
//            // Arrange
//            var orderItems = new List<OrderItem> { /* Add sample order items */ };
//            _mockOrderItemService.Setup(s => s.GetAllOrderItemsAsync())
//                .ReturnsAsync(orderItems);

//            // Act
//            var result = await _controller.GetAllOrderItems();

//            // Assert
//            Assert.IsInstanceOf<OkObjectResult>(result);
//        }

//        [Test]
//        public async Task PutOrderItem_ValidData_ReturnsNoContent()
//        {
//            // Arrange
//            var orderId = 1;
//            var itemId = 1;
//            var orderItem = new OrderItem { /* Set properties */ };
//            _mockOrderItemService.Setup(s => s.UpdateOrderItemAsync(orderId, orderItem, itemId))
//                .ReturnsAsync(true);

//            // Act
//            var result = await _controller.PutOrderItem(orderId, orderItem, itemId);

//            // Assert
//            Assert.IsInstanceOf<NoContentResult>(result);
//        }

//        [Test]
//        public async Task PutOrderItem_InvalidData_ReturnsNotFound()
//        {
//            // Arrange
//            var orderId = 1;
//            var itemId = 1;
//            var orderItem = new OrderItem { /* Set properties */ };
//            _mockOrderItemService.Setup(s => s.UpdateOrderItemAsync(orderId, orderItem, itemId))
//                .ReturnsAsync(false);

//            // Act
//            var result = await _controller.PutOrderItem(orderId, orderItem, itemId);

//            // Assert
//            Assert.IsInstanceOf<NotFoundResult>(result);
//        }

//        [Test]
//        public async Task GetBillAmount_ValidData_ReturnsOkResult()
//        {
//            // Arrange
//            var orderId = 1;
//            var itemId = 1;
//            var amount = 100.0m;
//            _mockOrderItemService.Setup(s => s.GetBillAmountAsync(orderId, itemId))
//                .ReturnsAsync(amount);

//            // Act
//            var result = await _controller.GetBillAmount(orderId, itemId);

//            // Assert
//            Assert.IsInstanceOf<OkObjectResult>(result);
//            Assert.AreEqual(amount, (result as OkObjectResult).Value);
//        }

//        [Test]
//        public async Task GetBillWithoutDiscount_ValidData_ReturnsOkResult()
//        {
//            // Arrange
//            var orderId = 1;
//            var itemId = 1;
//            var amount = 120.0m;
//            _mockOrderItemService.Setup(s => s.GetBillWithoutDiscountAsync(orderId, itemId))
//                .ReturnsAsync(amount);

//            // Act
//            var result = await _controller.GetBillWithoutDiscount(orderId, itemId);

//            // Assert
//            Assert.IsInstanceOf<OkObjectResult>(result);
//            Assert.AreEqual(amount, (result as OkObjectResult).Value);
//        }

//        [Test]
//        public async Task AllMethods_ExceptionThrown_ReturnsInternalServerError()
//        {
//            // Arrange
//            _mockOrderItemService.Setup(s => s.AddOrderItemAsync(It.IsAny<OrderItem>()))
//                .ThrowsAsync(new Exception("Test exception"));

//            _mockOrderItemService.Setup(s => s.GetAllOrderItemsAsync())
//                .ThrowsAsync(new Exception("Test exception"));

//            _mockOrderItemService.Setup(s => s.UpdateOrderItemAsync(It.IsAny<int>(), It.IsAny<OrderItem>(), It.IsAny<int>()))
//                .ThrowsAsync(new Exception("Test exception"));

//            _mockOrderItemService.Setup(s => s.GetBillAmountAsync(It.IsAny<int>(), It.IsAny<int>()))
//                .ThrowsAsync(new Exception("Test exception"));

//            _mockOrderItemService.Setup(s => s.GetBillWithoutDiscountAsync(It.IsAny<int>(), It.IsAny<int>()))
//                .ThrowsAsync(new Exception("Test exception"));

//            // Act & Assert
//            var postResult = await _controller.PostOrderItem(new OrderItem());
//            Assert.IsInstanceOf<ObjectResult>(postResult);
//            Assert.AreEqual(500, (postResult as ObjectResult).StatusCode);

//            var getAllResult = await _controller.GetAllOrderItems();
//            Assert.IsInstanceOf<ObjectResult>(getAllResult);
//            Assert.AreEqual(500, (getAllResult as ObjectResult).StatusCode);

//            var putResult = await _controller.PutOrderItem(1, new OrderItem(), 1);
//            Assert.IsInstanceOf<ObjectResult>(putResult);
//            Assert.AreEqual(500, (putResult as ObjectResult).StatusCode);

//            var getBillResult = await _controller.GetBillAmount(1, 1);
//            Assert.IsInstanceOf<ObjectResult>(getBillResult);
//            Assert.AreEqual(500, (getBillResult as ObjectResult).StatusCode);

//            var getBillWithoutDiscountResult = await _controller.GetBillWithoutDiscount(1, 1);
//            Assert.IsInstanceOf<ObjectResult>(getBillWithoutDiscountResult);
//            Assert.AreEqual(500, (getBillWithoutDiscountResult as ObjectResult).StatusCode);
//        }
//    }
//}