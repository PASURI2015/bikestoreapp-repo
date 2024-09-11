using Rohit_bike_store.Models;
using Microsoft.EntityFrameworkCore;
using Rohit_bike_store.DTO;
using Rohit_bike_store.Services;

namespace Rohit_bike_store.Services
{
    public class OrderItemServices : IOrderItem
    {
        private readonly RohitBikeStoreContext _context;
        public OrderItemServices(RohitBikeStoreContext context)
        {
            _context = context;
        }

        public async Task<string> AddOrderItemAsync(AddOrderItemDto orderItem)
        {
            try
            {
                var orderItems = new OrderItem
                {
                    OrderId = orderItem.OrderId,
                    ItemId = orderItem.ItemId,
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    ListPrice = orderItem.ListPrice,
                    Discount = orderItem.Discount
                };
                _context.OrderItems.Add(orderItems);
                await _context.SaveChangesAsync();
                return "Record Created Successfully";
            }
            catch (Exception ex)
            {
                return "Validation failed: " + ex.Message;
            }
        }

        public async Task<IEnumerable<OrderItem>> GetAllOrderItemsAsync()
        {
            var item = await _context.OrderItems.ToListAsync();
            return item;
        }

        public async Task<decimal> GetBillAmountAsync(int orderId, int itemId)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderId, itemId);
            if (orderItem == null)
                return 0;


            return (decimal)(orderItem.ListPrice * orderItem.Quantity) - orderItem.Discount;
        }

        public async Task<decimal> GetBillWithoutDiscountAsync(int orderId, int itemId)
        {
            var orderItem = await _context.OrderItems.FindAsync(orderId, itemId);
            if (orderItem == null)
                return 0;

            return orderItem.ListPrice * orderItem.Quantity;

        }

        public async Task<OrderItem?> GetOrderItemByIdAsync(int orderId)
        {
            return await _context.OrderItems.FindAsync(orderId);
        }

        public async Task<bool> UpdateOrderItemAsync(int orderId, AddOrderItemDto updatedOrderItem, int itemId)
        {
            try
            {
                var existingOrderItem = await _context.OrderItems
                    .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ItemId == updatedOrderItem.ItemId);

                if (existingOrderItem == null)
                    return false;

                existingOrderItem.ProductId = updatedOrderItem.ProductId;
                existingOrderItem.Quantity = updatedOrderItem.Quantity;
                existingOrderItem.ListPrice = updatedOrderItem.ListPrice;
                existingOrderItem.Discount = updatedOrderItem.Discount;


                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }

        public async Task<bool> UpdateOrderStatusAsync(UpdateOrderStatusDto dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }

            var orderItem = await _context.OrderItems.FindAsync(dto.OrderId, dto.ItemId);
            if (orderItem == null)
            {
                return false; // OrderItem not found
            }

            orderItem.OrderApproved = dto.OrderApproved;

            try
            {
                await _context.SaveChangesAsync();
                return true; // Update successful
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderItems.Any(e => e.OrderId == dto.OrderId))
                {
                    return false; // OrderItem not found after exception
                }
                else
                {
                    throw; // Re-throw exception if concurrency issue persists
                }
            }
        }
    }
}