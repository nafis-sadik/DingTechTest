namespace Application.Domain.Models
{
    public record AccountToAccountTransactionRecord
    {
        public int TransactionId { get; set; }
        public int SenderAccountNo { get; set; }
        public int RecieverAccountNo { get; set; }
        public string RecieverAccountHolderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
