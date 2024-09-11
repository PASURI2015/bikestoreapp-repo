using NUnit.Framework;
using Moq;
using AutoMapper;
using Rohit_bike_store.Services;
using Rohit_bike_store.Models;
using Rohit_bike_store.DTO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace TestProject
{
    [TestFixture]
    public class StockServicesTests
    {
        private Mock<RohitBikeStoreContext> _mockContext;
        private Mock<IMapper> _mockMapper;
        private StockServices _stockServices;

        [SetUp]
        public void Setup()
        {
            _mockContext = new Mock<RohitBikeStoreContext>();
            _mockMapper = new Mock<IMapper>();
            _stockServices = new StockServices(_mockContext.Object, _mockMapper.Object);
        }

        [Test]
        public async Task AddStockAsync_ValidStock_ReturnsSuccessMessage()
        {
            // Arrange
            var stockDto = new StockDto { StoreId = 1, ProductId = 1, Quantity = 10 };
            var mockDbSet = new Mock<DbSet<Stock>>();
            _mockContext.Setup(c => c.Stocks).Returns(mockDbSet.Object);

            // Act
            var result = await _stockServices.AddStockAsync(stockDto);

            // Assert
            Assert.That(result, Is.EqualTo("Record Added Successfully!!"));
            mockDbSet.Verify(m => m.Add(It.IsAny<Stock>()), Times.Once());
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }

        [Test]
        public void AddStockAsync_ExceptionThrown_ThrowsApplicationException()
        {
            // Arrange
            var stockDto = new StockDto { StoreId = 1, ProductId = 1, Quantity = 10 };
            _mockContext.Setup(c => c.Stocks).Throws(new Exception("Database error"));

            // Act & Assert
            var ex = Assert.ThrowsAsync<ApplicationException>(() => _stockServices.AddStockAsync(stockDto));
            Assert.That(ex.Message, Is.EqualTo("An error occurred while adding stock."));
        }

        [Test]
        public async Task GetAllStocksAsync_ReturnsAllStocks()
        { // Arrange
            var stockList = new List<Stock>
    {
        new Stock { StoreId = 1, ProductId = 1, Quantity = 10 },
        new Stock { StoreId = 1, ProductId = 2, Quantity = 20 }
    };
            var mockDbSet = MockDbSet(stockList);
            _mockContext.Setup(c => c.Stocks).Returns(mockDbSet.Object);

            // Act
            var result = await _stockServices.GetAllStocksAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count(), Is.EqualTo(2));
        }

       /* [Test]
        public async Task GetProductsWithMinimumStockAsync_ReturnsProductWithMinimumStock()
        {
            // Arrange: Mock a list of stocks with various quantities
            var stocks = new List<Stock>
    {
        new Stock { StoreId = 1, ProductId = 1, Quantity = 10, Product = new Product { ProductId = 1, ProductName = "Product1" } },
        new Stock { StoreId = 2, ProductId = 1, Quantity = 5, Product = new Product { ProductId = 1, ProductName = "Product1" } },
        new Stock { StoreId = 1, ProductId = 2, Quantity = 20, Product = new Product { ProductId = 2, ProductName = "Product2" } }
    }.AsQueryable();

            // Create a mock DbSet using the stock list
            var mockDbSet = new Mock<DbSet<Stock>>();
            mockDbSet.As<IAsyncEnumerable<Stock>>()
                     .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                     .Returns(new TestAsyncEnumerator<Stock>(stocks.GetEnumerator()));
            mockDbSet.As<IQueryable<Stock>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<Stock>(stocks.Provider));
            mockDbSet.As<IQueryable<Stock>>().Setup(m => m.Expression).Returns(stocks.Expression);
            mockDbSet.As<IQueryable<Stock>>().Setup(m => m.ElementType).Returns(stocks.ElementType);
            mockDbSet.As<IQueryable<Stock>>().Setup(m => m.GetEnumerator()).Returns(stocks.GetEnumerator());

            // Mock the context to return the mock DbSet
            _mockContext.Setup(c => c.Stocks).Returns(mockDbSet.Object);

            // Act: Call the method under test
            var result = await _stockServices.GetProductsWithMinimumStockAsync();

            // Assert: Check that the product with the minimum stock is returned
            Assert.IsNotNull(result);
            Assert.That(result.ProductId, Is.EqualTo(1));  // Product1 should have the minimum stock
            Assert.That(result.ProductName, Is.EqualTo("Product1"));
        }
*/



       /* [Test]
        public async Task UpdateStockAsync_ExistingStock_ReturnsUpdatedStock()
        {
            // Arrange
            var existingStock = new Stock { StoreId = 1, ProductId = 1, Quantity = 10 };
            var updateStockDto = new StockDto { StoreId = 1, ProductId = 1, Quantity = 20 };
            var mockDbSet = MockDbSet(new List<Stock> { existingStock });
            _mockContext.Setup(c => c.Stocks).Returns(mockDbSet.Object);

            // Act
            var result = await _stockServices.UpdateStockAsync(1, 1, updateStockDto);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Quantity, Is.EqualTo(20));
            _mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
        }*/

       /* [Test]
        public void UpdateStockAsync_NonExistingStock_ThrowsKeyNotFoundException()
        {
            // Arrange
            var updateStockDto = new StockDto { StoreId = 1, ProductId = 1, Quantity = 20 };
            var mockDbSet = MockDbSet(new List<Stock>());
            _mockContext.Setup(c => c.Stocks).Returns(mockDbSet.Object);

            // Act & Assert
            var ex = Assert.ThrowsAsync<KeyNotFoundException>(() => _stockServices.UpdateStockAsync(1, 1, updateStockDto));
            Assert.That(ex.Message, Is.EqualTo("The specified stock entry was not found."));
        }*/

        private static Mock<DbSet<T>> MockDbSet<T>(List<T> list) where T : class
        {
            var mockSet = new Mock<DbSet<T>>();
            mockSet.As<IAsyncEnumerable<T>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<T>(list.GetEnumerator()));

            mockSet.As<IQueryable<T>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<T>(list.AsQueryable().Provider));

            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(list.AsQueryable().Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(list.AsQueryable().ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => list.GetEnumerator());

            return mockSet;
        }
    }

    // Helper classes for async operations
    internal class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            return Execute<TResult>(expression);
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestAsyncQueryProvider<T>(this); }
        }
    }

    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current
        {
            get
            {
                return _inner.Current;
            }
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return new ValueTask<bool>(_inner.MoveNext());
        }

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return new ValueTask();
        }
    }
}



