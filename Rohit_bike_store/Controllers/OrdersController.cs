using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Rohit_bike_store.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Rohit_bike_store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrder o;
        private readonly IMapper m;

        public OrdersController(IOrder o, IMapper m)
        {
            this.o = o;
            this.m = m;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<List<Order>>> GetOrders()
        {
            try
            {
                var orders = await o.GetAllOrders();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching orders", error = ex.Message });
            }
        }

        [HttpGet("customerid/{customerid}")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<List<Order>>> SearchOrderByCustomerID(int customerid)
        {
            try
            {
                var orders = await o.GetOrderByCustomerId(customerid);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while searching orders by customer ID", error = ex.Message });
            }
        }

        [HttpGet("customer/{customername}")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<List<Order>>> GetOrdersByCustomerName(string customername)
        {
            try
            {
                var orders = await o.GetOrdersByCustomerName(customername);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching orders by customer name", error = ex.Message });
            }
        }

        [HttpGet("OrderDate/{date}")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<List<Order>>> GetOrdersByDate(DateOnly date)
        {
            try
            {
                var orders = await o.GetOrdersByDate(date);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching orders by date", error = ex.Message });
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<List<Order>>> GetOrdersByStatus(int status)
        {
            try
            {
                var orders = await o.GetOrdersByStatus(status);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching orders by status", error = ex.Message });
            }
        }

        [HttpGet("GetNumberOfOrderByDate")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<int>> GetNumberOfOrderByDate(DateOnly date)
        {
            try
            {
                var count = await o.GetNumberOfOrderByDate(date);
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while getting the number of orders by date", error = ex.Message });
            }
        }

        [HttpGet("GetDateWithMaximumOrders")]
        [Authorize(Roles = "Admin,Store")]
        public async Task<ActionResult<DateOnly?>> GetDateWithMaximumOrders()
        {
            try
            {
                var date = await o.GetDateWithMaximumOrders();
                return Ok(date);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while getting the date with maximum orders", error = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> CreateOrder(OrderDto order)
        {
            try
            {
                var ord = new Order
                {
                    CustomerId = order.CustomerId,
                    StoreId = order.StoreId,
                    StaffId = order.StaffId,
                    OrderDate = order.OrderDate,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate
                };

                var result = await o.CreateOrder(ord);
                if (result == null)
                {
                    return BadRequest(new { timeStamp = DateTime.Now, message = "Validation failed" });
                }
                var OrderDtores = m.Map<OrderDto>(result);
                return CreatedAtAction(nameof(SearchOrderByCustomerID), new { customerid = OrderDtores.CustomerId },
                    new { message = "Record Added Successfully", data = OrderDtores });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while creating the order", error = ex.Message });
            }
        }

        [HttpPut("edit/{orderId}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult> UpdateOrder(int orderId, OrderDto order)
        {
            try
            {
                var ord = new OrderDto
                {
                    CustomerId = order.CustomerId,
                    OrderStatus = order.OrderStatus,
                    OrderDate = order.OrderDate,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate,
                    StoreId = order.StoreId,
                    StaffId = order.StaffId,
                };
                bool tf = o.IdExsist(orderId);
                if (tf == true)
                {
                    var ans = await o.UpdateOrder(orderId, ord);
                    return Ok(new { message = "Order updated", data = ans });
                }
                return BadRequest("Update failed");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the order", error = ex.Message });
            }
        }
    }
}
