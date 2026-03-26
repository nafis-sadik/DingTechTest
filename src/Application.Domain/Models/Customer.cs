namespace Application.Domain.Models
{
    public class CustomerModel
    {
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public IEnumerable<int> AccountNumberList { get; set; } = new List<int>();
    }
}
