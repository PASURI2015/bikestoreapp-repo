using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Moq;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Rohit_bike_store.DTO;

namespace TestProject
{
    [TestFixture]
    public class CustomersControllerTests
    {
        private CustomersController _controller;
        private Mock<ICustomer> _mockCustomerService;

        [SetUp]
        public void Setup()
        {
            _mockCustomerService = new Mock<ICustomer>();
            _controller = new CustomersController(_mockCustomerService.Object);
        }

        [Test]
        public async Task GetallCustomers_ReturnsListOfCustomers()
        {
            // Arrange
            var customers = new List<Customer> { new Customer { CustomerId = 1, FirstName = "John" } };
            _mockCustomerService.Setup(s => s.GetallCustomers()).ReturnsAsync(customers);

            // Act
            var result = await _controller.GetallCustomers();

            // Assert
            Assert.IsInstanceOf<ActionResult<IEnumerable<Customer>>>(result);
            Assert.AreEqual(customers, result.Value);
        }

        [Test]
        public async Task Getcustomersbyzipcode_ReturnsCustomers()
        {
            // Arrange
            var zipcode = "12345";
            var customers = new List<Customer> { new Customer { CustomerId = 1, FirstName = "John", ZipCode = zipcode } };
            _mockCustomerService.Setup(s => s.Getcustomersbyzipcode(zipcode)).ReturnsAsync(customers);

            // Act
            var result = await _controller.Getcustomersbyzipcode(zipcode);

            // Assert
            Assert.IsInstanceOf<ActionResult<List<Customer>>>(result);
            Assert.AreEqual(customers, result.Value);
        }

        [Test]
        public async Task Updatethewholedetails_ValidData_ReturnsUpdatedCustomer()
        {
            // Arrange
            var id = 1;
            var customer = new Customer { CustomerId = id, FirstName = "John" };
            _mockCustomerService.Setup(s => s.Updatethewholedetails(customer)).ReturnsAsync(customer);

            // Act
            var result = await _controller.Updatethewholedetails(id, customer);

            // Assert
            Assert.IsInstanceOf<ActionResult<Customer>>(result);
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(customer, okResult.Value);
        }

        [Test]
        public async Task Updatethewholedetails_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            var id = 1;
            var customer = new Customer { CustomerId = 2, FirstName = "John" };

