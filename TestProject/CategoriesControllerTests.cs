using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rohit_bike_store.Controllers;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Service;
using System.Linq;

namespace TestProject
{
    [TestFixture]
    public class CategoriesControllerTest
    {
        private Mock<ICategory> _categoryServiceMock;
        private CategoriesController _categoriesController;

        [SetUp]
        public void SetUp()
        {
            _categoryServiceMock = new Mock<ICategory>();
            _categoriesController = new CategoriesController(_categoryServiceMock.Object);
        }

        // Test for POST: api/Categories
        [Test]
        public async Task Post_ShouldReturnOk_WhenCategoryIsCreated()
        {
            var newCategory = new CategoryDto { CategoryId = 1, CategoryName = "Electronics" };
            _categoryServiceMock.Setup(service => service.Post(It.IsAny<CategoryDto>()))
                .ReturnsAsync(newCategory);

            var result = await _categoriesController.Post(newCategory);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Record Created Successfully", okResult.Value);
        }

        // Test for GET: api/Categories
        [Test]
        public async Task Get_ShouldReturnCategoriesList()
        {
            var categoryList = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, CategoryName = "Electronics" },
                new CategoryDto { CategoryId = 2, CategoryName = "Furniture" }
            };
            _categoryServiceMock.Setup(service => service.Get())
                .ReturnsAsync(categoryList);

            var result = await _categoriesController.Get();

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as IEnumerable<CategoryDto>;

            Assert.IsNotNull(returnValue);
            Assert.AreEqual(2, returnValue.Count());
        }

        // Test for PUT: api/Category/edit/{id}
        [Test]
        public async Task Put_ShouldReturnOk_WhenCategoryIsUpdated()
        {
            var updatedCategory = new CategoryDto { CategoryId = 1, CategoryName = "Updated Category" };
            _categoryServiceMock.Setup(service => service.Put(It.IsAny<int>(), It.IsAny<CategoryDto>()))
                .ReturnsAsync(updatedCategory);

            var result = await _categoriesController.Put(1, updatedCategory);

            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual("Update Successfully", okResult.Value);
        }

        // Test for GET: api/categoryname/{categoryname}
        [Test]
        public async Task GetCategoryByCategoryName_ShouldReturnMatchingCategories()
        {
            var categoryList = new List<CategoryDto>
            {
                new CategoryDto { CategoryId = 1, CategoryName = "Electronics" }
            };
            _categoryServiceMock.Setup(service => service.GetCategoryByCategoryName(It.IsAny<string>()))
                .ReturnsAsync(categoryList);

            var result = await _categoriesController.GetCategoryByCategoryName("Electronics");

            Assert.IsInstanceOf<OkObjectResult>(result.Result);
            var okResult = result.Result as OkObjectResult;
            var returnValue = okResult.Value as IEnumerable<CategoryDto>;

            Assert.IsNotNull(returnValue);
            Assert.AreEqual(1, returnValue.Count());
            Assert.AreEqual("Electronics", returnValue.First().CategoryName);
        }
    }
}