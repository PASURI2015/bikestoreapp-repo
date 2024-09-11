using System.ComponentModel.DataAnnotations;

namespace Rohit_bike_store.DTO
{
    public class OrderDto
    {
        [Key]
        public int OrderId { get; set; }
        public int? CustomerId { get; set; }
        [Required]
        public byte OrderStatus { get; set; }
        public DateOnly OrderDate { get; set; }
        public DateOnly RequiredDate { get; set; }
        public DateOnly? ShippedDate { get; set; }
        public int StoreId { get; set; }
        public int StaffId { get; set; }
    }
}