            // Act
            var result = await _controller.Updatethewholedetails(id, customer);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result.Result);
        }

        [Test]
        public async Task AddnewCustomerObjectinDB_ReturnsNewCustomer()
        {
            // Arrange
            var customer = new Customer { CustomerId = 1, FirstName = "John" };
            _mockCustomerService.Setup(s => s.AddnewCustomerObjectinDB(customer)).ReturnsAsync(customer);

            // Act
            var result = await _controller.AddnewCustomerObjectinDB(customer);

            // Assert
            Assert.IsInstanceOf<ActionResult<Customer>>(result);
            Assert.AreEqual(customer, result.Value);
        }

        [Test]
        public async Task Getcustomersbyorderdate_ReturnsCustomers()
        {
            // Arrange
            var orderDate = new DateTime(2023, 1, 1);
            var customers = new List<Customer> { new Customer { CustomerId = 1, FirstName = "John" } };
            _mockCustomerService.Setup(s => s.Getcustomersbyorderdate(It.IsAny<DateOnly>())).ReturnsAsync(customers);

            // Act
            var result = await _controller.Getcustomersbyorderdate(orderDate);

            // Assert
            Assert.IsInstanceOf<ActionResult<List<Customer>>>(result);
            Assert.AreEqual(customers, result.Value);
        }

        [Test]
        public async Task Getthecustomerwhoplacedhighestorder_ReturnsCustomerName()
        {
            // Arrange
            var customerName = "John Doe";
            _mockCustomerService.Setup(s => s.Getthecustomerwhoplacedhighestorder()).ReturnsAsync(customerName);

            // Act
            var result = await _controller.Getthecustomerwhoplacedhighestorder();

            // Assert
            Assert.AreEqual(customerName, result);
        }

        [Test]
        public async Task Getdisplaycustomerbycityandorderbyname_ReturnsCustomers()
        {
            // Arrange
            var city = "New York";
            var customers = new List<Customer> { new Customer { CustomerId = 1, FirstName = "John", City = city } };
            _mockCustomerService.Setup(s => s.Getdisplaycustomerbycityandorderbyname(city)).ReturnsAsync(customers);

            // Act
            var result = await _controller.Getdisplaycustomerbycityandorderbyname(city);

            // Assert
            Assert.AreEqual(customers, result);
        }

        [Test]
        public async Task UpdateApproveStatus_ValidData_ReturnsNoContent()
        {
            // Arrange
            var dto = new ApproveStatusDto { Id = 1, ApproveStatus = true };
            _mockCustomerService.Setup(s => s.UpdateApproveStatusAsync(dto)).ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateApproveStatus(dto);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task UpdateApproveStatus_InvalidData_ReturnsBadRequest()
        {
            // Arrange
            ApproveStatusDto dto = null;

            // Act
            var result = await _controller.UpdateApproveStatus(dto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateApproveStatus_CustomerNotFound_ReturnsNotFound()
        {
            // Arrange
            var dto = new ApproveStatusDto { Id = 1, ApproveStatus = true };
            _mockCustomerService.Setup(s => s.UpdateApproveStatusAsync(dto)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateApproveStatus(dto);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}





//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using NUnit.Framework;
//using Moq;
//using Rohit_bike_store.Controllers;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;

//namespace TestProject
//{
//    [TestFixture]
//    public class CustomersControllerTests
//    {
//        private CustomersController _controller;
//        private Mock<ICustomer> _mockCustomerService;

//        [SetUp]
//        public void Setup()
//        {
//            _mockCustomerService = new Mock<ICustomer>();
//            _controller = new CustomersController(_mockCustomerService.Object);
//        }

//        [Test]
//        public async Task GetallCustomers_ReturnsAllCustomers()
//        {
//            // Arrange
//            var expectedCustomers = new List<Customer>
//            {
//                new Customer { CustomerId = 1, FirstName = "John" },
//                new Customer { CustomerId = 2, FirstName = "Jane" }
//            };
//            _mockCustomerService.Setup(s => s.GetallCustomers()).ReturnsAsync(expectedCustomers);

//            // Act
//            var result = await _controller.GetallCustomers();

//            // Assert
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(expectedCustomers.Count, result.Value.Count());
//        }

//        [Test]
//        public async Task GetallCustomers_ThrowsException_ReturnsInternalServerError()
//        {
//            // Arrange
//            _mockCustomerService.Setup(s => s.GetallCustomers()).ThrowsAsync(new Exception("Test exception"));

//            // Act
//            var result = await _controller.GetallCustomers();

//            // Assert
//            Assert.IsNull(result.Value);
//            Assert.IsInstanceOf<ObjectResult>(result.Result);
//            var statusCodeResult = result.Result as ObjectResult;
//            Assert.AreEqual(500, statusCodeResult.StatusCode);
//        }

//        [Test]
//        public async Task Getcustomersbyzipcode_ReturnsCustomers()
//        {
//            // Arrange
//            var zipcode = "12345";
//            var expectedCustomers = new List<Customer>
//            {
//                new Customer { CustomerId = 1, FirstName = "John", ZipCode = zipcode },
//                new Customer { CustomerId = 2, FirstName = "Jane", ZipCode = zipcode }
//            };
//            _mockCustomerService.Setup(s => s.Getcustomersbyzipcode(zipcode)).ReturnsAsync(expectedCustomers);

//            // Act
//            var result = await _controller.Getcustomersbyzipcode(zipcode);

//            // Assert
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(expectedCustomers.Count, result.Value.Count);
//        }

//        [Test]
//        public async Task Updatethewholedetails_ReturnsUpdatedCustomer()
//        {
//            // Arrange
//            var customer = new Customer { CustomerId = 1, FirstName = "John" };
//            _mockCustomerService.Setup(s => s.Updatethewholedetails(customer)).ReturnsAsync(customer);

//            // Act
//            var result = await _controller.Updatethewholedetails(customer);

//            // Assert
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(customer.CustomerId, result.Value.CustomerId);
//            Assert.AreEqual(customer.FirstName, result.Value.FirstName);
//        }

//        [Test]
//        public async Task AddnewCustomerObjectinDB_ReturnsNewCustomer()
//        {
//            // Arrange
//            var newCustomer = new Customer { FirstName = "New Customer" };
//            var savedCustomer = new Customer { CustomerId = 1, FirstName = "New Customer" };
//            _mockCustomerService.Setup(s => s.AddnewCustomerObjectinDB(newCustomer)).ReturnsAsync(savedCustomer);

//            // Act
//            var result = await _controller.AddnewCustomerObjectinDB(newCustomer);

//            // Assert
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(savedCustomer.CustomerId, result.Value.CustomerId);
//            Assert.AreEqual(savedCustomer.FirstName, result.Value.FirstName);
//        }

//        [Test]
//        public async Task Getcustomersbyorderdate_ReturnsCustomers()
//        {
//            // Arrange
//            var orderDate = new DateTime(2023, 5, 1);
//            var expectedCustomers = new List<Customer>
//            {
//                new Customer { CustomerId = 1, FirstName = "John" },
//                new Customer { CustomerId = 2, FirstName = "Jane" }
//            };
//            _mockCustomerService.Setup(s => s.Getcustomersbyorderdate(It.IsAny<DateOnly>())).ReturnsAsync(expectedCustomers);

//            // Act
//            var result = await _controller.Getcustomersbyorderdate(orderDate);

//            // Assert
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(expectedCustomers.Count, result.Value.Count);
//        }

//        [Test]
//        public async Task Getthecustomerwhoplacedhighestorder_ReturnsCustomerName()
//        {
//            // Arrange
//            var expectedCustomerName = "John";
//            _mockCustomerService.Setup(s => s.Getthecustomerwhoplacedhighestorder()).ReturnsAsync(expectedCustomerName);

//            // Act
//            var result = await _controller.Getthecustomerwhoplacedhighestorder();

//            // Assert
//            Assert.IsNotNull(result.Result);
//            var okResult = result.Result as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            Assert.AreEqual(expectedCustomerName, okResult.Value);
//        }

//        [Test]
//        public async Task Getdisplaycustomerbycityandorderbyname_ReturnsCustomers()
//        {
//            // Arrange
//            var city = "New York";
//            var expectedCustomers = new List<Customer>
//            {
//                new Customer { CustomerId = 1, FirstName = "Alice", City = city },
//                new Customer { CustomerId = 2, FirstName = "Bob", City = city }
//            };
//            _mockCustomerService.Setup(s => s.Getdisplaycustomerbycityandorderbyname(city)).ReturnsAsync(expectedCustomers);

//            // Act
//            var result = await _controller.Getdisplaycustomerbycityandorderbyname(city);

//            // Assert
//            Assert.IsNotNull(result.Value);
//            Assert.AreEqual(expectedCustomers.Count, result.Value.Count);
//            Assert.AreEqual(expectedCustomers[0].FirstName, result.Value[0].FirstName);
//            Assert.AreEqual(expectedCustomers[1].FirstName, result.Value[1].FirstName);
//        }
//    }
//}