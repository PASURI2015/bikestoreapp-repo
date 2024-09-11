using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rohit_bike_store.Services
{
    public class ProductServices : IProduct
    {
        private readonly RohitBikeStoreContext _context;

        public ProductServices(RohitBikeStoreContext context)
        {
            _context = context;
        }

        public async Task<Product> AddProduct(Product product)
        {
            try
            {
                var result = await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding product: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            try
            {
                return await _context.Products.ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting all products: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<Object>> GetNumberOfProductsByEachStore()
        {
            try
            {
                var storeSales = await _context.OrderItems
                    .GroupBy(oi => oi.Order.StoreId)
                    .Select(g => new
                    {
                        StoreId = g.Key,
                        TotalQuantity = g.Sum(oi => oi.Quantity)
                    })
                    .Join(
                        _context.Stores,
                        os => os.StoreId,
                        s => s.StoreId,
                        (os, s) => new
                        {
                            StoreName = s.StoreName,
                            NumberOfProducts = os.TotalQuantity
                        })
                    .ToListAsync();

                return storeSales;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting number of products by store: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> GetProductByBrandName(string BrandName)
        {
            try
            {
                return await _context.Products.Where(c => c.Brand.BrandName == BrandName).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product by brand name: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Product>> GetProductByCategoryName(string CategoryName)
        {
            try
            {
                return await _context.Products.Where(c => c.Category.CategoryName == CategoryName).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products by category name: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Product>> GetProductByCustomerId(int CustomerId)
        {
            try
            {
                var productList = await (from p in _context.Products
                                         where _context.OrderItems
                                                 .Where(oi => _context.Orders
                                                     .Where(o => o.CustomerId == CustomerId)
                                                     .Select(o => o.OrderId)
                                                     .Contains(oi.OrderId))
                                                 .Select(oi => oi.ProductId)
                                                 .Contains(p.ProductId)
                                         select p).ToListAsync();

                return productList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products by customer ID: {ex.Message}");
                throw;
            }
        }

        public async Task<List<Product>> GetProductByModelYear(int ModelYear)
        {
            try
            {
                return await _context.Products.Where(c => c.ModelYear == ModelYear).ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting products by model year: {ex.Message}");
                throw;
            }
        }

        public async Task<List<List<string>>> GetProductNameCategoryNameBrandName()
        {
            try
            {
                List<Product> products = await _context.Products
                    .Include(p => p.Brand)
                    .Include(p => p.Category)
                    .ToListAsync();

                List<List<string>> result = new List<List<string>>();

                foreach (var product in products)
                {
                    result.Add(new List<string>
                    {
                       product.ProductName,
                       product.Brand.BrandName,
                       product.Category.CategoryName
                    });
                }
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product name, category name, and brand name: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> ProductPurchasedByMaximumCustomer()
        {
            try
            {
                var productId = await _context.OrderItems
                    .GroupBy(oi => oi.ProductId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefaultAsync();

                var result = await _context.Products.FirstOrDefaultAsync(c => c.ProductId == productId);
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting product purchased by maximum customers: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> UpdateProduct(Product product)
        {
            try
            {
                var result = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == product.ProductId);
                if (result != null)
                {
                    result.ProductName = product.ProductName;
                    result.BrandId = product.BrandId;
                    result.CategoryId = product.CategoryId;
                    result.ModelYear = product.ModelYear;
                    result.ListPrice = product.ListPrice;
                    await _context.SaveChangesAsync();
                    return result;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating product: {ex.Message}");
                throw;
            }
        }

        public async Task<Product> UpdateParticularProductInfo(ProductDto productdto)
        {
            try
            {
                var result = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productdto.ProductId);

                if (result == null)
                {
                    return null;
                }

                result.ProductName = productdto.ProductName ?? result.ProductName;
                result.BrandId = productdto.BrandId ?? result.BrandId;
                result.CategoryId = productdto.CategoryId ?? result.CategoryId;
                result.ModelYear = productdto.ModelYear ?? result.ModelYear;
                result.ListPrice = productdto.ListPrice ?? result.ListPrice;

                await _context.SaveChangesAsync();

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating particular product info: {ex.Message}");
                throw;
            }
        }
    }
}






