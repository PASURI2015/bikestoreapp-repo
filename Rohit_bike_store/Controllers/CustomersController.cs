
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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomer c;
        public CustomersController(ICustomer c)
        {
            this.c = c;
        }

        // GET: api/Customers
        [HttpGet]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult<IEnumerable<Customer>>> GetallCustomers()
        {
            return await c.GetallCustomers();
        }

        // GET: api/Customers/5
        [HttpGet("{zipcode}")]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult<List<Customer>>> Getcustomersbyzipcode(string zipcode)
        {
            return await c.Getcustomersbyzipcode(zipcode);
        }

        // PUT: api/Customers/5

        [HttpPut("{id}")]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<Customer>> Updatethewholedetails(int id, [FromBody] Customer customer)
        {
            if (customer == null || customer.CustomerId != id)
            {
                return BadRequest("Invalid customer data.");
            }

            var updatedCustomer = await c.Updatethewholedetails(customer);

            if (updatedCustomer == null)
            {
                return BadRequest("Update not allowed or customer not found."); // Return 403 Forbidden if ApproveStatus is not true or customer not found
            }

            return Ok(updatedCustomer); // Return 200 OK with the updated customer data
        }

        // POST: api/Customers

        [HttpPost]
        [Authorize(Roles = "Staff")]
        public async Task<ActionResult<Customer>> AddnewCustomerObjectinDB(Customer customer)
        {
            return await c.AddnewCustomerObjectinDB(customer);
        }

        [HttpGet("DateTime/{orderdate}")]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult<List<Customer>>> Getcustomersbyorderdate(DateTime orderdate)
        {
            var d = DateOnly.FromDateTime(orderdate);
            return await c.Getcustomersbyorderdate(d);
        }
        [HttpGet("HighestOrder")]
        [Authorize(Roles = "Store")]
        public async Task<string?> Getthecustomerwhoplacedhighestorder()
        {
            return await c.Getthecustomerwhoplacedhighestorder();
        }
        [HttpGet("OrderByName")]
        [Authorize(Roles = "Store")]
        public async Task<List<Customer>> Getdisplaycustomerbycityandorderbyname(string city)
        {
            return await c.Getdisplaycustomerbycityandorderbyname(city);
        }

        [HttpPut("approve-status")]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> UpdateApproveStatus([FromBody] ApproveStatusDto dto)
        {
            if (dto == null)
            {
                return BadRequest("Invalid data.");
            }

            var updateSuccessful = await c.UpdateApproveStatusAsync(dto);

            if (!updateSuccessful)
            {
                return NotFound($"Customer with ID {dto.Id} not found.");
            }

            return NoContent();  // Return 204 No Content on success
        }
    }
}