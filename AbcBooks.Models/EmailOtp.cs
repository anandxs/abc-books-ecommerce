using System.ComponentModel.DataAnnotations;

namespace AbcBooks.Models;

public class EmailOtp
{
    public int Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [RegularExpression(@"^\d{6}$", ErrorMessage = "Enter valid OTP")]
    public string Otp { get; set; } = null!;

    public DateTime CreatedTime { get; set; }
}
