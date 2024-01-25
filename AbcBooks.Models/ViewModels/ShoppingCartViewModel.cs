using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBooks.Models.ViewModels
{
	public class ShoppingCartViewModel
	{
		public IEnumerable<ShoppingCart> CartItems { get; set; } = null!;
        public Order Order { get; set; } = null!;
		public IEnumerable<ProductImage> ProductImages { get; set; } = null!;
		public IEnumerable<SelectListItem> AddressList { get; set; } = null!;
        public Wallet Wallet { get; set; } = null!;
        public string? CouponCode { get; set; }
        public float? PreDiscountOrderTotal { get; set; }
    }
}
