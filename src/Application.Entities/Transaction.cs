using System.ComponentModel.DataAnnotations.Schema;

namespace Application.Entities
{
    public class Transaction
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }

        public decimal Amount { get; set; }

        public DateTime TransactionTime { get; set; }
    }
}
