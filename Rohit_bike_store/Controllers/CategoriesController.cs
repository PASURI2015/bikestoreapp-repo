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
using Rohit_bike_store.Service;

namespace Rohit_bike_store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategory _c;
        public CategoriesController(ICategory c)
        {
            _c = c;

        }

        // POST: api/Categories
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Post(CategoryDto category)
        {
            if (category == null)
            {
                return BadRequest("Category data is null");
            }
            await _c.Post(category);
            return Ok("Record Created Successfully");
        }

       // GET: api/Categories
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Category>>> Get()
        {
            try
            {
                var categories = await _c.Get();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // PUT: api/Category/edit/Id
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Put(int id, CategoryDto category)
        {
            try
            {
                var res = await _c.Put(id, category);
                if (res == null)
                {
                    return BadRequest("category Not Found");
                }
                return Ok("Update Successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

       //Get: api/category categoryname/{categoryname}
        [HttpGet("categoryname/{categoryname}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategoryByCategoryName(string categoryname)
        {
            try
            {
                var categories = await _c.GetCategoryByCategoryName(categoryname);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}