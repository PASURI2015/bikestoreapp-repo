using NUnit.Framework;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TestProject
{
    [TestFixture]
    public class StoreServicesTests
    {
        private RohitBikeStoreContext _context;
        private StoreServices _storeServices;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new RohitBikeStoreContext(options);
            _storeServices = new StoreServices(_context);

            // Seed some test data
            _context.Stores.AddRange(
                new Store { StoreId = 1, StoreName = "Store 1", City = "City1", State = "State1" },
                new Store { StoreId = 2, StoreName = "Store 2", City = "City2", State = "State2" }
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
        public async Task GetAllStores_ReturnsAllStores()
        {
            var result = await _storeServices.GetAllStores();
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task CreateStore_AddsNewStore()
        {
            var newStore = new Store { StoreName = "New Store", City = "NewCity", State = "NewState" };
            var result = await _storeServices.CreateStore(newStore);
            Assert.That(result.StoreName, Is.EqualTo("New Store"));
            Assert.That(_context.Stores.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreateStore_ThrowsException_WhenStoreIsNull()
        {
            Assert.ThrowsAsync<Exception>(async () => await _storeServices.CreateStore(null));
        }

        [Test]
        public async Task UpdateStore_UpdatesExistingStore()
        {
            var store = await _context.Stores.FindAsync(1);
            store.StoreName = "Updated Store";
            var result = await _storeServices.UpdateStore(store);
            Assert.That(result.StoreName, Is.EqualTo("Updated Store"));
        }

        [Test]
        public async Task UpdateStore_ReturnsNull_WhenStoreNotFound()
        {
            var nonExistentStore = new Store { StoreId = 999, StoreName = "Non-existent" };
            var result = await _storeServices.UpdateStore(nonExistentStore);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetStoreByCity_ReturnsCorrectStores()
        {
            var result = await _storeServices.GetStoreByCity("City1");
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].StoreName, Is.EqualTo("Store 1"));
        }

        [Test]
        public async Task GetStoreByCity_ReturnsEmptyList_WhenCityNotFound()
        {
            var result = await _storeServices.GetStoreByCity("NonExistentCity");
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task Delete_RemovesStore()
        {
            await _storeServices.Delete(1);
            Assert.That(_context.Stores.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Delete_ReturnsNull_WhenStoreNotFound()
        {
            var result = await _storeServices.Delete(999);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetNumberOfStoresInEachState_ReturnsCorrectCounts()
        {
            var result = await _storeServices.GetNumberOfStoresInEachState();
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.Any(s => s.State == "State1" && s.StoreCount == 1), Is.True);
            Assert.That(result.Any(s => s.State == "State2" && s.StoreCount == 1), Is.True);
        }

        [Test]
        public async Task GetMaximumCustomers_ReturnsCorrectStore()
        {
            // Add some test orders
            _context.Orders.AddRange(
                new Order { OrderId = 1, StoreId = 1, CustomerId = 1 },
                new Order { OrderId = 2, StoreId = 1, CustomerId = 2 },
                new Order { OrderId = 3, StoreId = 2, CustomerId = 3 }
            );
            _context.SaveChanges();

            var result = await _storeServices.GetMaximumCustomers();
            Assert.That(result, Is.EqualTo("Store 1"));
        }

        [Test]
        public async Task GetHighestSale_ReturnsCorrectStore()
        {
            // Add some test orders and order items
            _context.Orders.Add(new Order { OrderId = 1, StoreId = 1 });
            _context.OrderItems.AddRange(
                new OrderItem { OrderId = 1, ItemId = 1 },
                new OrderItem { OrderId = 1, ItemId = 2 }
            );
            _context.SaveChanges();

            var result = await _storeServices.GetHighestSale();
            Assert.That(result, Is.EqualTo("Store 1"));
        }

        [Test]
        public async Task PatchUpdateStore_UpdatesSpecifiedFields()
        {
            var updatedStore = new Store { StoreName = "Updated Name", Phone = "1234567890" };
            var result = await _storeServices.PatchUpdateStore(1, updatedStore);
            Assert.That(result, Is.True);

            var store = await _context.Stores.FindAsync(1);
            Assert.That(store.StoreName, Is.EqualTo("Updated Name"));
            Assert.That(store.Phone, Is.EqualTo("1234567890"));
            Assert.That(store.City, Is.EqualTo("City1")); // Unchanged field
        }

        [Test]
        public async Task PatchUpdateStore_ReturnsFalse_WhenStoreNotFound()
        {
            var updatedStore = new Store { StoreName = "Updated Name" };
            var result = await _storeServices.PatchUpdateStore(999, updatedStore);
            Assert.That(result, Is.False);
        }
    }
}






//using Microsoft.EntityFrameworkCore;
//using NUnit.Framework;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;


