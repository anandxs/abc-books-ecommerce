using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.Repository;

public class WishlistRepository : Repository<Wishlist>, IWishlistRepository
{
    private readonly ApplicationDbContext _db;

    public WishlistRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public bool ContainsProduct(string applicationUserId, int productId)
    {
        return _db.Wishlists.FirstOrDefault(x => x.ApplicationUserId == applicationUserId && x.ProductId == productId) is not null ? true : false;
    }

    public IEnumerable<Wishlist> GetUserWishlist(string applicationUserId)
    {
        return _db.Wishlists
            .Include(x => x.Product)
            .Where(x => x.ApplicationUserId == applicationUserId);
    }
}
