using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.Repository;

    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
{
        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db): base(db)
        {
            _db = db;
        }

	public int DecrementQuantity(ShoppingCart cart, int decrement)
	{
		cart.Quantity -= decrement;
		return cart.Quantity;
	}

	public IEnumerable<ShoppingCart> GetUserShoppingCart(string ApplicationUserId)
	{
		return _db.ShoppingCart
			.Include(x => x.Product)
			.Where(x => x.ApplicationUserId == ApplicationUserId);
	}

	public int IncrementQuantity(ShoppingCart cart, int incrememnt)
	{
		cart.Quantity += incrememnt;
		return cart.Quantity;
	}

	public void Updat(ShoppingCart cart)
	{
		_db.ShoppingCart.Update(cart);
	}
}
