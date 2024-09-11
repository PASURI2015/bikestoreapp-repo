using NUnit.Framework;
using Moq;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rohit_bike_store.DTO;
using System.Linq.Expressions;

namespace TestProject
{
    [TestFixture]
    public class OrderItemServicesTests
    {
        private Mock<RohitBikeStoreContext> _mockContext;
        private OrderItemServices _orderItemServices;
        private Mock<DbSet<OrderItem>> _mockSet;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<RohitBikeStoreContext>();
            _mockSet = new Mock<DbSet<OrderItem>>();
            _mockContext.Setup(m => m.OrderItems).Returns(_mockSet.Object);
            _orderItemServices = new OrderItemServices(_mockContext.Object);
        }

        
        //[Test]
        //public async Task GetAllOrderItemsAsync_ReturnsAllItems()
        //{
        //    // Arrange
        //    var orderItems = new List<OrderItem>
        //    {
        //        new OrderItem { OrderId = 1, ItemId = 1 },
        //        new OrderItem { OrderId = 1, ItemId = 2 }
        //    };
        //    var mockDbSet = orderItems.AsQueryable().BuildMockDbSet();
        //    _mockContext.Setup(m => m.OrderItems).Returns(mockDbSet.Object);

        //    // Act
        //    var result = await _orderItemServices.GetAllOrderItemsAsync();

        //    // Assert
        //    Assert.That(result.Count(), Is.EqualTo(0));
        //}

        [Test]
        public async Task GetBillAmountAsync_ValidIds_ReturnsCorrectAmount()
        {
            // Arrange
            var orderItem = new OrderItem { OrderId = 1, ItemId = 1, ListPrice = 100, Quantity = 2, Discount = 10 };
            _mockSet.Setup(m => m.FindAsync(1, 1)).ReturnsAsync(orderItem);

            // Act
            var result = await _orderItemServices.GetBillAmountAsync(1, 1);

            // Assert
            Assert.That(result, Is.EqualTo(190));
        }

        [Test]
        public async Task GetBillAmountAsync_InvalidIds_ReturnsZero()
        {
            // Arrange
            _mockSet.Setup(m => m.FindAsync(1, 1)).ReturnsAsync((OrderItem)null);

            // Act
            var result = await _orderItemServices.GetBillAmountAsync(1, 1);

            // Assert
            Assert.That(result, Is.EqualTo(0));
        }

        [Test]
        public async Task GetBillWithoutDiscountAsync_ValidIds_ReturnsCorrectAmount()
        {
            // Arrange
            var orderItem = new OrderItem { OrderId = 1, ItemId = 1, ListPrice = 100, Quantity = 2 };
            _mockSet.Setup(m => m.FindAsync(1, 1)).ReturnsAsync(orderItem);

            // Act
            var result = await _orderItemServices.GetBillWithoutDiscountAsync(1, 1);

            // Assert
            Assert.That(result, Is.EqualTo(200));
        }

        [Test]
        public async Task GetOrderItemByIdAsync_ValidId_ReturnsOrderItem()
        {
            // Arrange
            var orderItem = new OrderItem { OrderId = 1, ItemId = 1 };
            _mockSet.Setup(m => m.FindAsync(1)).ReturnsAsync(orderItem);

            // Act
            var result = await _orderItemServices.GetOrderItemByIdAsync(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.OrderId, Is.EqualTo(1));
        }

        [Test]
        public async Task AddOrderItemAsync_ValidItem_ReturnsSuccessMessage()
        {
            // Arrange
            var orderItemDto = new AddOrderItemDto
            {
                OrderId = 1,
                ItemId = 1,
                ProductId = 1,
                Quantity = 2,
                ListPrice = 10.99m,
                Discount = 0
            };

            // Act
            var result = await _orderItemServices.AddOrderItemAsync(orderItemDto);

            // Assert
            Assert.That(result, Is.EqualTo("Record Created Successfully"));
            _mockSet.Verify(m => m.Add(It.Is<OrderItem>(oi =>
                oi.OrderId == orderItemDto.OrderId &&
                oi.ItemId == orderItemDto.ItemId &&
                oi.ProductId == orderItemDto.ProductId &&
                oi.Quantity == orderItemDto.Quantity &&
                oi.ListPrice == orderItemDto.ListPrice &&
                oi.Discount == orderItemDto.Discount)), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public async Task AddOrderItemAsync_ExceptionThrown_ReturnsErrorMessage()
        {
            // Arrange
            var orderItemDto = new AddOrderItemDto
            {
                OrderId = 1,
                ItemId = 1,
                ProductId = 1,
                Quantity = 2,
                ListPrice = 10.99m,
                Discount = 0
            };
            _mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _orderItemServices.AddOrderItemAsync(orderItemDto);

            // Assert
            Assert.That(result, Does.StartWith("Validation failed:"));
            Assert.That(result, Does.Contain("Test exception"));
        }


        [Test]
        public async Task UpdateOrderItemAsync_InvalidId_ReturnsFalse()
        {
            // Arrange
            var orderId = 1;
            var itemId = 1;
            var updatedOrderItemDto = new AddOrderItemDto
            {
                ItemId = itemId,
                ProductId = 2,
                Quantity = 5,
                ListPrice = 15.99m,
                Discount = 5
            };

            // Create a list with a non-matching item
            var mockData = new List<OrderItem>
    {
        new OrderItem { OrderId = 2, ItemId = 2 } // Different OrderId and ItemId
    }.AsQueryable();

            var mockSet = new Mock<DbSet<OrderItem>>();
            mockSet.As<IQueryable<OrderItem>>().Setup(m => m.Provider).Returns(mockData.Provider);
            mockSet.As<IQueryable<OrderItem>>().Setup(m => m.Expression).Returns(mockData.Expression);
            mockSet.As<IQueryable<OrderItem>>().Setup(m => m.ElementType).Returns(mockData.ElementType);
            mockSet.As<IQueryable<OrderItem>>().Setup(m => m.GetEnumerator()).Returns(mockData.GetEnumerator());

            _mockContext.Setup(c => c.OrderItems).Returns(mockSet.Object);

            // Act
            var result = await _orderItemServices.UpdateOrderItemAsync(orderId, updatedOrderItemDto, itemId);

            // Assert
            Assert.That(result, Is.False);
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

    }

    public static class MockDbSetExtensions
    {
        public static Mock<DbSet<T>> BuildMockDbSet<T>(this IQueryable<T> data) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(data.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(data.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(data.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(data.GetEnumerator());
            return mockSet;
        }
    }
}