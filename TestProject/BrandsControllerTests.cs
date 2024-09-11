using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Services;

namespace TestProject
{
    [TestFixture]
    public class BrandsControllerTests
    {
        private Mock<IBrand> _mockBrandService;
        private BrandsController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockBrandService = new Mock<IBrand>();
            _controller = new BrandsController(_mockBrandService.Object);
        }
        
        [Test]
        public async Task Post_ValidDto_ReturnsOkResult()
        {
            // Arrange
            var brandDto = new BrandDto { BrandId = 1, BrandName = "NewBrand" };
            var insertedBrand = new BrandDto { BrandId = 1, BrandName = "NewBrand" };
            _mockBrandService.Setup(service => service.Post(It.IsAny<BrandDto>())).ReturnsAsync(insertedBrand);

            // Act
            var result = await _controller.Post(brandDto) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result, "Result should be an OkObjectResult.");
            Assert.AreEqual(200, result.StatusCode, "Status code should be 200 OK.");

            var response = result.Value;
            Assert.IsNotNull(response, "Response should not be null.");

            // Use reflection to access properties
            var responseType = response.GetType();

            var messageProperty = responseType.GetProperty("Message");
            Assert.IsNotNull(messageProperty, "Response should contain 'Message' property.");
            var message = messageProperty.GetValue(response) as string;
            Assert.AreEqual("Record Added Successfully.", message, "Message should match.");

            var insertedRecordProperty = responseType.GetProperty("InsertedRecord");
            Assert.IsNotNull(insertedRecordProperty, "Response should contain 'InsertedRecord' property.");
            var insertedRecord = insertedRecordProperty.GetValue(response) as BrandDto;
            Assert.IsNotNull(insertedRecord, "InsertedRecord should not be null.");
            Assert.AreEqual(insertedBrand.BrandId, insertedRecord.BrandId, "BrandId should match.");
            Assert.AreEqual(insertedBrand.BrandName, insertedRecord.BrandName, "BrandName should match.");
        }

        // Negative Test: PUT with invalid ID
        [Test]
        public async Task Put_InvalidId_ReturnsNotFound()
        {
            // Arrange
            var brandDto = new BrandDto { BrandId = 1, BrandName = "UpdatedBrand" };
            _mockBrandService.Setup(service => service.Put(It.IsAny<int>(), It.IsAny<BrandDto>())).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await _controller.Put(1, brandDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        // Negative Test: POST with invalid DTO (e.g., missing required fields)
        [Test]
        public async Task Post_InvalidDto_ReturnsBadRequest()
        {
            // Arrange
            var invalidBrandDto = new BrandDto(); // Create an invalid DTO here
            _mockBrandService.Setup(service => service.Post(It.IsAny<BrandDto>())).ThrowsAsync(new ArgumentException("Invalid DTO"));

            // Act
            var result = await _controller.Post(invalidBrandDto);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        // Negative Test: GET returns empty list
        [Test]
        public async Task GetAllBrands_NoBrands_ReturnsOkWithEmptyList()
        {
            // Arrange
            _mockBrandService.Setup(service => service.GetAllBrands()).ReturnsAsync(new List<BrandDto>());

            // Act
            var result = await _controller.GetAllBrands();

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnValue = okResult.Value as IEnumerable<BrandDto>;
            Assert.IsNotNull(returnValue);
            Assert.IsEmpty(returnValue);
        }

        // Negative Test: GET returns server error (simulating an internal failure)
        [Test]
        public async Task GetAllBrands_InternalError_ReturnsServerError()
        {
            // Arrange
            _mockBrandService.Setup(service => service.GetAllBrands()).ThrowsAsync(new System.Exception("Something went wrong"));

            // Act
            var result = await _controller.GetAllBrands();

            // Assert
            var statusResult = result.Result as ObjectResult;
            Assert.IsNotNull(statusResult);
            Assert.AreEqual(500, statusResult.StatusCode);
        }
    }

}





