using Rohit_bike_store.Models;
using Rohit_bike_store.DTO;

namespace Rohit_bike_store.Services
{
    public interface IOrder
    {
        Task<List<Order>> GetAllOrders();
        Task<List<Order>> GetOrderByCustomerId(int customerid);
        Task<List<Order>> GetOrdersByCustomerName(string customername);
        Task<List<Order>> GetOrdersByDate(DateOnly date);
        Task<List<Order>> GetOrdersByStatus(int status);
        Task<int> GetNumberOfOrderByDate(DateOnly date);
        Task<Order> CreateOrder(Order order);
        Task<Order> UpdateOrder(int orderId, OrderDto order);
        bool IdExsist(int id);
        Task<DateOnly?> GetDateWithMaximumOrders();
    }
}
