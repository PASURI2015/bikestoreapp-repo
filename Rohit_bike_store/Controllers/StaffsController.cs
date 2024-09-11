using AutoMapper;
using Rohit_bike_store.Models;
using Rohit_bike_store.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BikeStoreApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StaffsController : ControllerBase
    {
        private readonly IStaff _staffService;

        public StaffsController(IStaff staffService)
        {
            _staffService = staffService;
        }

        // POST: /api/staff/
        [HttpPost]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> AddStaff([FromBody] Staff staff)
        {
            try
            {
                var (isSuccess, result) = await _staffService.AddStaff(staff);

                if (isSuccess)
                {
                    return Ok(result);
                }
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while adding staff.");
            }
        }

        // GET: /api/staff/
        [HttpGet]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> GetAllStaff()
        {
            try
            {
                var staffList = await _staffService.GetAllStaff();
                if (staffList == null || !staffList.Any())
                {
                    return NotFound("No staff found.");
                }
                return Ok(staffList);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving staff.");
            }
        }


        [HttpGet("storeName/{storeName}")]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> GetAllStaffByStore(string storeName)
        {
            try
            {
                if (string.IsNullOrEmpty(storeName))
                {
                    return BadRequest("Store name not provided.");
                }

                var result = await _staffService.GetStaffByStore(storeName);
                if (result == null || !result.Any())
                {
                    return NotFound($"No staff found for store: {storeName}");
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving staff by store.");
            }
        }

        [HttpGet("managerdetails/{staffId}")]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> ManagerDetails(int staffId)
        {
            try
            {
                var result = await _staffService.ManagerDetails(staffId);
                if (result == null)
                {
                    return NotFound(); // Return NotFound if staff or manager is not found
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving manager details.");
            }
        }

        [HttpGet("salesmadebystaff/{staffId}")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> SalesMadeByStaff(int staffId)
        {
            try
            {
                var result = await _staffService.SalesMadeByStaff(staffId);
                if (result == null)
                {
                    return NotFound();
                }
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while retrieving sales made by staff.");
            }
        }

        // PUT: /api/staff/edit/{staffid}
        [HttpPut("edit/{staffid}")]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> EditStaff(int staffid, [FromBody] Staff staff)
        {
            try
            {
                var result = await _staffService.UpdateStaffDetails(staffid, staff);

                if (result == null)
                {
                    return NotFound(new { Message = "Staff not found." });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating staff details.");
            }
        }

        // PATCH: /api/staff/edit/{staffid}
        [HttpPatch("editpatch/{staffid}")]
        [Authorize(Roles = "Store")]
        public async Task<IActionResult> EditStaffDetails(int staffid, [FromBody] Staff staff)
        {
            try
            {
                var result = await _staffService.UpdateStaffDetailsPatch(staffid, staff);
                if (result.Equals(false))
                {
                    return NotFound(new { Message = "Staff not found." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while updating staff details.");
            }
        }
    
    }
}