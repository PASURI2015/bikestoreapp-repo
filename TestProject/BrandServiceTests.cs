using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;

namespace TestProject
{
    public class BrandServiceTest
    {
        [TestFixture]
        public class BrandServiceTests
        {
            private BrandServices _brandService;
            private DbContextOptions<RohitBikeStoreContext> _dbContextOptions;
            private IMapper _mapper;

            [SetUp]
            public void SetUp()
            {
                _dbContextOptions = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                    .UseInMemoryDatabase(databaseName: "BrandServiceTestDatabase")
                    .Options;


                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.CreateMap<Brand, BrandDto>().ReverseMap();
                });

                _mapper = mapperConfig.CreateMapper();

                using (var context = new RohitBikeStoreContext(_dbContextOptions))
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                    var brands = new List<Brand>
                {
                    new Brand { BrandId = 1, BrandName = "Brand1" },
                    new Brand { BrandId = 2, BrandName = "Brand2" }
                };

                    context.Brands.AddRange(brands);
                    context.SaveChanges();
                }

                _brandService = new BrandServices(new RohitBikeStoreContext(_dbContextOptions), _mapper);
            }

            [Test]
            public async Task Put_ShouldUpdateExistingBrand()
            {
                // Arrange
                var brandDto = new BrandDto { BrandId = 1, BrandName = "Updated Brand" };

                using (var context = new RohitBikeStoreContext(_dbContextOptions))
                {
                    var brandService = new BrandServices(context, _mapper);

                    // Act
                    var result = await brandService.Put(1, brandDto);

                    // Assert
                    Assert.NotNull(result);
                    Assert.AreEqual(brandDto.BrandName, result.BrandName);

                    var updatedBrand = await context.Brands.FindAsync(1);
                    Assert.NotNull(updatedBrand);
                    Assert.AreEqual("Updated Brand", updatedBrand.BrandName);
                }
            }


            [Test]
            public void Put_InvalidId_ShouldThrowKeyNotFoundException()
            {
                // Arrange
                var brandDto = new BrandDto { BrandId = 99, BrandName = "Non-existent Brand" };

                using (var context = new RohitBikeStoreContext(_dbContextOptions))
                {
                    var brandService = new BrandServices(context, _mapper);

                    // Act & Assert
                    Assert.ThrowsAsync<KeyNotFoundException>(() => brandService.Put(99, brandDto));
                }
            }


            [Test]
            public async Task GetAllBrands_ShouldReturnAllBrands()
            {
                var result = await _brandService.GetAllBrands();


                Assert.NotNull(result);
                Assert.AreEqual(2, result.Count());

                var brand1 = result.FirstOrDefault(b => b.BrandId == 1);
                Assert.IsNotNull(brand1);
                Assert.AreEqual("Brand1", brand1.BrandName);

                var brand2 = result.FirstOrDefault(b => b.BrandId == 2);
                Assert.IsNotNull(brand2);
                Assert.AreEqual("Brand2", brand2.BrandName);
            }


            [Test]
            public async Task GetAllBrands_NoBrands_ShouldReturnEmptyList()
            {
                using (var context = new RohitBikeStoreContext(_dbContextOptions))
                {
                    context.Brands.RemoveRange(context.Brands);  // Remove all existing brands
                    context.SaveChanges();

                    var brandService = new BrandServices(context, _mapper);

                    // Act
                    var result = await brandService.GetAllBrands();

                    // Assert
                    Assert.NotNull(result);
                    Assert.AreEqual(0, result.Count());
                }
            }


            [Test]
            public async Task Post_ShouldAddNewBrand()
            {
                var brandDto = new BrandDto { BrandId = 3, BrandName = "New Brand" };


                using (var context = new RohitBikeStoreContext(_dbContextOptions))
                {
                    var brandService = new BrandServices(context, _mapper);
                    var result = await brandService.Post(brandDto);


                    Assert.NotNull(result);
                    Assert.AreEqual(brandDto.BrandName, result.BrandName);


                    var addedBrand = await context.Brands.FindAsync(3);
                    Assert.NotNull(addedBrand);
                    Assert.AreEqual("New Brand", addedBrand.BrandName);
                }
            }


            [Test]
            public void Post_DuplicateBrand_ShouldThrowInvalidOperationException()
            {
                // Arrange
                var brandDto = new BrandDto { BrandId = 1, BrandName = "Duplicate Brand" };  // Brand with Id 1 already exists

                using (var context = new RohitBikeStoreContext(_dbContextOptions))
                {
                    var brandService = new BrandServices(context, _mapper);

                    // Act & Assert
                    Assert.ThrowsAsync<InvalidOperationException>(() => brandService.Post(brandDto));
                }
            }


        }
    }
}