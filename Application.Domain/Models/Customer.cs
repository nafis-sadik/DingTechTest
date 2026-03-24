namespace Application.Domain.Models
{
    public record Customer
    {
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
