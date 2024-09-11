using NUnit.Framework;

using Moq;

using Microsoft.AspNetCore.Mvc;

using Rohit_bike_store.Controllers;

using Rohit_bike_store.Models;

using Rohit_bike_store.Services;

using System.Collections.Generic;

using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace TestProject

{

    [TestFixture]

    public class StoresControllerTests

    {

        private Mock<IStore> _mockStoreService;

        private StoresController _controller;

        [SetUp]

        public void SetUp()

        {

            _mockStoreService = new Mock<IStore>();

            _controller = new StoresController(_mockStoreService.Object);

        }

        [Test]

        public async Task GetAllStores_ReturnsOkResult_WithStores()

        {

            // Arrange

            var stores = new List<Store>

    {

        new Store { StoreId = 1, StoreName = "Store1" },

        new Store { StoreId = 2, StoreName = "Store2" }

    };

            _mockStoreService.Setup(s => s.GetAllStores()).ReturnsAsync(stores);

            // Act

            var result = await _controller.GetAllStores();

            // Assert

            var actionResult = result as ActionResult<IEnumerable<Store>>;

            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");

            var returnValue = okResult.Value as IEnumerable<Store>;

            Assert.IsNotNull(returnValue, "Expected returnValue to be not null.");

            Assert.AreEqual(stores.Count, new List<Store>(returnValue).Count, "Store count does not match.");

        }


        [Test]

        public async Task GetAllStores_ReturnsInternalServerError_WhenExceptionThrown()

        {

            // Arrange

            _mockStoreService.Setup(s => s.GetAllStores()).ThrowsAsync(new System.Exception("Unexpected error"));

            // Act

            var result = await _controller.GetAllStores();

            // Assert

            var objectResult = result.Result as ObjectResult;

            Assert.IsNotNull(objectResult, "Expected ObjectResult but got null.");

            Assert.AreEqual(500, objectResult.StatusCode);

            Assert.AreEqual("Unexpected error", objectResult.Value);

        }


        [Test]

        public async Task GetStoreByCity_ReturnsListOfStores()

        {

            // Arrange

            var city = "New York";

            var stores = new List<Store>

    {

        new Store { StoreId = 1, StoreName = "Store1", City = city },

        new Store { StoreId = 2, StoreName = "Store2", City = city }

    };

            _mockStoreService.Setup(s => s.GetStoreByCity(city)).ReturnsAsync(stores);

            // Act

            var result = await _controller.GetStoreByCity(city);

            // Assert

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");

            var returnedStores = okResult.Value as List<Store>;

            Assert.IsNotNull(returnedStores, "Expected List<Store> but got null.");

            Assert.AreEqual(stores.Count, returnedStores.Count, "Store count does not match.");

        }


        [Test]

        public async Task GetStoreByCity_ReturnsNotFound_WhenNoStoresFound()

        {

            // Arrange

            var city = "NonExistentCity";

            _mockStoreService.Setup(s => s.GetStoreByCity(city)).ReturnsAsync(new List<Store>());

            // Act

            var result = await _controller.GetStoreByCity(city);

            // Assert

            Assert.IsInstanceOf<NotFoundResult>(result.Result);

        }

        [Test]

        public async Task CreateStore_ReturnsOkResult()

        {

            // Arrange

            var store = new Store { StoreId = 1, StoreName = "NewStore" };

            _mockStoreService.Setup(s => s.CreateStore(store)).ReturnsAsync(store);

            // Act

            var result = await _controller.CreateStore(store);

            // Assert

            var actionResult = result as ActionResult;

            var okResult = actionResult as OkObjectResult;

            Assert.IsNotNull(okResult);

            Assert.AreEqual("Record created successfully", okResult.Value);

        }

        [Test]

        public async Task CreateStore_ReturnsBadRequest_WhenStoreCreationFails()

        {

            // Arrange

            var store = new Store { StoreId = 1, StoreName = "NewStore" };

            // Simulate a failure in the store creation

            _mockStoreService.Setup(s => s.CreateStore(store)).ThrowsAsync(new Exception("Creation failed"));

            // Act

            var result = await _controller.CreateStore(store);

            // Assert

            var badRequestResult = result as BadRequestObjectResult;

            Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult but got null.");

            Assert.AreEqual("Creation failed: Creation failed", badRequestResult.Value);

        }


        [Test]

        public async Task PatchUpdateStore_ReturnsResult()

        {

            // Arrange

            var storeId = 1;

            var updatedStore = new Store { StoreId = storeId, StoreName = "UpdatedStore" };

            _mockStoreService.Setup(s => s.PatchUpdateStore(storeId, updatedStore)).ReturnsAsync(true);

            // Act

            var result = await _controller.PatchUpdateStore(storeId, updatedStore);

            // Assert

            var actionResult = result as ActionResult<bool>;

            var okResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");

            Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200.");

            var returnValue = okResult.Value as bool?;

            Assert.IsTrue(returnValue.GetValueOrDefault(), "Expected true but got false.");

        }


        [Test]

        public async Task PatchUpdateStore_ReturnsNotFound_WhenStoreNotFound()

        {

            // Arrange

            var storeId = 1;

            var updatedStore = new Store { StoreId = storeId, StoreName = "UpdatedStore" };

            _mockStoreService.Setup(s => s.PatchUpdateStore(storeId, updatedStore)).ReturnsAsync(false);

            // Act

            var result = await _controller.PatchUpdateStore(storeId, updatedStore);

            // Assert

            var notFoundResult = result.Result as NotFoundResult;

            Assert.IsNotNull(notFoundResult, "Expected NotFoundResult but got null.");

        }

        [Test]

        public async Task GetNumberOfStoresInEachState_ReturnsListOfStores()

        {

            // Arrange

            var expectedStores = new List<GetStoreInEachState>

    {

        new GetStoreInEachState { State = "CA", StoreCount = 10 },

        new GetStoreInEachState { State = "NY", StoreCount = 5 }

    };

            _mockStoreService.Setup(s => s.GetNumberOfStoresInEachState()).ReturnsAsync(expectedStores);

            // Act

            var result = await _controller.GetNumberOfStoresInEachState();

            // Assert

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");

            var returnedStores = okResult.Value as List<GetStoreInEachState>;

            Assert.IsNotNull(returnedStores, "Expected List<GetStoreInEachState> but got null.");

            Assert.AreEqual(expectedStores.Count, returnedStores.Count, "Store count does not match.");

            for (int i = 0; i < expectedStores.Count; i++)

            {

                Assert.AreEqual(expectedStores[i].State, returnedStores[i].State, $"State at index {i} does not match.");

                Assert.AreEqual(expectedStores[i].StoreCount, returnedStores[i].StoreCount, $"StoreCount at index {i} does not match.");

            }

        }


        [Test]

        public async Task GetNumberOfStoresInEachState_ReturnsNotFound_WhenNoStoresFound()

        {

            // Arrange

            _mockStoreService.Setup(s => s.GetNumberOfStoresInEachState()).ReturnsAsync((List<GetStoreInEachState>)null);

            // Act

            var result = await _controller.GetNumberOfStoresInEachState();

            // Assert

            var notFoundResult = result.Result as NotFoundObjectResult; // Expecting NotFoundResult

            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult but got null.");

            Assert.AreEqual("No stores found for each state.", notFoundResult.Value, "The NotFound result message does not match.");

        }


        [Test]

        public async Task GetMaximumCustomers_ReturnsStoreName()

        {

            // Arrange

            var expectedStoreName = "BestStore";

            _mockStoreService.Setup(s => s.GetMaximumCustomers()).ReturnsAsync(expectedStoreName);

            // Act

            var result = await _controller.GetMaximumCustomers();

            // Assert

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");

            Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200.");

            Assert.AreEqual(expectedStoreName, okResult.Value, "Expected store name to match.");

        }


        [Test]

        public async Task GetMaximumCustomers_ReturnsNotFound_WhenNoStoreFound()

        {

            // Arrange

            _mockStoreService.Setup(s => s.GetMaximumCustomers()).ReturnsAsync((string)null);

            // Act

            var result = await _controller.GetMaximumCustomers();

            // Assert

            var notFoundResult = result.Result as NotFoundObjectResult;

            Assert.IsNotNull(notFoundResult, "Expected NotFoundObjectResult but got null.");

            Assert.AreEqual(404, notFoundResult.StatusCode, "Expected status code 404.");

            Assert.AreEqual("No store found.", notFoundResult.Value, "Expected error message not found.");

        }

        [Test]

        public async Task GetHighestSale_ReturnsSaleInfo_WhenSaleFound()

        {

            // Arrange

            var expectedSaleInfo = "TopSellingProduct";

            _mockStoreService.Setup(s => s.GetHighestSale()).ReturnsAsync(expectedSaleInfo);

            // Act

            var result = await _controller.GetHighestSale();

            // Assert

            var okResult = result.Result as OkObjectResult;

            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");

            Assert.AreEqual(200, okResult.StatusCode, "Expected status code 200.");

            Assert.AreEqual(expectedSaleInfo, okResult.Value, "Expected sale info to match.");

        }


        [Test]

        public async Task GetHighestSale_ReturnsInternalServerError_WhenExceptionThrown()

        {

            // Arrange

            var exceptionMessage = "Unexpected error";

            _mockStoreService.Setup(s => s.GetHighestSale()).ThrowsAsync(new System.Exception(exceptionMessage));

            // Act

            var result = await _controller.GetHighestSale();

            // Assert

            var actionResult = result as ActionResult<string>;

            Assert.IsNotNull(actionResult, "Expected ActionResult<string> but got null.");

            var objectResult = actionResult.Result as ObjectResult;

            Assert.IsNotNull(objectResult, "Expected ObjectResult but got null.");

            Assert.AreEqual(500, objectResult.StatusCode);

            Assert.AreEqual(exceptionMessage, objectResult.Value);

        }

    }

}





