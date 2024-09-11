using NUnit.Framework;
using Rohit_bike_store.Services;
using Rohit_bike_store.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace TestProject
{
    [TestFixture]
    public class StaffServiceTests
    {
        private RohitBikeStoreContext _context;
        private IMapper _mapper;
        private StaffServices _staffService;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new RohitBikeStoreContext(options);
            _mapper = new MapperConfiguration(cfg => {
                // Add your mapping configurations here
            }).CreateMapper();

            _staffService = new StaffServices(_context, _mapper);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var store = new Store { StoreId = 1, StoreName = "Test Store" };
            _context.Stores.Add(store);

            var staff1 = new Staff { StaffId = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", StoreId = 1 };
            var staff2 = new Staff { StaffId = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", StoreId = 1, ManagerId = 1 };
            _context.Staffs.AddRange(staff1, staff2);

            var customer = new Customer { CustomerId = 1, FirstName = "Test", LastName = "Customer" , Email = "xys@gmail.com" };
            _context.Customers.Add(customer);

            var order = new Order { OrderId = 1, CustomerId = 1, StaffId = 1 };
            _context.Orders.Add(order);

            _context.SaveChanges();
        }

        [TearDown]
        public void TearDown()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

       /* [Test]
        public async Task AddStaff_ValidStaff_ReturnsSuccess()
        {
            var newStaff = new Staff { FirstName = "New", LastName = "Staff", Email = "new@example.com", StoreId = 1 };
            var result = await _staffService.AddStaff(newStaff);
            Assert.IsTrue(result.IsSuccess);
            Assert.IsInstanceOf<SuccessResult>(result.Result);
        }*/

        //[Test]
        //public async Task AddStaff_InvalidStaff_ReturnsFailure()
        //{
        //    var invalidStaff = new Staff();
        //    var result = await _staffService.AddStaff(invalidStaff);
        //    Assert.IsFalse(result.IsSuccess);
        //}

        [Test]
        public async Task GetAllStaff_ReturnsAllStaff()
        {
            var result = await _staffService.GetAllStaff();
            Assert.AreEqual(2, result.Count());
        }
/*
        [Test]
        public async Task GetAllStaff_NoStaffInDatabase_ReturnsEmptyList()
        {
            // Arrange: Create a new context and service for this test
            var options = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: "EmptyTestDatabase")
                .Options;

            using (var emptyContext = new RohitBikeStoreContext(options))
            {
                var emptyStaffService = new StaffServices(emptyContext, _mapper);

                // Act
                var result = await emptyStaffService.GetAllStaff();

                // Assert
                Assert.IsEmpty(result);
            }
        }*/

        [Test]
        public async Task GetStaffByStore_ValidStoreName_ReturnsStaff()
        {
            var result = await _staffService.GetStaffByStore("Test Store");
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task GetStaffByStore_InvalidStoreName_ReturnsEmptyList()
        {
            var result = await _staffService.GetStaffByStore("Non-existent Store");
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task ManagerDetails_ValidStaffId_ReturnsManager()
        {
            var result = await _staffService.ManagerDetails(2);
            Assert.AreEqual(1, result.StaffId);
        }

        [Test]
        public void ManagerDetails_InvalidStaffId_ThrowsException()
        {
            Assert.ThrowsAsync<Exception>(async () => await _staffService.ManagerDetails(999));
        }

        [Test]
        public async Task SalesMadeByStaff_ValidStaffId_ReturnsSales()
        {
            var result = await _staffService.SalesMadeByStaff(1);
            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task SalesMadeByStaff_InvalidStaffId_ReturnsEmptyList()
        {
            var result = await _staffService.SalesMadeByStaff(999);
            Assert.IsEmpty(result);
        }

        [Test]
        public async Task UpdateStaffDetails_ValidStaff_ReturnsTrue()
        {
            var updatedStaff = new Staff { StaffId = 1, FirstName = "Updated", LastName = "Name", Email = "updated@example.com", StoreId = 1 };
            var result = await _staffService.UpdateStaffDetails(1, updatedStaff);
            Assert.IsTrue(result);
            var staff = await _context.Staffs.FindAsync(1);
            Assert.AreEqual("Updated", staff.FirstName);
        }

        [Test]
        public async Task UpdateStaffDetails_InvalidStaffId_ReturnsFalse()
        {
            var updatedStaff = new Staff { StaffId = 999, FirstName = "Updated", LastName = "Name", Email = "updated@example.com", StoreId = 1 };
            var result = await _staffService.UpdateStaffDetails(999, updatedStaff);
            Assert.IsFalse(result);
        }

        [Test]
        public async Task UpdateStaffDetailsPatch_ValidStaff_ReturnsTrue()
        {
            var patchStaff = new Staff { FirstName = "Patched" };
            var result = await _staffService.UpdateStaffDetailsPatch(1, patchStaff);
            Assert.IsTrue(result);
            var staff = await _context.Staffs.FindAsync(1);
            Assert.AreEqual("Patched", staff.FirstName);
            Assert.AreEqual("Doe", staff.LastName); // Unchanged
        }

        [Test]
        public async Task UpdateStaffDetailsPatch_InvalidStaffId_ReturnsFalse()
        {
            var patchStaff = new Staff { FirstName = "Patched" };
            var result = await _staffService.UpdateStaffDetailsPatch(999, patchStaff);
            Assert.IsFalse(result);
        }
    }
}





