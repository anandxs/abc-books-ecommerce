using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBooks.Models.ViewModels
{
	public class CouponViewModel
	{
		public Coupon Coupon { get; set; } = null!;
		[ValidateNever]
		public IEnumerable<SelectListItem> DiscountTypes { get; set; } = null!;
    }
}
