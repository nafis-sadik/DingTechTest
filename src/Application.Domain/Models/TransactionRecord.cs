namespace Application.Domain.Models
{
    public record TransactionRecord
    {
        public int TransactionId { get; set; }
        public string Sender { get; set; }
        public string Reciever { get; set; }
        public double Amount { get; set; }
    }
}
