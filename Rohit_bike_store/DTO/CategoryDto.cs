using System.ComponentModel.DataAnnotations;

namespace Rohit_bike_store.DTO
{
    
        public class CategoryDto
        {
            [Key]
            public int CategoryId { get; set; }
            [Required]
            public string CategoryName { get; set; } = null!;
        }
}