//using NUnit.Framework;
//using Moq;
//using Microsoft.AspNetCore.Mvc;
//using Rohit_bike_store.Controllers;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Newtonsoft.Json.Linq;

//namespace TestProject
//{
//    [TestFixture]
//    public class StoresControllerTests
//    {
//        private Mock<IStore> _mockStoreService;
//        private StoresController _controller;

//        [SetUp]
//        public void SetUp()
//        {
//            _mockStoreService = new Mock<IStore>();
//            _controller = new StoresController(_mockStoreService.Object);
//        }

//        [Test]
//        public async Task GetAllStores_ReturnsOkResult_WithStores()
//        {
//            // Arrange
//            var stores = new List<Store>
//    {
//        new Store { StoreId = 1, StoreName = "Store1" },
//        new Store { StoreId = 2, StoreName = "Store2" }
//    };

//            _mockStoreService.Setup(s => s.GetAllStores()).ReturnsAsync(stores);

//            // Act
//            var result = await _controller.GetAllStores();

//            // Assert
//            var actionResult = result as ActionResult<IEnumerable<Store>>;
//            var okResult = actionResult.Result as OkObjectResult;
//            Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
//            var returnValue = okResult.Value as IEnumerable<Store>;
//            Assert.IsNotNull(returnValue, "Expected returnValue to be not null.");
//            Assert.AreEqual(stores.Count, new List<Store>(returnValue).Count, "Store count does not match.");
//        }


