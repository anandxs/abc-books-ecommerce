using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AbcBooks.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public bool IsBlocked { get; set; } = false;
        public Wallet? Wallet { get; set; }
    }
}
