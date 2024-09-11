using AutoMapper;
using Rohit_bike_store.Models;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.DTO;

namespace Rohit_bike_store.Services
{
    public class OrderServices : IOrder
    {
        private readonly RohitBikeStoreContext _context;
        private readonly IMapper _mapper;

        public OrderServices(RohitBikeStoreContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


            public async Task<Order> CreateOrder(Order order)
            {
                try
                {
                    var result = await _context.Orders.AddAsync(order);
                    await _context.SaveChangesAsync();
                    return result.Entity;
                }
                catch (Exception ex)
                {
                    // Log the exception
                    Console.WriteLine($"Error creating order: {ex.Message}");
                    return null;
                }
            }

            public async Task<List<Order>> GetAllOrders()
            {
                try
                {
                    return await _context.Orders.ToListAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting all orders: {ex.Message}");
                    return new List<Order>();
                }
            }

            public async Task<DateOnly?> GetDateWithMaximumOrders()
            {
                try
                {
                    var result = await _context.Orders
                        .GroupBy(o => o.OrderDate)
                        .Select(g => new
                        {
                            Date = g.Key,
                            OrderCount = g.Count()
                        })
                        .OrderByDescending(g => g.OrderCount)
                        .FirstOrDefaultAsync();
                    return result?.Date;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting date with maximum orders: {ex.Message}");
                    return null;
                }
            }

            public async Task<List<Order>> GetOrderByCustomerId(int customerid)
            {
                try
                {
                    return await _context.Orders.Where(o => o.CustomerId == customerid).ToListAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting orders by customer ID: {ex.Message}");
                    return new List<Order>();
                }
            }

            public async Task<List<Order>> GetOrdersByCustomerName(string customername)
            {
                try
                {
                    var orders = await _context.Orders
                        .Where(o => o.Customer.FirstName.ToLower() == customername.ToLower()).ToListAsync();
                    return orders;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting orders by customer name: {ex.Message}");
                    return new List<Order>();
                }
            }

            public async Task<List<Order>> GetOrdersByDate(DateOnly date)
            {
                try
                {
                    return await _context.Orders
                        .Where(o => o.OrderDate.Year == date.Year && o.OrderDate.Month == date.Month && o.OrderDate.Day == date.Day)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting orders by date: {ex.Message}");
                    return new List<Order>();
                }
            }

            public async Task<List<Order>> GetOrdersByStatus(int status)
            {
                try
                {
                    return await _context.Orders
                        .Where(o => o.OrderStatus == status)
                        .ToListAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting orders by status: {ex.Message}");
                    return new List<Order>();
                }
            }

            public async Task<Order> UpdateOrder(int orderId, OrderDto order)
            {
                try
                {
                    var result = await _context.Orders.FirstOrDefaultAsync(p => p.OrderId == orderId);
                    if (result != null)
                    {
                        result.CustomerId = order.CustomerId;
                        result.OrderStatus = order.OrderStatus;
                        result.OrderDate = order.OrderDate;
                        result.ShippedDate = order.ShippedDate;
                        result.RequiredDate = order.RequiredDate;
                        result.StoreId = order.StoreId;
                        result.StaffId = order.StaffId;
                        await _context.SaveChangesAsync();
                        return result;
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error updating order: {ex.Message}");
                    return null;
                }
            }

            public bool IdExsist(int orderId)
            {
                try
                {
                    bool result = _context.Orders.Any(x => x.OrderId == orderId);
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking if ID exists: {ex.Message}");
                    return false;
                }
            }

            public async Task<int> GetNumberOfOrderByDate(DateOnly date)
            {
                try
                {
                    return await _context.Orders
                        .CountAsync(x => x.OrderDate.Year == date.Year && x.OrderDate.Month == date.Month && x.OrderDate.Day == date.Day);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting number of orders by date: {ex.Message}");
                    return 0;
                }
            }
        }
    }

