using NUnit.Framework;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestProject
{
    [TestFixture]
    public class CustomerServicesTests
    {
        private RohitBikeStoreContext _context;
        private CustomerServices _customerServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RohitBikeStoreContext(options);
            _customerServices = new CustomerServices(_context);

            // Seed the database with some test data
            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _context.Customers.AddRange(
                new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Phone = "1234567890", City = "New York", Street = "123 Main St", ZipCode = "10001" },
                new Customer { CustomerId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Phone = "0987654321", City = "Los Angeles", Street = "456 Elm St", ZipCode = "90001" }
            );

            _context.Orders.AddRange(
                new Order { OrderId = 1, CustomerId = 1, OrderDate = new DateOnly(2023, 1, 1) },
                new Order { OrderId = 2, CustomerId = 1, OrderDate = new DateOnly(2023, 2, 1) },
                new Order { OrderId = 3, CustomerId = 2, OrderDate = new DateOnly(2023, 1, 1) }
            );

            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddnewCustomerObjectinDB_ValidCustomer_ReturnsAddedCustomer()
        {
            // Arrange
            var newCustomer = new Customer { FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Phone = "1112223333", City = "Chicago", Street = "789 Oak St", ZipCode = "60601" };

            // Act
            var result = await _customerServices.AddnewCustomerObjectinDB(newCustomer);

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.CustomerId, Is.GreaterThan(0));
            Assert.That(result.FirstName, Is.EqualTo("Alice"));
        }

        [Test]
        public void AddnewCustomerObjectinDB_NullCustomer_ThrowsException()
        {
            // Arrange
            Customer nullCustomer = null;

            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _customerServices.AddnewCustomerObjectinDB(nullCustomer));
        }

        [Test]
        public async Task GetallCustomers_ReturnsAllCustomers()
        {
            // Act
            var result = await _customerServices.GetallCustomers();

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Getcustomersbyorderdate_ValidDate_ReturnsCustomers()
        {
            // Arrange
            var orderDate = new DateOnly(2023, 1, 1);

            // Act
            var result = await _customerServices.Getcustomersbyorderdate(orderDate);

            // Assert
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task Getcustomersbyorderdate_InvalidDate_ReturnsEmptyList()
        {
            // Arrange
            var orderDate = new DateOnly(2022, 1, 1);

            // Act
            var result = await _customerServices.Getcustomersbyorderdate(orderDate);

            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Getcustomersbyzipcode_ValidZipCode_ReturnsCustomers()
        {
            // Act
            var result = await _customerServices.Getcustomersbyzipcode("10001");

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].FirstName, Is.EqualTo("John"));
        }

        [Test]
        public async Task Getcustomersbyzipcode_InvalidZipCode_ReturnsEmptyList()
        {
            // Act
            var result = await _customerServices.Getcustomersbyzipcode("99999");

            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Getdisplaycustomerbycityandorderbyname_ValidCity_ReturnsCustomers()
        {
            // Act
            var result = await _customerServices.Getdisplaycustomerbycityandorderbyname("New York");

            // Assert
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].FirstName, Is.EqualTo("John"));
        }

        [Test]
        public async Task Getdisplaycustomerbycityandorderbyname_InvalidCity_ReturnsEmptyList()
        {
            // Act
            var result = await _customerServices.Getdisplaycustomerbycityandorderbyname("Invalid City");

            // Assert
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Getthecustomerwhoplacedhighestorder_ReturnsCustomerName()
        {
            // Act
            var result = await _customerServices.Getthecustomerwhoplacedhighestorder();

            // Assert
            Assert.That(result, Is.EqualTo("John"));
        }

        //[Test]
        //public async Task Updatethewholedetails_ValidCustomer_ReturnsUpdatedCustomer()
        //{
        //    // Arrange
        //    var updatedCustomer = new Customer { CustomerId = 1, FirstName = "John", LastName = "Updated", Email = "john.updated@example.com", Phone = "9876543210", City = "Updated City", Street = "Updated Street", ZipCode = "12345" };

        //    // Act
        //    var result = await _customerServices.Updatethewholedetails(updatedCustomer);

        //    // Assert
        //    Assert.IsNotNull(result);
        //    Assert.That(result.LastName, Is.EqualTo("Updated"));
        //    Assert.That(result.Email, Is.EqualTo("john.updated@example.com"));
        //}

        [Test]
        public async Task Updatethewholedetails_InvalidCustomerId_ReturnsNull()
        {
            // Arrange
            var invalidCustomer = new Customer { CustomerId = 999, FirstName = "Invalid", LastName = "Customer" };

            // Act
            var result = await _customerServices.Updatethewholedetails(invalidCustomer);

            // Assert
            Assert.IsNull(result);
        }

        // Note: Editthecustomerstreet method is not implemented, so we can't test it.
        // If you implement it, you should add tests for it as well.
    }
}