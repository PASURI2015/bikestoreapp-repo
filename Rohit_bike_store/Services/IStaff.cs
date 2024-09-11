using Rohit_bike_store.Models;
using System.Security.Policy;

namespace Rohit_bike_store.Services
{
        public interface IStaff
        {
            public Task<(bool IsSuccess, object Result)> AddStaff(Staff staff);
            public Task<IEnumerable<Staff>> GetAllStaff();
            public Task<bool> UpdateStaffDetails(int staffId, Staff staff);
            public Task<bool> UpdateStaffDetailsPatch(int staffId, Staff staff);
            public Task<IEnumerable<Staff>> GetStaffByStore(string storeName);
            public Task<List<object>> SalesMadeByStaff(int staffId);
            public Task<Staff> ManagerDetails(int staffId);           
        }
    }

   
