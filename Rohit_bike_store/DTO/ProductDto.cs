namespace Rohit_bike_store.DTO
{
    public class ProductDto
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public int? BrandId { get; set; }

        public int? CategoryId { get; set; }

        public short? ModelYear { get; set; }
        public decimal? ListPrice { get; set; }
    }
}
