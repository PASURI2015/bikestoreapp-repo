using AutoMapper;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Rohit_bike_store.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TestProject
{
    [TestFixture]
    public class CategoryServiceTests
    {
        private CategoryServices _categoryService;
        private DbContextOptions<RohitBikeStoreContext> _dbContextOptions;
        private IMapper _mapper;

        [SetUp]
        public void SetUp()
        {
            _dbContextOptions = new DbContextOptionsBuilder<RohitBikeStoreContext>()
                .UseInMemoryDatabase(databaseName: "CategoryServiceTestDatabase")
                .Options;

            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryDto>().ReverseMap();
            });

            _mapper = mapperConfig.CreateMapper();


            using (var context = new RohitBikeStoreContext(_dbContextOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                var categories = new List<Category>
        {
            new Category { CategoryId = 1, CategoryName = "Category1" },
            new Category { CategoryId = 2, CategoryName = "Category2" }
        };

                context.Categories.AddRange(categories);
                context.SaveChanges();
            }

            _categoryService = new CategoryServices(new RohitBikeStoreContext(_dbContextOptions), _mapper);
        }


        [Test]
        public async Task Put_ShouldUpdateExistingCategory()
        {
            var categoryDto = new CategoryDto { CategoryId = 1, CategoryName = "Updated Category" };

            using (var context = new RohitBikeStoreContext(_dbContextOptions))
            {
                var categoryService = new CategoryServices(context, _mapper);


                var result = await categoryService.Put(1, categoryDto);


                Assert.NotNull(result);
                Assert.AreEqual(categoryDto.CategoryName, result.CategoryName);

                var updatedCategory = await context.Categories.FindAsync(1);
                Assert.NotNull(updatedCategory);
                Assert.AreEqual("Updated Category", updatedCategory.CategoryName);
            }
        }

        [Test]
        public async Task Get_ShouldReturnAllCategories()
        {
            var result = await _categoryService.Get();

            Assert.NotNull(result);
            Assert.AreEqual(2, result.Count);

            var category1 = result.FirstOrDefault(c => c.CategoryId == 1);
            Assert.IsNotNull(category1);
            Assert.AreEqual("Category1", category1.CategoryName);

            var category2 = result.FirstOrDefault(c => c.CategoryId == 2);
            Assert.IsNotNull(category2);
            Assert.AreEqual("Category2", category2.CategoryName);
        }

        [Test]
        public async Task Post_ShouldAddNewCategory()
        {
            var categoryDto = new CategoryDto { CategoryName = "New Category" };

            using (var context = new RohitBikeStoreContext(_dbContextOptions))
            {
                var categoryService = new CategoryServices(context, _mapper);


                var result = await categoryService.Post(categoryDto);


                Assert.NotNull(result);
                Assert.AreEqual(categoryDto.CategoryName, result.CategoryName);

                var addedCategory = await context.Categories.FirstOrDefaultAsync(c => c.CategoryName == "New Category");
                Assert.NotNull(addedCategory);
                Assert.AreEqual("New Category", addedCategory.CategoryName);
            }
        }


        [Test]
        public async Task GetCategoryByCategoryName_ShouldReturnMatchingCategories()
        {
            var result = await _categoryService.GetCategoryByCategoryName("Category1");


            Assert.NotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Category1", result[0].CategoryName);
        }
    }
}