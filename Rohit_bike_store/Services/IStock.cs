using Microsoft.AspNetCore.Mvc;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;

namespace Rohit_bike_store.Services
{
    public interface IStock
    {
        Task<string> AddStockAsync(StockDto stockDto);
        Task<IEnumerable<StockDto>> GetAllStocksAsync();
        Task<ProductDto> GetProductsWithMinimumStockAsync();
        Task<StockDto> UpdateStockAsync(int storeId, int productId, StockDto updateStockDto); // Ensure this method signature is correct
    }
}
