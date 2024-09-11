using System.ComponentModel.DataAnnotations;

namespace Rohit_bike_store.DTO
{
    public class BrandDto
    {
        [Key]
        public int BrandId { get; set; }
        [Required]
        public string BrandName { get; set; } = null!;
    }
}
