using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.Models
{
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
}
