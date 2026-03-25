namespace Application.Domain.Models
{
    public class SingleTransactionRecord
    {
        public int TransactionId { get; set; }
        public int AccountId { get; set; }
        public string AccountHolderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;
    }
}
