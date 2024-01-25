namespace AbcBooks.Models.ViewModels
{
	public class HomeViewModel
	{
		public IEnumerable<ProductImage> ProductImages { get; set; }
		public List<Product> Products { get; set; }
        public List<Banner> Banners { get; set; }
    }
}
