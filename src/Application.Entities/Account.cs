using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Application.Entities
{
    public class Account
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AccountId { get; set; }

        [Required]
        public string AccountTitle { get; set; } = string.Empty;

        [Required]
        public string AccountHolderId { get; set; } = string.Empty;

        [AllowNull]
        public string? AccountDescription { get; set; }

        public decimal CurrentBalance { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AccountHolderId")]
        public Customer Customer { get; set; } = new Customer();
    }
}
