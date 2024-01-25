using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models
{
	public class Transaction
	{
        public int Id { get; set; }
        public float Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public int WalletId { get; set; }
        [ForeignKey(nameof(WalletId))]
        public Wallet Wallet { get; set; } = null!;
        public string TransactionType { get; set; } = null!;
    }
}
