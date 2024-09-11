using Rohit_bike_store.DTO;
using Rohit_bike_store.Models;

namespace Rohit_bike_store.Services
{
    public interface IOrderItem
    {
        public Task<string> AddOrderItemAsync(AddOrderItemDto orderItem);
        public Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync();
        public Task<OrderItem?> GetOrderItemByIdAsync(int orderId);
        public Task<bool> UpdateOrderItemAsync(int orderId, AddOrderItemDto updatedOrderItem, int itemId);
        public Task<decimal> GetBillAmountAsync(int orderId, int itemId);
        public Task<decimal> GetBillWithoutDiscountAsync(int orderId, int itemId);
        public Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusDto dto);
    }
}