namespace AbcBooks.Models
{
	public class Wishlist
	{
		public int Id { get; set; }
		public string ApplicationUserId { get; set; } = null!;
		public ApplicationUser ApplicationUser { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
    }
}
