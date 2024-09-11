using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;

namespace Rohit_bike_store.Services
{
    public interface ICustomer
    {
        public Task<Customer> AddnewCustomerObjectinDB(Customer customer);
        public Task<List<Customer>> GetallCustomers();
        public Task<Customer> Updatethewholedetails(Customer customer);
        public Task<Customer> Editthecustomerstreet(Customer customer);
        public Task<List<Customer>> Getdisplaycustomerbycityandorderbyname(string city);
        public Task<List<Customer>> Getcustomersbyorderdate(DateOnly orderDate);
        public Task<string?> Getthecustomerwhoplacedhighestorder();
        public Task<List<Customer>> Getcustomersbyzipcode(string zipcode);
        public Task<Customer> GetCustomerById(int id);
        public Task<bool> UpdateApproveStatusAsync(ApproveStatusDto dto);
    }
}