//        [Test]
//        public async Task GetStoreByCity_ReturnsListOfStores()
//        {
//            // Arrange
//            var city = "New York";
//            var stores = new List<Store>
//            {
//                new Store { StoreId = 1, StoreName = "Store1", City = city },
//                new Store { StoreId = 2, StoreName = "Store2", City = city }
//            };

//            _mockStoreService.Setup(s => s.GetStoreByCity(city)).ReturnsAsync(stores);

//            // Act
//            var result = await _controller.GetStoreByCity(city);

//            // Assert
//            Assert.IsInstanceOf<List<Store>>(result);
//            Assert.AreEqual(stores.Count, result.Count);
//        }

//        [Test]
//        public async Task CreateStore_ReturnsOkResult()
//        {
//            // Arrange
//            var store = new Store { StoreId = 1, StoreName = "NewStore" };

//            _mockStoreService.Setup(s => s.CreateStore(store)).ReturnsAsync(store);

//            // Act
//            var result = await _controller.CreateStore(store);

//            // Assert
//            var actionResult = result as ActionResult;
//            var okResult = actionResult as OkObjectResult;
//            Assert.IsNotNull(okResult);
//            Assert.AreEqual("Record created successfully", okResult.Value);
//        }

//        [Test]
//        public async Task PatchUpdateStore_ReturnsResult()
//        {
//            // Arrange
//            var storeId = 1;
//            var updatedStore = new Store { StoreId = storeId, StoreName = "UpdatedStore" };
//            _mockStoreService.Setup(s => s.PatchUpdateStore(storeId, updatedStore)).ReturnsAsync(true);

//            // Act
//            var result = await _controller.PatchUpdateStore(storeId, updatedStore);

//            // Assert
//            var actionResult = result as ActionResult<bool>;
//            Assert.IsTrue(actionResult.Value);
//        }


//        [Test]
//        public async Task GetNumberOfStoresInEachState_ReturnsListOfStores()
//        {
//            // Arrange
//            var expectedStores = new List<GetStoreInEachState>
//        {
//            new GetStoreInEachState { State = "CA",StoreCount = 10 },
//            new GetStoreInEachState { State = "NY", StoreCount = 5 }
//        };
//            _mockStoreService.Setup(s => s.GetNumberOfStoresInEachState()).ReturnsAsync(expectedStores);

//            // Act
//            var result = await _controller.GetNumberOfStoresInEachState();

//            // Assert
//            Assert.IsNotNull(result, "Expected non-null result.");
//            Assert.AreEqual(expectedStores.Count, result.Count, "Expected the number of stores to match.");
//            Assert.AreEqual(expectedStores[0].State, result[0].State, "Expected state to match.");
//            Assert.AreEqual(expectedStores[0].StoreCount, result[0].StoreCount, "Expected number of stores to match.");
//        }

//        [Test]
//        public async Task GetMaximumCustomers_ReturnsStoreName()
//        {
//            // Arrange
//            var expectedStoreName = "BestStore";
//            _mockStoreService.Setup(s => s.GetMaximumCustomers()).ReturnsAsync(expectedStoreName);

//            // Act
//            var result = await _controller.GetMaximumCustomers();

//            // Assert
//            Assert.AreEqual(expectedStoreName, result, "Expected store name to match.");
//        }

//        [Test]
//        public async Task GetHighestSale_ReturnsSaleInfo()
//        {
//            // Arrange
//            var expectedSaleInfo = "HighestSaleStore";
//            _mockStoreService.Setup(s => s.GetHighestSale()).ReturnsAsync(expectedSaleInfo);

//            // Act
//            var result = await _controller.GetHighestSale();

//            // Assert
//            Assert.AreEqual(expectedSaleInfo, result, "Expected sale info to match.");
//        }
//    }
//}