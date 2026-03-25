using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Entities
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        public string? From { get; set; }

        public string? To { get; set; }

        public decimal Amount { get; set; }

        public DateTime Time { get; set; }
    }
}
