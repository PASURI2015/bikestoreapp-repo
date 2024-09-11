using Moq;
using NUnit.Framework;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject
{
    [TestFixture]
    public class StocksControllerTests
    {
        private Mock<IStock> _mockStockService;
        private StocksController _controller;

        [SetUp]
        public void Setup()
        {
            _mockStockService = new Mock<IStock>();
            _controller = new StocksController(_mockStockService.Object);
        }

        [Test]
        public async Task AddStock_ShouldReturnOk_WhenSuccessful()
        {
            // Arrange
            var stockDto = new StockDto { StoreId = 1, ProductId = 1, Quantity = 100 };
            _mockStockService
                .Setup(service => service.AddStockAsync(stockDto))
                .ReturnsAsync("Stock added successfully");

            // Act
            var response = await _controller.AddStock(stockDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            var result = (OkObjectResult)response;
            Assert.AreEqual("Stock added successfully", result.Value);
        }

        [Test]
        public async Task GetAllStocks_ShouldReturnAllStocks()
        {
            // Arrange
            var stocks = new List<StockDto>
            {
                new StockDto { StoreId = 1, ProductId = 1, Quantity = 100 },
                new StockDto { StoreId = 1, ProductId = 2, Quantity = 150 }
            };

            _mockStockService
                .Setup(service => service.GetAllStocksAsync())
                .ReturnsAsync(stocks);

            // Act
            var response = await _controller.GetAllStocks();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(response);
            var result = (OkObjectResult)response;
            var returnedStocks = (IEnumerable<StockDto>)result.Value;
            Assert.AreEqual(2, returnedStocks.Count());
        }

        [Test]
        public async Task UpdateStock_ShouldReturnUpdatedStock_WhenSuccessful()
        {
            // Arrange
            var stockdto = new StockDto { Quantity = 200 };
            var updatedStock = new StockDto { StoreId = 1, ProductId = 1, Quantity = 200 };

            _mockStockService
                .Setup(service => service.UpdateStockAsync(1, 1, stockdto))
                .ReturnsAsync(updatedStock);

            // Act
            var response = await _controller.UpdateStock(1, 1, stockdto);

            // Assert
            Assert.IsInstanceOf<StockDto>(response);
            var result = (StockDto)response;
            Assert.AreEqual(200, result.Quantity);
        }

        [Test]
        public async Task UpdateStock_ShouldReturnNull_WhenStockNotFound()
        {
            // Arrange
            var stockdto = new StockDto { Quantity = 200 };

            _mockStockService
                .Setup(service => service.UpdateStockAsync(1, 1, stockdto))
                .ReturnsAsync((StockDto)null);

            // Act
            var response = await _controller.UpdateStock(1, 1, stockdto);

            // Assert
            Assert.IsNull(response);
        }

        [Test]
        public async Task GetProductsWithMinimumStock_ShouldReturnProducts()
        {
            // Arrange
            var productDto = new ProductDto
            {
                ProductId = 1,
                ProductName = "Sample Product"
            };

            _mockStockService.Setup(s => s.GetProductsWithMinimumStockAsync())
                             .ReturnsAsync(productDto);

            // Act
            var result = await _controller.GetProductsWithMinimumStock();

            // Assert
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);

            var returnedProductDto = okResult.Value as ProductDto;
            Assert.IsNotNull(returnedProductDto);
            Assert.AreEqual(productDto.ProductId, returnedProductDto.ProductId);
            Assert.AreEqual(productDto.ProductName, returnedProductDto.ProductName);            
        }
    }
}