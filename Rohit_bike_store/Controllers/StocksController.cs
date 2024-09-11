using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;

namespace Rohit_bike_store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly IStock _context;

        public StocksController(IStock context)
        {
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> AddStock([FromBody] StockDto stockDto)
        {
            var result = await _context.AddStockAsync(stockDto);
            return Ok(result);
        }

        [HttpGet]
        [Authorize(Roles = "Staff,Store")]
        public async Task<IActionResult> GetAllStocks()
        {
            var stocks = await _context.GetAllStocksAsync();
            return Ok(stocks);
        }

        [HttpPut("edit")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<StockDto> UpdateStock(int storeId, int productId, [FromBody] StockDto stockDto)
        {
            var updatedStock = await _context.UpdateStockAsync(storeId, productId, stockDto);
            if (updatedStock == null)
            {
                return null;
            }

            return updatedStock;
        }

        [HttpGet("getproductwithminiumstock")]
        [Authorize(Roles = "Staff,Store")]
        public async Task<IActionResult> GetProductsWithMinimumStock()
        {
            var products = await _context.GetProductsWithMinimumStockAsync();
            return Ok(products);
        }
    }
}
