using NUnit.Framework;
using Rohit_bike_store.Services;
using Rohit_bike_store.Models;
using Rohit_bike_store.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Configuration;

namespace TestProject
{
    [TestFixture]
    public class ProductServicesTests
    {
        private RohitBikeStoreContext _context;
        private ProductServices _productServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RohitBikeStoreContext(options);
            _productServices = new ProductServices(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var brands = new List<Brand>
            {
                new Brand { BrandId = 1, BrandName = "Brand 1" },
                new Brand { BrandId = 2, BrandName = "Brand 2" }
            };

            var categories = new List<Category>
            {
                new Category { CategoryId = 1, CategoryName = "Category 1" },
                new Category { CategoryId = 2, CategoryName = "Category 2" }
            };

            var products = new List<Product>
            {
                new Product { ProductId = 1, ProductName = "Product 1", BrandId = 1, CategoryId = 1, ModelYear = 2023, ListPrice = 100 },
                new Product { ProductId = 2, ProductName = "Product 2", BrandId = 2, CategoryId = 2, ModelYear = 2023, ListPrice = 200 }
            };

            var stores = new List<Store>
            {
                new Store { StoreId = 1, StoreName = "Store 1" },
                new Store { StoreId = 2, StoreName = "Store 2" }
            };

            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, FirstName = "John", LastName = "Doe", Email="xys@gmail.com" },
                new Customer { CustomerId = 2, FirstName = "Jane", LastName = "Doe", Email = "wer@gmail.com" }
            };

            var orders = new List<Order>
            {
                new Order { OrderId = 1, CustomerId = 1, StoreId = 1, OrderStatus = 1, OrderDate = DateOnly.FromDateTime(DateTime.UtcNow) },
                new Order { OrderId = 2, CustomerId = 2, StoreId = 2, OrderStatus = 1, OrderDate = DateOnly.FromDateTime(DateTime.UtcNow) }
            };

            var orderItems = new List<OrderItem>
            {
                new OrderItem { OrderId = 1, ItemId = 1, ProductId = 1, Quantity = 2 },
                new OrderItem { OrderId = 2, ItemId = 2, ProductId = 2, Quantity = 1 }
            };

