using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models;

public class Wallet
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; } = null!;
    public float Balance { get; set; }
}
