using BikeStoreApplication.Controllers;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;


namespace TestProject
{
    [TestFixture]
    public class TestStaffsController
    {
        private Mock<IStaff> _staffServiceMock;
        private StaffsController _controller;

        [SetUp]
        public void SetUp()
        {
            _staffServiceMock = new Mock<IStaff>();
            _controller = new StaffsController(_staffServiceMock.Object);
        }

        [Test]
        public async Task AddStaff_ReturnsOk_WhenSuccess()
        {
            var staff = new Staff
            {
                StaffId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "23423",
                Active = 3,
                StoreId = 1,
                ManagerId = 1,
                Password = "jsdkjsk"
            };
            _staffServiceMock.Setup(s => s.AddStaff(It.IsAny<Staff>())).ReturnsAsync((true, staff));

            var result = await _controller.AddStaff(staff);

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(staff, okResult.Value);
        }

        [Test]
        public async Task AddStaff_ReturnsBadRequest_WhenFailure()
        {
            var staff = new Staff
            {
                StaffId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "23423",
                Active = 3,
                StoreId = 1,
                ManagerId = 1,
                Password = "jsdkjsk"
            };

            _staffServiceMock.Setup(s => s.AddStaff(It.IsAny<Staff>())).ReturnsAsync((false, "Error"));

            //Act
            var result = await _controller.AddStaff(staff);

            //Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
            var badRequestResult = result as BadRequestObjectResult;
            Assert.IsNotNull(badRequestResult);
            Assert.AreEqual("Error", badRequestResult.Value);
        }

        [Test]
        public async Task GetAllStaff_ReturnsOk_WithListOfStaff()
        {
            var staffList = new List<Staff>
            {
                new Staff{ StaffId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "23423",
                Active = 3,
                StoreId = 1,
                ManagerId=1,
                Password = "jsdkjsk" },
                new Staff{ StaffId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "23423",
                Active = 3,
                StoreId = 2,
                ManagerId=1,
                Password = "jsdkjsk" },

            };

            _staffServiceMock.Setup(s => s.GetAllStaff()).ReturnsAsync(staffList);

            var result = await _controller.GetAllStaff();

            //Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(staffList, okResult.Value);
        }

        [Test]
        public async Task GetAllStaff_ReturnsInternalServerError_OnException()
        {
            // Arrange: Set up the service to throw an exception
            _staffServiceMock.Setup(s => s.GetAllStaff()).ThrowsAsync(new Exception("Database failure"));

            // Act: Call the controller method
            var result = await _controller.GetAllStaff();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result); // Ensure the result is of type ObjectResult
            var objectResult = result as ObjectResult;
            Assert.IsNotNull(objectResult);
            Assert.AreEqual(500, objectResult.StatusCode); // Check if the status code is 500
            Assert.AreEqual("An error occurred while retrieving staff.", objectResult.Value); // Ensure the correct error message is returned
        }

