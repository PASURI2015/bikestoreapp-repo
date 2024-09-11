using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using Moq;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Rohit_bike_store.DTO;

namespace TestProject
{
    [TestFixture]
    public class OrdersControllerTests
    {
        private OrdersController _controller;
        private Mock<IOrder> _orderServiceMock;
        private Mock<IMapper> _mapperMock;

        [SetUp]
        public void Setup()
        {
            _orderServiceMock = new Mock<IOrder>();
            _mapperMock = new Mock<IMapper>();
            _controller = new OrdersController(_orderServiceMock.Object, _mapperMock.Object);
        }

        [Test]
        public async Task GetOrders_ReturnsOkResult()
        {
            // Arrange
            var orders = new List<Order> { new Order(), new Order() };
            _orderServiceMock.Setup(s => s.GetAllOrders()).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetOrders_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _orderServiceMock.Setup(s => s.GetAllOrders()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrders();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task SearchOrderByCustomerID_ReturnsOkResult()
        {
            // Arrange
            int customerId = 1;
            var orders = new List<Order> { new Order(), new Order() };
            _orderServiceMock.Setup(s => s.GetOrderByCustomerId(customerId)).ReturnsAsync(orders);

            // Act
            var result = await _controller.SearchOrderByCustomerID(customerId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task SearchOrderByCustomerID_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int customerId = 1;
            _orderServiceMock.Setup(s => s.GetOrderByCustomerId(customerId)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.SearchOrderByCustomerID(customerId);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetOrdersByCustomerName_ReturnsOkResult()
        {
            // Arrange
            string customerName = "John Doe";
            var orders = new List<Order> { new Order(), new Order() };
            _orderServiceMock.Setup(s => s.GetOrdersByCustomerName(customerName)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrdersByCustomerName(customerName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetOrdersByCustomerName_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            string customerName = "John Doe";
            _orderServiceMock.Setup(s => s.GetOrdersByCustomerName(customerName)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrdersByCustomerName(customerName);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetOrdersByDate_ReturnsOkResult()
        {
            // Arrange
            DateOnly date = new DateOnly(2023, 1, 1);
            var orders = new List<Order> { new Order(), new Order() };
            _orderServiceMock.Setup(s => s.GetOrdersByDate(date)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrdersByDate(date);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetOrdersByDate_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            DateOnly date = new DateOnly(2023, 1, 1);
            _orderServiceMock.Setup(s => s.GetOrdersByDate(date)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrdersByDate(date);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetOrdersByStatus_ReturnsOkResult()
        {
            // Arrange
            int status = 1;
            var orders = new List<Order> { new Order(), new Order() };
            _orderServiceMock.Setup(s => s.GetOrdersByStatus(status)).ReturnsAsync(orders);

            // Act
            var result = await _controller.GetOrdersByStatus(status);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetOrdersByStatus_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int status = 1;
            _orderServiceMock.Setup(s => s.GetOrdersByStatus(status)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetOrdersByStatus(status);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetNumberOfOrderByDate_ReturnsOkResult()
        {
            // Arrange
            DateOnly date = new DateOnly(2023, 1, 1);
            int count = 5;
            _orderServiceMock.Setup(s => s.GetNumberOfOrderByDate(date)).ReturnsAsync(count);

            // Act
            var result = await _controller.GetNumberOfOrderByDate(date);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetNumberOfOrderByDate_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            DateOnly date = new DateOnly(2023, 1, 1);
            _orderServiceMock.Setup(s => s.GetNumberOfOrderByDate(date)).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetNumberOfOrderByDate(date);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task GetDateWithMaximumOrders_ReturnsOkResult()
        {
            // Arrange
            DateOnly? date = new DateOnly(2023, 1, 1);
            _orderServiceMock.Setup(s => s.GetDateWithMaximumOrders()).ReturnsAsync(date);

            // Act
            var result = await _controller.GetDateWithMaximumOrders();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
        }

        [Test]
        public async Task GetDateWithMaximumOrders_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _orderServiceMock.Setup(s => s.GetDateWithMaximumOrders()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetDateWithMaximumOrders();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result.Result);
            var statusCodeResult = result.Result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task CreateOrder_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var orderDto = new OrderDto { CustomerId = 1, StoreId = 1, StaffId = 1 };
            var order = new Order { OrderId = 1, CustomerId = 1, StoreId = 1, StaffId = 1 };
            _orderServiceMock.Setup(s => s.CreateOrder(It.IsAny<Order>())).ReturnsAsync(order);
            _mapperMock.Setup(m => m.Map<OrderDto>(It.IsAny<Order>())).Returns(orderDto);

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
        }

        [Test]
        public async Task CreateOrder_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            var orderDto = new OrderDto { CustomerId = 1, StoreId = 1, StaffId = 1 };
            _orderServiceMock.Setup(s => s.CreateOrder(It.IsAny<Order>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.CreateOrder(orderDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }

        [Test]
        public async Task UpdateOrder_ReturnsOkResult()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDto { CustomerId = 1, StoreId = 1, StaffId = 1 };
            _orderServiceMock.Setup(s => s.IdExsist(orderId)).Returns(true);
            _orderServiceMock.Setup(s => s.UpdateOrder(orderId, It.IsAny<OrderDto>()));

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
        }

        [Test]
        public async Task UpdateOrder_ReturnsBadRequestResult()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDto { CustomerId = 1, StoreId = 1, StaffId = 1 };
            _orderServiceMock.Setup(s => s.IdExsist(orderId)).Returns(false);

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateOrder_ThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            int orderId = 1;
            var orderDto = new OrderDto { CustomerId = 1, StoreId = 1, StaffId = 1 };
            _orderServiceMock.Setup(s => s.IdExsist(orderId)).Returns(true);
            _orderServiceMock.Setup(s => s.UpdateOrder(orderId, It.IsAny<OrderDto>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.UpdateOrder(orderId, orderDto);

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var statusCodeResult = result as ObjectResult;
            Assert.AreEqual(500, statusCodeResult.StatusCode);
        }
    }
}