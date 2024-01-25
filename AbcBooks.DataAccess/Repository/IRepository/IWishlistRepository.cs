using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface IWishlistRepository : IRepository<Wishlist>
	{
		bool ContainsProduct(string applicationUserId, int productId);
		IEnumerable<Wishlist> GetUserWishlist(string applicationUserId);
	}
}
