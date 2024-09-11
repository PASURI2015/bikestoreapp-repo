using NUnit.Framework;
using Moq;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.Services;
using Rohit_bike_store.Models;
using Rohit_bike_store.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace TestProject
{
    [TestFixture]
    public class ProductsControllerTests
    {
        private Mock<IProduct> _mockProductService;
        private ProductsController _controller;

        [SetUp]
        public void Setup()
        {
            _mockProductService = new Mock<IProduct>();
            _controller = new ProductsController(_mockProductService.Object);
        }

        [Test]
        public async Task GetAllProducts_ReturnsOkResult_WithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            _mockProductService.Setup(s => s.GetAllProducts()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(products, okResult.Value);
        }

        [Test]
        public async Task GetAllProducts_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetAllProducts()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetAllProducts();

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task GetProductByBrandName_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { ProductId = 1, ProductName = "TestBrand" };
            _mockProductService.Setup(s => s.GetProductByBrandName("TestBrand")).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductByBrandName("TestBrand");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(product, okResult.Value);
        }

        [Test]
        public async Task GetProductByBrandName_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByBrandName("NonExistentBrand")).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductByBrandName("NonExistentBrand");

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetProductByCategoryName_ReturnsOkResult_WithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            _mockProductService.Setup(s => s.GetProductByCategoryName("TestCategory")).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProductByCategoryName("TestCategory");

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(products, okResult.Value);
        }

        [Test]
        public async Task GetProductByCategoryName_ReturnsNotFound_WhenNoProductsExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByCategoryName("NonExistentCategory")).ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetProductByCategoryName("NonExistentCategory");

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetProductByModelyear_ReturnsOkResult_WithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            _mockProductService.Setup(s => s.GetProductByModelYear(2023)).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProductByModelyear(2023);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(products, okResult.Value);
        }

        [Test]
        public async Task GetProductByModelyear_ReturnsNotFound_WhenNoProductsExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByModelYear(1900)).ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetProductByModelyear(1900);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetProductByCustomerId_ReturnsOkResult_WithProducts()
        {
            // Arrange
            var products = new List<Product> { new Product(), new Product() };
            _mockProductService.Setup(s => s.GetProductByCustomerId(1)).ReturnsAsync(products);

            // Act
            var result = await _controller.GetProductByCustomerId(1);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(products, okResult.Value);
        }

        [Test]
        public async Task GetProductByCustomerId_ReturnsNotFound_WhenNoProductsExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductByCustomerId(999)).ReturnsAsync(new List<Product>());

            // Act
            var result = await _controller.GetProductByCustomerId(999);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetProductNameCategoryNameBrandName_ReturnsOkResult_WithDetails()
        {
            // Arrange
            var details = new List<List<string>>
    {
        new List<string> { "TestProduct", "TestCategory", "TestBrand" }
    };
            _mockProductService.Setup(s => s.GetProductNameCategoryNameBrandName()).ReturnsAsync(details);

            // Act
            var result = await _controller.GetProductNameCategoryNameBrandName();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(details, okResult.Value);
        }

        [Test]
        public async Task GetProductNameCategoryNameBrandName_ReturnsNotFound_WhenNoDetailsExist()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetProductNameCategoryNameBrandName());

            // Act
            var result = await _controller.GetProductNameCategoryNameBrandName();

            // Assert
            Assert.IsInstanceOf<NotFoundObjectResult>(result);
        }

        [Test]
        public async Task GetProductPurchasedByMaxCustomer_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { ProductId = 1, ProductName = "MostPurchased" };
            _mockProductService.Setup(s => s.ProductPurchasedByMaximumCustomer()).ReturnsAsync(product);

            // Act
            var result = await _controller.GetProductPurchasedByMaxCustomer();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(product, okResult.Value);
        }

        [Test]
        public async Task GetProductPurchasedByMaxCustomer_ReturnsNotFound_WhenNoProductExists()
        {
            // Arrange
            _mockProductService.Setup(s => s.ProductPurchasedByMaximumCustomer()).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.GetProductPurchasedByMaxCustomer();

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }

        [Test]
        public async Task GetnumberofProductsSoldByeachStore_ReturnsResults()
        {
            // Arrange
            var storeProducts = new List<object> { new { StoreName = "Store1", ProductCount = 10 } };
            _mockProductService.Setup(s => s.GetNumberOfProductsByEachStore()).ReturnsAsync(storeProducts);

            // Act
            var result = await _controller.GetnumberofProductsSoldByeachStore();

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(storeProducts, result);
        }

        [Test]
        public async Task GetnumberofProductsSoldByeachStore_ReturnsNull_WhenExceptionOccurs()
        {
            // Arrange
            _mockProductService.Setup(s => s.GetNumberOfProductsByEachStore()).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetnumberofProductsSoldByeachStore();

            // Assert
            Assert.IsNull(result);
        }

        [Test]
        public async Task AddProduct_ReturnsCreatedAtAction_WhenProductIsAdded()
        {
            // Arrange
            var newProduct = new Product { ProductId = 1, ProductName = "New Product" };
            _mockProductService.Setup(s => s.AddProduct(It.IsAny<Product>()));

            // Act
            var result = await _controller.AddProduct(newProduct);

            // Assert
            Assert.IsInstanceOf<CreatedAtActionResult>(result);
            var createdAtActionResult = result as CreatedAtActionResult;
            Assert.AreEqual("GetAllProducts", createdAtActionResult.ActionName);
            Assert.AreEqual(1, createdAtActionResult.RouteValues["id"]);
        }

        [Test]
        public async Task AddProduct_ReturnsBadRequest_WhenExceptionOccurs()
        {
            // Arrange
            var newProduct = new Product { ProductId = 1, ProductName = "New Product" };
            _mockProductService.Setup(s => s.AddProduct(It.IsAny<Product>())).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.AddProduct(newProduct);

            // Assert
            Assert.IsInstanceOf<BadRequestObjectResult>(result);
        }

        [Test]
        public async Task UpdateProduct_ReturnsOkResult_WhenProductIsUpdated()
        {
            // Arrange
            var product = new Product { ProductId = 1, ProductName = "Updated Product" };
            _mockProductService.Setup(s => s.UpdateProduct(It.IsAny<Product>())).ReturnsAsync(product);

            // Act
            var result = await _controller.UpdateProduct(1, product);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(product, okResult.Value);
        }

        [Test]
        public async Task UpdateProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var product = new Product { ProductId = 999, ProductName = "Non-existent Product" };
            _mockProductService.Setup(s => s.UpdateProduct(It.IsAny<Product>())).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateProduct(999, product);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result);
        }

        [Test]
        public async Task UpdateParticularInfo_ReturnsOkResult_WhenProductIsUpdated()
        {
            // Arrange
            var productDto = new ProductDto { ProductId = 1, ProductName = "Updated Product" };
            var updatedProduct = new Product { ProductId = 1, ProductName = "Updated Product" };
            _mockProductService.Setup(s => s.UpdateParticularProductInfo(It.IsAny<ProductDto>())).ReturnsAsync(updatedProduct);

            // Act
            var result = await _controller.UpdateParticularInfo(productDto);

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            Assert.AreEqual(updatedProduct, okResult.Value);
        }

        [Test]
        public async Task UpdateParticularInfo_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var productDto = new ProductDto { ProductId = 999, ProductName = "Non-existent Product" };
            _mockProductService.Setup(s => s.UpdateParticularProductInfo(It.IsAny<ProductDto>())).ReturnsAsync((Product)null);

            // Act
            var result = await _controller.UpdateParticularInfo(productDto);

            // Assert
            Assert.IsInstanceOf<NotFoundResult>(result.Result);
        }
    }
}