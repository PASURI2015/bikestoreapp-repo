using Microsoft.AspNetCore.Mvc;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;

namespace Rohit_bike_store.Services
{
    public interface IProduct
    {
        public Task<Product> AddProduct(Product product);
        public Task<List<Product>> GetAllProducts();
        public Task<Product> UpdateProduct(Product product);
        public Task<List<Product>> GetProductByCategoryName(string CategoryName);
        public Task<Product> GetProductByBrandName(string BrandName);
        public Task<List<Product>> GetProductByModelYear(int ModelYear);
        public Task<IEnumerable<Object>> GetNumberOfProductsByEachStore();
        public Task<List<List<string>>> GetProductNameCategoryNameBrandName();
        public Task<List<Product>> GetProductByCustomerId(int CustomerId);
        public Task<Product> ProductPurchasedByMaximumCustomer();
        public Task<Product> UpdateParticularProductInfo(ProductDto productdto);
    }
}