            _context.Brands.AddRange(brands);
            _context.Categories.AddRange(categories);
            _context.Products.AddRange(products);
            _context.Stores.AddRange(stores);
            _context.Customers.AddRange(customers);
            _context.Orders.AddRange(orders);
            _context.OrderItems.AddRange(orderItems);
            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Test]
        public async Task AddProduct_ValidProduct_ReturnsAddedProduct()
        {
            // Arrange
            var newProduct = new Product { ProductName = "New Product", BrandId = 1, CategoryId = 1, ModelYear = 2023, ListPrice = 300 };

            // Act
            var result = await _productServices.AddProduct(newProduct);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo("New Product"));
            Assert.That(_context.Products.Count(), Is.EqualTo(3));
        }

        [Test]
        public void AddProduct_NullProduct_ThrowsArgumentNullException()
        {
            // Act & Assert
            Assert.ThrowsAsync<ArgumentNullException>(() => _productServices.AddProduct(null));
        }

        [Test]
        public async Task GetAllProducts_ReturnsAllProducts()
        {
            // Act
            var result = await _productServices.GetAllProducts();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllProducts_NoProductsExist_ReturnsEmptyList()
        {
            // Arrange
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productServices.GetAllProducts();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetNumberOfProductsByEachStore_ReturnsCorrectData()
        {
            // Act
            var result = await _productServices.GetNumberOfProductsByEachStore();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
            var resultList = result.ToList();
            Assert.That(resultList[0].GetType().GetProperty("StoreName").GetValue(resultList[0], null), Is.EqualTo("Store 1"));
            Assert.That(resultList[0].GetType().GetProperty("NumberOfProducts").GetValue(resultList[0], null), Is.EqualTo(2));
            Assert.That(resultList[1].GetType().GetProperty("StoreName").GetValue(resultList[1], null), Is.EqualTo("Store 2"));
            Assert.That(resultList[1].GetType().GetProperty("NumberOfProducts").GetValue(resultList[1], null), Is.EqualTo(1));
        }

        [Test]
        public async Task GetNumberOfProductsByEachStore_NoOrderItems_ReturnsEmptyList()
        {
            // Arrange
            _context.OrderItems.RemoveRange(_context.OrderItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productServices.GetNumberOfProductsByEachStore();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetProductByBrandName_ValidBrandName_ReturnsProduct()
        {
            // Act
            var result = await _productServices.GetProductByBrandName("Brand 1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo("Product 1"));
        }

        [Test]
        public async Task GetProductByBrandName_InvalidBrandName_ReturnsNull()
        {
            // Act
            var result = await _productServices.GetProductByBrandName("Invalid Brand");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetProductByCategoryName_ValidCategoryName_ReturnsProducts()
        {
            // Act
            var result = await _productServices.GetProductByCategoryName("Category 1");

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ProductName, Is.EqualTo("Product 1"));
        }

        [Test]
        public async Task GetProductByCategoryName_InvalidCategoryName_ReturnsEmptyList()
        {
            // Act
            var result = await _productServices.GetProductByCategoryName("Invalid Category");

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetProductByCustomerId_ValidCustomerId_ReturnsProducts()
        {
            // Act
            var result = await _productServices.GetProductByCustomerId(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].ProductName, Is.EqualTo("Product 1"));
        }

        [Test]
        public async Task GetProductByCustomerId_InvalidCustomerId_ReturnsEmptyList()
        {
            // Act
            var result = await _productServices.GetProductByCustomerId(999);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetProductByModelYear_ValidModelYear_ReturnsProducts()
        {
            // Act
            var result = await _productServices.GetProductByModelYear(2023);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetProductByModelYear_InvalidModelYear_ReturnsEmptyList()
        {
            // Act
            var result = await _productServices.GetProductByModelYear(1900);

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetProductNameCategoryNameBrandName_ReturnsCorrectData()
        {
            // Act
            var result = await _productServices.GetProductNameCategoryNameBrandName();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(new List<string> { "Product 1", "Brand 1", "Category 1" }));
            Assert.That(result[1], Is.EqualTo(new List<string> { "Product 2", "Brand 2",  "Category 2" }));
        }

        [Test]
        public async Task GetProductNameCategoryNameBrandName_NoProducts_ReturnsEmptyList()
        {
            // Arrange
            _context.Products.RemoveRange(_context.Products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productServices.GetProductNameCategoryNameBrandName();

            // Assert
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task ProductPurchasedByMaximumCustomer_ReturnsCorrectProduct()
        {
            // Act
            var result = await _productServices.ProductPurchasedByMaximumCustomer();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo("Product 1"));
        }

        [Test]
        public async Task ProductPurchasedByMaximumCustomer_NoOrderItems_ReturnsNull()
        {
            // Arrange
            _context.OrderItems.RemoveRange(_context.OrderItems);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productServices.ProductPurchasedByMaximumCustomer();

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateProduct_ValidProduct_ReturnsUpdatedProduct()
        {
            // Arrange
            var updatedProduct = new Product { ProductId = 1, ProductName = "Updated Product", BrandId = 1, CategoryId = 1, ModelYear = 2024, ListPrice = 150 };

            // Act
            var result = await _productServices.UpdateProduct(updatedProduct);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo("Updated Product"));
            Assert.That(result.ModelYear, Is.EqualTo(2024));
            Assert.That(result.ListPrice, Is.EqualTo(150));
        }

        [Test]
        public async Task UpdateProduct_NonExistentProduct_ReturnsNull()
        {
            // Arrange
            var nonExistentProduct = new Product { ProductId = 999, ProductName = "Non-existent Product" };

            // Act
            var result = await _productServices.UpdateProduct(nonExistentProduct);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task UpdateParticularProductInfo_ValidProductDto_ReturnsUpdatedProduct()
        {
            // Arrange
            var productDto = new ProductDto { ProductId = 1, ProductName = "Updated Product", ModelYear = 2024, ListPrice = 150 };

            // Act
            var result = await _productServices.UpdateParticularProductInfo(productDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ProductName, Is.EqualTo("Updated Product"));
            Assert.That(result.ModelYear, Is.EqualTo(2024));
            Assert.That(result.ListPrice, Is.EqualTo(150));
        }

        [Test]
        public async Task UpdateParticularProductInfo_NonExistentProduct_ReturnsNull()
        {
            // Arrange
            var productDto = new ProductDto { ProductId = 999, ProductName = "Non-existent Product" };

            // Act
            var result = await _productServices.UpdateParticularProductInfo(productDto);

            // Assert
            Assert.That(result, Is.Null);
        }
    }
}


