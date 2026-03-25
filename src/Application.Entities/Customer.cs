using System.ComponentModel.DataAnnotations;

namespace Application.Entities
{
    public class Customer
    {
        [Key]
        public string CustomerId { get; set; } = string.Empty;

        [Required]
        public string CustomerName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
