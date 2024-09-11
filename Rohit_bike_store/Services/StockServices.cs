using AutoMapper;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Rohit_bike_store.Services
{
    public class StockServices : IStock
    {
        private readonly RohitBikeStoreContext _context;
        private readonly IMapper _mapper;

        public StockServices(RohitBikeStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<string> AddStockAsync(StockDto stockDto)
        {

            try
            {
                var stock = new Stock
                {
                    StoreId = stockDto.StoreId,
                    ProductId = stockDto.ProductId,
                    Quantity = stockDto.Quantity
                };
                _context.Stocks.Add(stock);
                await _context.SaveChangesAsync();
                return "Record Added Successfully!!";
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while adding stock.", ex);
            }
        }

        public async Task<IEnumerable<StockDto>> GetAllStocksAsync()
        {
            try
            {
                return await _context.Stocks
                    .Select(s => new StockDto
                    {
                        StoreId = s.StoreId,
                        ProductId = s.ProductId,
                        Quantity = s.Quantity
                    })
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while retrieving all stocks.", ex);

            }
        }
        public async Task<ProductDto> GetProductsWithMinimumStockAsync()
        {

            var productWithMinStock = await _context.Stocks
        .GroupBy(s => s.Product)
        .Select(g => new
        {
            Product = g.Key,
            TotalQuantity = g.Sum(s => s.Quantity)
        })
        .OrderBy(result => result.TotalQuantity)
        .FirstOrDefaultAsync();

            
            if (productWithMinStock == null)
            {
                return null;
            }
            
            var product = productWithMinStock.Product;
            var productDto = new ProductDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName
            };
            return productDto;
        }
        public async Task<StockDto> UpdateStockAsync(int storeId, int productId, StockDto updateStockDto)
        {
            var stock = await _context.Stocks
        .FirstOrDefaultAsync(s => s.StoreId == storeId && s.ProductId == productId);

            if (stock != null)
            {
                
                stock.Quantity = updateStockDto.Quantity;                 
                await _context.SaveChangesAsync();
                
                var stockDto = new StockDto
                {
                    StoreId = stock.StoreId,
                    ProductId = stock.ProductId,
                    Quantity = stock.Quantity
                };

                return stockDto;
            }
            else
            {
                throw new KeyNotFoundException("The specified stock entry was not found.");
            }
        }
    }
}
