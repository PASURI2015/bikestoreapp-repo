namespace Rohit_bike_store.DTO
{
    public class AddOrderItemDto
    {
        public int OrderId { get; set; }

        public int ItemId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal ListPrice { get; set; }

        public decimal Discount { get; set; }
    }
}
