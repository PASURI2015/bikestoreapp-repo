using Rohit_bike_store.Models;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.DTO;

namespace Rohit_bike_store.Services
{
    public class CustomerServices : ICustomer
    {
        private readonly RohitBikeStoreContext _context;
        private DateOnly orderDate;

        public CustomerServices(RohitBikeStoreContext context) { _context = context; }
        public async Task<Customer> AddnewCustomerObjectinDB(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
            _context.SaveChanges();
            return customer;
        }

        public async Task<Customer> Editthecustomerstreet(Customer customer)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Customer>> GetallCustomers()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<List<Customer>> Getcustomersbyorderdate(DateOnly orderdate)
        {
            var orders = await _context.Orders.Where(c => (DateOnly)c.OrderDate == orderdate).ToListAsync();
            List<Customer> answer = new List<Customer>();

            foreach (var order in orders)
            {
                var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == order.CustomerId);

                if (customer != null)
                {
                    answer.Add(customer);
                }
            }

            return answer;
        }



        public async Task<List<Customer>> Getcustomersbyzipcode(string zipcode)
        {
            return await _context.Customers.Where(c => c.ZipCode == zipcode).ToListAsync();
        }


        public async Task<List<Customer>> Getdisplaycustomerbycityandorderbyname(string city)
        {
            return await _context.Customers.Where(c => c.City == city).ToListAsync();
        }


        public async Task<string?> Getthecustomerwhoplacedhighestorder()
        {
            var ans = await _context.Orders.GroupBy(p => p.CustomerId)
                 .Select(g => new { CustomerId = g.Key, OrderCount = g.Count() })
                 .OrderByDescending(x => x.OrderCount).Select(x => x.CustomerId)
                 .FirstOrDefaultAsync();
            if (ans == null)
            {
                return null;
            }
            var a = await _context.Customers.FirstOrDefaultAsync(o => o.CustomerId == ans);
            return (a.FirstName);
        }

        public async Task<Customer> Updatethewholedetails(Customer customer)
        {
            // Fetch the existing customer
            var result = await _context.Customers
                                       .FirstOrDefaultAsync(c => c.CustomerId == customer.CustomerId);

            if (result != null)
            {
                // Check if the ApproveStatus is true
                if ((bool)!result.ApproveStatus)
                {
                    // Return null if ApproveStatus is not true
                    return null;
                }

                // Update the customer details
                result.FirstName = customer.FirstName;
                result.LastName = customer.LastName;
                result.Email = customer.Email;
                result.Phone = customer.Phone;
                result.City = customer.City;
                result.Street = customer.Street;
                result.ZipCode = customer.ZipCode;

                // Save changes to the database
                await _context.SaveChangesAsync();
                return result;
            }

            // Return null if the customer was not found
            return null;
        }


        public async Task<Customer> GetCustomerById(int id)
        {
            return await _context.Customers.FindAsync(id);
        }

        public async Task<bool> UpdateApproveStatusAsync(ApproveStatusDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var customer = await _context.Customers.FindAsync(dto.Id);
            if (customer == null)
            {
                return false; // Customer not found
            }

            customer.ApproveStatus = dto.ApproveStatus;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Update successful
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.CustomerId == dto.Id))
                {
                    return false; // Customer not found after exception
                }
                else
                {
                    throw; // Re-throw exception if concurrency issue persists
                }
            }
        }
    }
}