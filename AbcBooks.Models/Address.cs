using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models
{
    public class Address
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; } = null!;

        [ForeignKey(nameof(ApplicationUserId))]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;
		[Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = null!;

		[Required]
        [Display(Name = "House Number or House Name")]
        public string HouseNameOrNumber { get; set; } = null!;

		[Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; } = null!;

		[Required]
        public string City { get; set; } = null!;
		[Required]
        public string District { get; set; } = null!;

		[Required]
        [Display(Name = "PIN")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "Invalid PIN code format.")]
        public int PinCode { get; set; }
        [Required]
        public string State { get; set; } = null!;

		public bool IsDefault { get; set; }
        public bool IsDeleted { get; set; }
    }
}