//using Moq;
//using NUnit.Framework;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Rohit_bike_store.Controllers;
//using Rohit_bike_store.DTO;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;

//namespace TestProject
//{
//    [TestFixture]
//    public class StockServiceTests
//    {
//        private StocksController _controller;
//        private Mock<IStock> _mockStockService;

//        [SetUp]
//        public void Setup()
//        {
//            _mockStockService = new Mock<IStock>();
//            _controller = new StocksController(_mockStockService.Object);
//        }

//        [Test]
//        public async Task AddStock_ReturnsOkResult_WithSuccessMessage_WhenStockIsAdded()
//        {
//            // Arrange
//            var stockDto = new StockDto
//            {
//                StoreId = 1,
//                ProductId = 2,
//                Quantity = 10
//            };
//            var successMessage = "Record Added Successfully!!";
//            _mockStockService.Setup(s => s.AddStockAsync(It.IsAny<StockDto>())).ReturnsAsync(successMessage);

//            // Act
//            var result = await _controller.AddStock(stockDto) as OkObjectResult;

//            // Assert
//            Assert.NotNull(result, "Result should not be null");
//            Assert.IsInstanceOf<OkObjectResult>(result, "Result should be of type OkObjectResult");
//            Assert.AreEqual(successMessage, result.Value, "Returned message should match the success message");
//        }

//        [Test]
//        public async Task GetAllStocks_ReturnsOkResult_WithListOfStocks()
//        {
//            // Arrange
//            var stockList = new List<StockDto> { new StockDto(), new StockDto() };
//            _mockStockService.Setup(s => s.GetAllStocksAsync()).ReturnsAsync(stockList);

//            // Act
//            var result = await _controller.GetAllStocks() as OkObjectResult;

//            // Assert
//            Assert.NotNull(result);
//            Assert.IsInstanceOf<OkObjectResult>(result);
//            Assert.IsInstanceOf<List<StockDto>>(result.Value);
//            Assert.AreEqual(stockList.Count, ((List<StockDto>)result.Value).Count);
//        }

//        [Test]
//        public async Task UpdateStock_ReturnsUpdatedStock_WhenStockIsUpdated()
//        {
//            // Arrange
//            int storeId = 1;
//            int productId = 1;
//            var updateStockDto = new ProductDto();
//            var updatedStockDto = new StockDto();
//            _mockStockService.Setup(s => s.UpdateStockAsync(storeId, productId, updatedStockDto)).ReturnsAsync(updatedStockDto);

//            // Act
//            var result = await _controller.UpdateStock(storeId, productId, updatedStockDto);

//            // Assert
//            Assert.IsInstanceOf<StockDto>(result);
//            Assert.AreEqual(updatedStockDto, result);
//        }

//        [Test]
//        public async Task GetProductsWithMinimumStock_ReturnsOkResult_WithSingleProduct()
//        {
//            // Arrange
//            var product = new ProductDto(); 
//            _mockStockService.Setup(s => s.GetProductsWithMinimumStockAsync())
//                .ReturnsAsync(product);

//            // Act
//            var result = await _controller.GetProductsWithMinimumStock();

//            // Assert
//            Assert.NotNull(result);
//            Assert.IsInstanceOf<OkObjectResult>(result);
//            var okResult = result as OkObjectResult;
//            Assert.NotNull(okResult.Value);
//            Assert.IsInstanceOf<ProductDto>(okResult.Value);
//            Assert.AreEqual(product, okResult.Value);
//        }
//    }
//}