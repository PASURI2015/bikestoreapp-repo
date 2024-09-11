using NUnit.Framework;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Rohit_bike_store.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject
{
    [TestFixture]
    public class OrderServicesTests
    {
        private RohitBikeStoreContext _context;
        private IMapper _mapper;
        private OrderServices _orderServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new RohitBikeStoreContext(options);
            _mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Order, OrderDto>();
                cfg.CreateMap<OrderDto, Order>();
            }).CreateMapper();

            _orderServices = new OrderServices(_context, _mapper);

            // Clear the database before each test
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateOrder_ValidOrder_ReturnsCreatedOrder()
        {
            // Arrange
            var order = new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) };

            // Act
            var result = await _orderServices.CreateOrder(order);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.OrderId);
            Assert.AreEqual(1, result.CustomerId);
        }

        [Test]
        public async Task CreateOrder_NullOrder_ReturnsNull()
        {
            // Act
            var result = await _orderServices.CreateOrder(null);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetAllOrders_OrdersExist_ReturnsListOfOrders()
        {
            // Arrange
            _context.Orders.AddRange(
                new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { CustomerId = 2, OrderDate = new DateOnly(2023, 1, 2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetAllOrders();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
        }

        [Test]
        public async Task GetAllOrders_NoOrders_ReturnsEmptyList()
        {
            // Act
            var result = await _orderServices.GetAllOrders();

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetDateWithMaximumOrders_OrdersExist_ReturnsCorrectDate()
        {
            // Arrange
            _context.Orders.AddRange(
                new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { CustomerId = 2, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { CustomerId = 3, OrderDate = new DateOnly(2023, 1, 2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetDateWithMaximumOrders();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(new DateOnly(2023, 1, 1), result);
        }

        [Test]
        public async Task GetDateWithMaximumOrders_NoOrders_ReturnsNull()
        {
            // Act
            var result = await _orderServices.GetDateWithMaximumOrders();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task GetOrderByCustomerId_OrdersExist_ReturnsCorrectOrders()
        {
            // Arrange
            _context.Orders.AddRange(
                new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 2) },
                new Order { CustomerId = 2, OrderDate = new DateOnly(2023, 1, 3) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrderByCustomerId(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(o => o.CustomerId == 1));
        }

        [Test]
        public async Task GetOrderByCustomerId_NoOrdersForCustomer_ReturnsEmptyList()
        {
            // Arrange
            _context.Orders.Add(new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrderByCustomerId(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }


        [Test]
        public async Task GetOrdersByCustomerName_OrdersExist_ReturnsCorrectOrders()
        {
            // Arrange
            var customer1 = new Customer { FirstName = "John", LastName = "Doe" , Email = "xyz@gmail.com"};
            var customer2 = new Customer { FirstName = "Jane", LastName = "Doe", Email = "avs@gmail.com" };
            _context.Customers.AddRange(customer1, customer2);
            await _context.SaveChangesAsync();

            _context.Orders.AddRange(
                new Order { CustomerId = customer1.CustomerId, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { CustomerId = customer1.CustomerId, OrderDate = new DateOnly(2023, 1, 2) },
                new Order { CustomerId = customer2.CustomerId, OrderDate = new DateOnly(2023, 1, 3) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrdersByCustomerName("John");

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(o => o.Customer.FirstName == "John"));
        }

        [Test]
        public async Task GetOrdersByCustomerName_NoOrdersForCustomerName_ReturnsEmptyList()
        {
            // Arrange
            var customer = new Customer { FirstName = "John", LastName = "Doe" , Email = "xyz@gmail.com"};
            _context.Customers.Add(customer);
            _context.Orders.Add(new Order { CustomerId = customer.CustomerId, OrderDate = new DateOnly(2023, 1, 1) });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrdersByCustomerName("Jane");

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetOrdersByDate_OrdersExist_ReturnsCorrectOrders()
        {
            // Arrange
            var testDate = new DateOnly(2023, 1, 1);
            _context.Orders.AddRange(
                new Order { CustomerId = 1, OrderDate = testDate },
                new Order { CustomerId = 2, OrderDate = testDate },
                new Order { CustomerId = 3, OrderDate = new DateOnly(2023, 1, 2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrdersByDate(testDate);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(o => o.OrderDate == testDate));
        }

        [Test]
        public async Task GetOrdersByDate_NoOrdersForDate_ReturnsEmptyList()
        {
            // Arrange
            _context.Orders.Add(new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrdersByDate(new DateOnly(2023, 1, 2));

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task GetOrdersByStatus_OrdersExist_ReturnsCorrectOrders()
        {
            // Arrange
            _context.Orders.AddRange(
                new Order { CustomerId = 1, OrderStatus = 1, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { CustomerId = 2, OrderStatus = 1, OrderDate = new DateOnly(2023, 1, 2) },
                new Order { CustomerId = 3, OrderStatus = 2, OrderDate = new DateOnly(2023, 1, 3) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrdersByStatus(1);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.All(o => o.OrderStatus == 1));
        }

        [Test]
        public async Task GetOrdersByStatus_NoOrdersWithStatus_ReturnsEmptyList()
        {
            // Arrange
            _context.Orders.Add(new Order { CustomerId = 1, OrderStatus = 1, OrderDate = new DateOnly(2023, 1, 1) });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetOrdersByStatus(2);

            // Assert
            Assert.IsNotNull(result);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task UpdateOrder_OrderExists_ReturnsUpdatedOrder()
        {
            // Arrange
            var existingOrder = new Order { CustomerId = 1, OrderStatus = 1, OrderDate = new DateOnly(2023, 1, 1) };
            _context.Orders.Add(existingOrder);
            await _context.SaveChangesAsync();

            var updatedOrderDto = new OrderDto { CustomerId = 2, OrderStatus = 2, OrderDate = new DateOnly(2023, 1, 2) };

            // Act
            var result = await _orderServices.UpdateOrder(existingOrder.OrderId, updatedOrderDto);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.CustomerId);
            Assert.AreEqual(2, result.OrderStatus);
            Assert.AreEqual(new DateOnly(2023, 1, 2), result.OrderDate);
        }

        [Test]
        public async Task UpdateOrder_OrderDoesNotExist_ReturnsNull()
        {
            // Arrange
            var updatedOrderDto = new OrderDto { CustomerId = 2, OrderStatus = 2, OrderDate = new DateOnly(2023, 1, 2) };

            // Act
            var result = await _orderServices.UpdateOrder(999, updatedOrderDto);

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public void IdExsist_IdExists_ReturnsTrue()
        {
            // Arrange
            var order = new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) };
            _context.Orders.Add(order);
            _context.SaveChanges();

            // Act
            var result = _orderServices.IdExsist(order.OrderId);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void IdExsist_IdDoesNotExist_ReturnsFalse()
        {
            // Act
            var result = _orderServices.IdExsist(999);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public async Task GetNumberOfOrderByDate_OrdersExist_ReturnsCorrectCount()
        {
            // Arrange
            var testDate = new DateOnly(2023, 1, 1);
            _context.Orders.AddRange(
                new Order { CustomerId = 1, OrderDate = testDate },
                new Order { CustomerId = 2, OrderDate = testDate },
                new Order { CustomerId = 3, OrderDate = new DateOnly(2023, 1, 2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetNumberOfOrderByDate(testDate);

            // Assert
            Assert.AreEqual(2, result);
        }
        [Test]
        public async Task GetNumberOfOrderByDate_NoOrdersForDate_ReturnsZero()
        {
            // Arrange
            _context.Orders.Add(new Order { CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) });
            await _context.SaveChangesAsync();

            // Act
            var result = await _orderServices.GetNumberOfOrderByDate(new DateOnly(2023, 1, 2));

            // Assert
            Assert.AreEqual(0, result);
        }

    }
}





//using Moq;
//using NUnit.Framework;
//using Microsoft.EntityFrameworkCore;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Linq;
//using System;
//using AutoMapper;
//using Rohit_bike_store.DTO;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;

//namespace BikeApp.Tests
//{
//    [TestFixture]
//    public class OrderServiceTests
//    {
//        private RohitBikeStoreContext _dbContext;
//        private OrderServices _orderService;
//        private IMapper _mapper;
//        [SetUp]
//        public void SetUp()
//        {
//            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
//                .UseInMemoryDatabase("TestDb")
//                .Options;

//            _dbContext = new RohitBikeStoreContext(options);

//            _dbContext.Orders.AddRange(new List<Order>
//            {
//                new Order{OrderId = 1, CustomerId = 1, OrderStatus = 1, OrderDate = DateOnly.FromDateTime(DateTime.Now), ShippedDate = DateOnly.FromDateTime(DateTime.Now), RequiredDate = DateOnly.FromDateTime(DateTime.Now), StoreId = 1, StaffId = 1},
//                new Order{OrderId = 2, CustomerId = 2, OrderStatus = 2, OrderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), ShippedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1)), StoreId = 2, StaffId = 2},
//            });

//            _dbContext.Customers.AddRange(new List<Customer>
//            {
//            new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
//            new Customer { CustomerId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" }
//            });
//            _dbContext.SaveChanges();

//            var config = new MapperConfiguration(cfg =>
//            {
//                cfg.CreateMap<OrderDto, Order>();
//            });

//            _mapper = config.CreateMapper();

//            _orderService = new OrderServices(_dbContext, _mapper);
//        }

//        [Test]
//        public async Task GetAllOrders_ShouldReturnAllOrders()
//        {


//            // Act
//            var result = await _orderService.GetAllOrders();

//            // Assert
//            Assert.That(result.Count, Is.EqualTo(2));
//        }

//        [Test]
//        public async Task GetOrderById_ShouldReturnOrder_WhenOrderExists()
//        {
//            // Act
//            var result = await _orderService.GetOrderByCustomerId(1);

//            // Assert
//            Assert.AreEqual(result.OrderId, 1);
//        }

//        [Test]
//        public async Task GetOrderByCustomerName_ShouldReturnCorrectOrder()
//        {

//            // Act
//            var result = await _orderService.GetOrderByCustomerName("John");

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(1, result.OrderId);
//            Assert.AreEqual(1, result.CustomerId);
//        }

//        [Test]

//        public async Task GetOrderByStatus_returnOrderStatus()
//        {
//            var result = await _orderService.GetOrderByStatus(1);


//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<List<Order>>(result);
//            Assert.That(result.All(o => o.OrderStatus == 1), Is.True);
//        }

//        [Test]
//        public async Task NumberOfOrderByDate_ShouldReturnOrdersSortedByDate()
//        {
//            var result = await _orderService.NumberOfOrderByDate();
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<List<Order>>(result);
//            Assert.That(result, Is.Ordered.By("OrderDate"));

//        }

//        [Test]
//        public async Task GetMaximumOrderDate_ShouldReturnDateWithMostOrders()
//        {
//            var result = await _orderService.GetMaximumOrderDate();
//            Assert.IsNotNull(result);
//            Assert.AreEqual(DateOnly.FromDateTime(DateTime.Now), result);
//        }

//        [Test]
//        public async Task GetOrderByDate_ShouldReturnOrders()
//        {
//            var testDate = DateOnly.FromDateTime(DateTime.Now);
//            var result = await _orderService.GetOrderByDate(testDate);
//            Assert.IsNotNull(result);
//            Assert.IsInstanceOf<List<Order>>(result);
//            Assert.That(result.All(o => o.OrderDate == testDate), Is.True);
//        }

//        //[Test]
//        //public async Task UpdateOrder_ShouldReturnUpdatedOrder()
//        //{
//        //    var orderToUpdate = new Order
//        //    {
//        //        OrderId = 1,
//        //        CustomerId = 1,
//        //        OrderStatus = 2,
//        //        OrderDate = DateOnly.FromDateTime(DateTime.Now),
//        //        ShippedDate = DateOnly.FromDateTime(DateTime.Now),
//        //        RequiredDate = DateOnly.FromDateTime(DateTime.Now),
//        //        StoreId = 1,
//        //        StaffId = 1
//        //    };
//        //    var result = await _orderService.UpdateOrder(orderToUpdate);
//        //    Assert.IsNotNull(result);
//        //    Assert.AreEqual(orderToUpdate.OrderId, result.OrderId);
//        //    Assert.AreEqual(orderToUpdate.OrderStatus, result.OrderStatus);
//        //}

//        [Test]
//        public async Task CreateOrder_ShouldAddNewOrder()
//        {
//            // Arrange
//            var newOrder = new Order
//            {
//                OrderId = 3,
//                CustomerId = 3,
//                OrderStatus = 3,
//                OrderDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
//                ShippedDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
//                RequiredDate = DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
//                StoreId = 3,
//                StaffId = 3
//            };

//            // Act
//            var result = await _orderService.CreateOrder(newOrder);

//            // Assert
//            Assert.IsNotNull(result);
//            Assert.AreEqual(newOrder.OrderId, result.OrderId);
//            Assert.AreEqual(newOrder.CustomerId, result.CustomerId);
//            Assert.AreEqual(newOrder.OrderStatus, result.OrderStatus);
//            Assert.AreEqual(newOrder.OrderDate, result.OrderDate);
//            Assert.AreEqual(newOrder.ShippedDate, result.ShippedDate);
//            Assert.AreEqual(newOrder.RequiredDate, result.RequiredDate);
//            Assert.AreEqual(newOrder.StoreId, result.StoreId);
//            Assert.AreEqual(newOrder.StaffId, result.StaffId);
//        }
//        [TearDown]
//        public void TearDown()
//        {
//            _dbContext.Dispose();
//        }
//    }
//}