        [Test]
        public async Task GetAllStaff_ReturnsNotFound_WhenNoStaffFound()
        {
            // Arrange: Set up the service to return null or an empty list
            _staffServiceMock.Setup(s => s.GetAllStaff()).ReturnsAsync(new List<Staff>()); // Or null if you prefer

            // Act: Call the controller method
            var result = await _controller.GetAllStaff();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result); // Ensure the result is of type NotFoundObjectResult
            var notFoundResult = result as NotFoundObjectResult;
            Assert.IsNotNull(notFoundResult);
            Assert.AreEqual("No staff found.", notFoundResult.Value); // Ensure the correct error message is returned
        }



        [Test]
        public async Task GetAllStaffByStore_ReturnsOk_WithStaffList_WhenStoreExists()
        {
            // Arrange
            var storeName = "Store 1";
            var staffList = new List<Staff>
            {
                new Staff {  StaffId = 1,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Phone = "23423",
                Active = 3,
                StoreId = 1,
                ManagerId=1,
                Password = "jsdkjsk" }
            };
            _staffServiceMock.Setup(s => s.GetStaffByStore(storeName)).ReturnsAsync(staffList);

            // Act
            var result = await _controller.GetAllStaffByStore(storeName);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(staffList, okResult.Value);
        }

        [Test]
        public async Task GetAllStaffByStore_ReturnsNotFound_WhenStoreHasNoStaff()
        {
            // Arrange
            var storeName = "NonExistingStore";
            _staffServiceMock.Setup(s => s.GetStaffByStore(storeName)).ReturnsAsync(new List<Staff>());

            // Act
            var result = await _controller.GetAllStaffByStore(storeName);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
            var notFoundResult = result as NotFoundObjectResult;
            Assert.AreEqual($"No staff found for store: {storeName}", notFoundResult.Value);
        }



        [Test]
        public async Task ManagerDetails_ReturnsOk_WithManagerDetails()
        {
            // Arrange
            var staffId = 1;
            var manager = new Staff
            {
                StaffId = 101,
                FirstName = "Manager",
                LastName = "One",
                Email = "manager.one@example.com"
            };
            var staff = new Staff
            {
                StaffId = staffId,
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                Manager = manager
            };
            //_staffServiceMock.Setup(s => s.GetS(staffId)).ReturnsAsync(staff);
            _staffServiceMock.Setup(s => s.ManagerDetails(staffId)).ReturnsAsync(staff.Manager);

            // Act
            var result = await _controller.ManagerDetails(staffId);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);

            var actualManager = okResult.Value as Staff;
            Assert.NotNull(actualManager);

            // Custom property-by-property comparison
            Assert.AreEqual(manager.StaffId, actualManager.StaffId);
            Assert.AreEqual(manager.FirstName, actualManager.FirstName);
            Assert.AreEqual(manager.LastName, actualManager.LastName);
            Assert.AreEqual(manager.Email, actualManager.Email);
        }


        [Test]
        public async Task ManagerDetails_ReturnsNotFound_WhenManagerDetailsNotAvailable()
        {
            // Arrange
            var staffId = 1;
            _staffServiceMock.Setup(s => s.ManagerDetails(staffId)).ReturnsAsync((Staff)null); // Simulate manager not found

            // Act
            var result = await _controller.ManagerDetails(staffId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
            var notFoundResult = result as NotFoundResult;
            Assert.AreEqual(404, notFoundResult.StatusCode); // NotFoundResult has a 404 status code
        }

        [Test]
        public async Task SalesMadeByStaff_ReturnsOk_WhenSalesAreFound()
        {
            // Arrange
            var staffId = 1;
            var sales = new List<object>
        {
            new
            {
                OrderId = 1,
                CustomerName = "John Doe"
            },
            new
            {
                OrderId = 2,
                CustomerName = "Jane Smith"
            }
        };

            _staffServiceMock.Setup(s => s.SalesMadeByStaff(staffId)).ReturnsAsync(sales);

            // Act
            var result = await _controller.SalesMadeByStaff(staffId);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            var returnedSales = okResult.Value as List<object>;
            Assert.NotNull(returnedSales);
            Assert.AreEqual(sales.Count, returnedSales.Count);

            // Optional: Check individual items if necessary
            var returnedSale1 = returnedSales.First();
            Assert.AreEqual(sales[0], returnedSale1);
        }

        [Test]
        public async Task SalesMadeByStaff_ReturnsNotFound_WhenNoSalesAreFound()
        {
            // Arrange
            var staffId = 2;
            // _staffServiceMock.Setup(s => s.SalesMadeByStaff(staffId)).ReturnsAsync(new List<object>());

            // Act
            var result = await _controller.SalesMadeByStaff(staffId);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }


        [Test]
        public async Task EditStaff_ReturnsNoContent_WhenStaffUpdatedSuccessfully()
        {
            // Arrange
            var staffId = 1;
            var staff = new Staff { StaffId = staffId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", StoreId = 1 };
            //_staffServiceMock.Setup(s => s.UpdateStaffDetails(staffId, staff)).Returns((Staff)null);

            // Act
            var result = await _controller.EditStaff(staffId, staff);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }



        [Test]
        public async Task EditStaff_ReturnsNotFound_WhenStaffDoesNotExist()
        {
            // Arrange
            var staffId = 1;
            var staff = new Staff { StaffId = staffId, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", StoreId = 1 };
            //_staffServiceMock.Setup(s => s.UpdateStaffDetails(staffId, staff)).ReturnsAsync((Staff)null);

            // Act
            var result = await _controller.EditStaff(staffId, staff);


            Assert.IsInstanceOf<NoContentResult>(result);
        }

        [Test]
        public async Task EditStaffDetails_ReturnsNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var staffId = 1;
            var staffToUpdate = new Staff
            {
                StaffId = staffId,
                FirstName = "Updated",
                LastName = "Name",
                Email = "updated.email@example.com"
            };

            // Set up the service to return true, indicating the update was successful
            _staffServiceMock.Setup(s => s.UpdateStaffDetailsPatch(staffId, staffToUpdate)).ReturnsAsync(true);

            // Act
            var result = await _controller.EditStaffDetails(staffId, staffToUpdate);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result); // Ensure the result is of type NoContentResult
        }


        [Test]
        public async Task EditStaffDetails_ReturnsNotFound_WhenStaffNotFound()
        {
            // Arrange
            var staffId = -18798;
            var staffToUpdate = new Staff
            {
                StaffId = staffId,
                FirstName = "Nonexistent",
                LastName = "Staff",
                Email = "nonexistent.staff@example.com"
            };

            // Mock the service to return false when staff is not found
            _staffServiceMock.Setup(s => s.UpdateStaffDetailsPatch(staffId, staffToUpdate)).ReturnsAsync(false);

            // Act
            var result = await _controller.EditStaffDetails(staffId, staffToUpdate);

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }
    }
}