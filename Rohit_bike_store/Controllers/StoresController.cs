using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;

namespace Rohit_bike_store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStore s;

        public StoresController(IStore s)
        {
            this.s = s;
        }


        // GET: api/<StoresController>
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<Store>>> GetAllStores()
        {
            try
            {
                var stores = await s.GetAllStores();
                return Ok(stores);
            }
            catch (Exception ex)
            {
                // Log the exception if needed
                return StatusCode(500, ex.Message);
            }
        }
        // GET api/<StoresController>/5
        [HttpGet("{city}")]
        [Authorize(Roles = "Admin")]
        [HttpGet("bycity/{city}")]
        public async Task<ActionResult<IEnumerable<Store>>> GetStoreByCity(string city)
        {
            var stores = await s.GetStoreByCity(city);

            if (stores == null || !stores.Any())
            {
                return NotFound();
            }

            return Ok(stores);
        }


        // POST api/<StoresController>
        [HttpPost]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult> CreateStore(Store store)
        {
            try
            {
                var createdStore = await s.CreateStore(store);

                if (createdStore == null)
                {
                    return BadRequest("Creation failed");
                }

                return Ok("Record created successfully");
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return BadRequest("Creation failed: " + ex.Message);
            }
        }

        // PUT api/<StoresController>/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult> UpdateStore(int id, Store store)
        {
            var res = await s.UpdateStore(store);
            return Ok(new { Message = "Record updated successfully.", InsertedRecord = res });
        }

        [HttpGet("storesineachstate")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<GetStoreInEachState>>> GetNumberOfStoresInEachState()
        {
            var storesInStates = await s.GetNumberOfStoresInEachState();
            if (storesInStates == null || !storesInStates.Any())
            {
                return NotFound("No stores found for each state.");
            }
            return Ok(storesInStates);
        }

        [HttpGet("MaxCustomersStoreName")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> GetMaximumCustomers()
        {
            var storeName = await s.GetMaximumCustomers();
            if (string.IsNullOrEmpty(storeName))
            {
                return NotFound("No store found.");
            }
            return Ok(storeName);
        }

        [HttpGet("GetHighestSale")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<string>> GetHighestSale()
        {
            try
            {
                var saleInfo = await s.GetHighestSale();
                if (string.IsNullOrEmpty(saleInfo))
                {
                    return NotFound("No sale information found.");
                }
                return Ok(saleInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        // DELETE api/<StoresController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            return Ok(s.Delete(id));
        }

        //Patch
        [HttpPatch("HttpPatchAttribute")]
        [Authorize(Roles = "Store")]
        public async Task<ActionResult<bool>> PatchUpdateStore(int storeId, Store updatedStore)
        {
            var result = await s.PatchUpdateStore(storeId, updatedStore);
            if (result)
            {
                return Ok(result);
            }
            return NotFound();
        }

    }
}