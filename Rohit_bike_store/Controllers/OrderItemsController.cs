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
    public class OrderItemsController : ControllerBase
    {
        private readonly IOrderItem _context;

        public OrderItemsController(IOrderItem context)
        {
            _context = context;
        }

        // GET: api/OrderItems
        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> PostOrderItem([FromBody] AddOrderItemDto orderItem)
        {
            var result = await _context.AddOrderItemAsync(orderItem);

            if (result.StartsWith("Record Created Successfully"))
                return Ok(result);

            else
                return BadRequest(new { Timestamp = DateTime.UtcNow, Message = "Validation failed: Invalid data" });
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Store")]
        public async Task<IActionResult> GetAllOrderItems()
        {
            var orderItems = await _context.GetAllOrderItemsAsync();
            return Ok(orderItems);
        }

        [HttpPut("{orderId}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> PutOrderItem(int orderId, [FromBody] AddOrderItemDto orderItem, int itemId)
        {
            var updated = await _context.UpdateOrderItemAsync(orderId, orderItem, itemId);

            if (updated)
                return NoContent();

            else
                return NotFound();
        }

        [HttpGet("{orderId_itemId}")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<IActionResult> GetBillAmount(int orderId, int itemId)
        {
            var amount = await _context.GetBillAmountAsync(orderId, itemId);
            return Ok(amount);
        }

        [HttpGet("{orderId}/billwithoutdiscount")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<IActionResult> GetBillWithoutDiscount(int orderId, int itemId)
        {
            var amount = await _context.GetBillWithoutDiscountAsync(orderId, itemId);
            return Ok(amount);
        }


        [HttpPut("update-order-status")]
        [Authorize(Roles = "Store")] // Ensure appropriate authorization
        public async Task<IActionResult> UpdateOrderStatus([FromBody] UpdateOrderStatusDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            var success = await _context.UpdateOrderStatusAsync(dto);
            if (success)
            {
                return Content("Order Approved!"); // Return 204 No Content on success
            }
            else
            {
                return NotFound($"OrderItem with ID {dto.OrderId} not found.");
            }
        }
    }
}