using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBooks.Models.ViewModels
{
	public class BrowseViewModel
	{
        public IEnumerable<Product> Products { get; set; } = null!;
        public IEnumerable<SelectListItem> CategoryList { get; set; } = null!;
        public IEnumerable<SelectListItem> SortOptions { get; set; } = null!;
		public IEnumerable<ProductImage> ProductImages { get; set; } = null!;
        public int CategoryId { get; set; }
        public string? SearchString { get; set; }
        public string SortOption { get; set; } = null!;
    }
}
