using AutoMapper;
using Rohit_bike_store.Models;
using Rohit_bike_store.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Rohit_bike_store.Services
{
    public class SuccessResult
    {
        public string Message { get; set; }
    }
    public class StaffServices : IStaff
    {
        private readonly RohitBikeStoreContext _dbContext;
        private readonly IMapper _mapper;

        public StaffServices(RohitBikeStoreContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        // AddStaff
        public async Task<(bool IsSuccess, object Result)> AddStaff(Staff staff)
        {
            try
            {
                await _dbContext.Staffs.AddAsync(staff);
                await _dbContext.SaveChangesAsync();

                var success = new SuccessResult { Message = "Record Created Successfully" };
                return (true, success);
            }
            catch (Exception ex)
            {
                var errorResponse = new
                {
                    Timestamp = DateTime.UtcNow,
                    Message = "An error occurred while adding staff: " + ex.Message
                };

                return (false, errorResponse);
            }
        }


        // GetAllStaff
        public async Task<IEnumerable<Staff>> GetAllStaff()
        {
            try
            {
                return await _dbContext.Staffs.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving staff: " + ex.Message);
            }
        }

        // GetStaffByStore
        public async Task<IEnumerable<Staff>> GetStaffByStore(string storeName)
        {
            try
            {
                var staffList = await _dbContext.Staffs.Include(s => s.Store).Where(s => s.Store.StoreName == storeName).ToListAsync();
                return staffList;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving staff by store: " + ex.Message);
            }
        }

        // ManagerDetails
        public async Task<Staff> ManagerDetails(int staffId)
        {
            try
            {
                var staff = await _dbContext.Staffs.FirstOrDefaultAsync(s => s.StaffId == staffId);
                if (staff == null) throw new Exception("Staff not found.");

                int? managerId = staff.ManagerId;
                var manager = await _dbContext.Staffs.FirstOrDefaultAsync(s => s.StaffId == managerId);

                if (manager == null) throw new Exception("Manager not found.");
                return manager;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving manager details: " + ex.Message);
            }
        }

        // SalesMadeByStaff
        public async Task<List<object>> SalesMadeByStaff(int staffId)
        {
            try
            {
                var ordersWithCustomers = await _dbContext.Orders
                    .Where(o => o.StaffId == staffId)
                    .Include(o => o.Customer)
                    .ToListAsync();

                return ordersWithCustomers
                    .Select(o => new
                    {
                        OrderId = o.OrderId,
                        CustomerName = o.Customer.FirstName + " " + o.Customer.LastName
                    })
                    .Cast<object>()
                    .ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving sales made by staff: " + ex.Message);
            }
        }

        // UpdateStaffDetails
        public async Task<bool> UpdateStaffDetails(int staffId, [FromBody] Staff staff)
        {
            try
            {
                var existingStaff = await _dbContext.Staffs.FirstOrDefaultAsync(c => c.StaffId == staffId);
                if (existingStaff == null)
                {
                    return false;
                }

                existingStaff.FirstName = staff.FirstName;
                existingStaff.LastName = staff.LastName;
                existingStaff.Email = staff.Email;
                existingStaff.Phone = staff.Phone;
                existingStaff.Active = staff.Active;
                existingStaff.StoreId = staff.StoreId;
                existingStaff.ManagerId = staff.ManagerId;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating staff details: " + ex.Message);
            }
        }

        // UpdateStaffDetailsPatch
        public async Task<bool> UpdateStaffDetailsPatch(int staffId, [FromBody] Staff staff)
        {
            try
            {
                var existingStaff = await _dbContext.Staffs.FindAsync(staffId);
                if (existingStaff == null)
                {
                    return false;
                }

                if (!string.IsNullOrEmpty(staff.FirstName))
                    existingStaff.FirstName = staff.FirstName;

                if (!string.IsNullOrEmpty(staff.LastName))
                    existingStaff.LastName = staff.LastName;

                if (!string.IsNullOrEmpty(staff.Email))
                    existingStaff.Email = staff.Email;

                if (!string.IsNullOrEmpty(staff.Phone))
                    existingStaff.Phone = staff.Phone;

                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while partially updating staff details: " + ex.Message);
            }
        }
    }
}








//using AutoMapper;
//using Microsoft.AspNetCore.Mvc;
//using Rohit_bike_store.Models;
//using Rohit_bike_store.Services;
//using Microsoft.EntityFrameworkCore;

//namespace Rohit_bike_store.Services
//{
//    public class StaffServices : IStaff
//    {
//        private readonly RohitBikeStoreContext _dbContext;
//        private readonly IMapper _mapper;

//        public StaffServices(RohitBikeStoreContext dbContext, IMapper mapper)
//        {
//            _dbContext = dbContext;
//            _mapper = mapper;
//        }


//        //AddStaff
//        public async Task<(bool IsSuccess, object Result)> AddStaff(Staff staff)
//        {
//            try
//            {
//                var result = await _dbContext.Staffs.AddAsync(staff);
//                await _dbContext.SaveChangesAsync();


//                var success = new { Message = "Record Created Successfully" };
//                return (true, success);
//            }
//            catch (Exception ex)
//            {

//                var errorResponse = new
//                {
//                    Timestamp = DateTime.UtcNow,
//                    Message = "Validation failed: " + ex.Message
//                };

//                return (false, errorResponse);
//            }
//        }


//        //GetAllStaff
//        public async Task<IEnumerable<Staff>> GetAllStaff()
//        {
//            return await _dbContext.Staffs.ToListAsync();
//        }


//        //GetStaffByStore
//        public async Task<IEnumerable<Staff>> GetStaffByStore(string storeName)
//        {
//            var staffList = await _dbContext.Staffs.Include(s => s.Store).Where(s => s.Store.StoreName == storeName).ToListAsync();
//            return staffList;
//        }

//        //ManagerDetails
//        public async Task<Staff> ManagerDetails(int staffId)
//        {
//            var staff = await _dbContext.Staffs.FirstOrDefaultAsync(s => s.StaffId == staffId);
//            int? id = staff.ManagerId;
//            return await _dbContext.Staffs.FirstOrDefaultAsync(s => s.StaffId == id);
//        }
//        //SalesMadeByStaff
//        public async Task<List<object>> SalesMadeByStaff(int staffId)
//        {
//            var ordersWithCustomers = await _dbContext.Orders
//            .Where(o => o.StaffId == staffId)
//            .Include(o => o.Customer)
//            .ToListAsync();


//            return ordersWithCustomers
//            .Select(o => new
//            {
//                OrderId = o.OrderId,
//                CustomerName = o.Customer.FirstName + " " + o.Customer.LastName
//            })
//            .Cast<object>()
//            .ToList();

//        }

//        public async Task<bool> UpdateStaffDetails(int staffId, [FromBody] Staff staff)
//        {
//            var result = await _dbContext.Staffs.FirstOrDefaultAsync(c => c.StaffId == staffId);
//            if (result == null)
//            {
//                return false;
//            }
//            if (result != null)
//            {
//                result.StaffId = staff.StaffId;
//                result.FirstName = staff.FirstName;
//                result.LastName = staff.LastName;
//                result.Email = staff.Email;
//                result.Phone = staff.Phone;
//                result.Active = staff.Active;
//                result.StoreId = staff.StoreId;
//                result.ManagerId = staff.ManagerId;

//                await _dbContext.SaveChangesAsync();
//            }
//            return true;
//        }

//        public async Task<bool> UpdateStaffDetailsPatch(int staffId, [FromBody] Staff staff)
//        {
//            var existingStaff = await _dbContext.Staffs.FindAsync(staffId);
//            if (existingStaff == null)
//            {
//                return false;
//            }

//            if (!string.IsNullOrEmpty(staff.FirstName))
//                existingStaff.FirstName = staff.FirstName;

//            if (!string.IsNullOrEmpty(staff.LastName))
//                existingStaff.LastName = staff.LastName;

//            if (!string.IsNullOrEmpty(staff.Email))
//                existingStaff.Email = staff.Email;

//            if (!string.IsNullOrEmpty(staff.Phone))
//                existingStaff.Phone = staff.Phone;

//            await _dbContext.SaveChangesAsync();

//            return true;

//        }
//    }

//}
