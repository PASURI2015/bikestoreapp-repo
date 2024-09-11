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
    public class BrandsController : ControllerBase
    {
        private readonly IBrand _service;

        public BrandsController(IBrand service)
        {
            _service = service;
        }

        // GET: api/brands
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<BrandDto>>> GetAllBrands()
        {
            try
            {
                var brands = await _service.GetAllBrands();
                return Ok(brands);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }
        [HttpPut("edit/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, BrandDto brandDto)
        {
            try
            {
                await _service.Put(id, brandDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Post([FromBody] BrandDto brandDto)
        {
            if (brandDto == null)
            {
                return BadRequest("Invalid DTO");
            }

            try
            {
                var insertedBrand = await _service.Post(brandDto);
                return Ok(new { Message = "Record Added Successfully.", InsertedRecord = insertedBrand });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
