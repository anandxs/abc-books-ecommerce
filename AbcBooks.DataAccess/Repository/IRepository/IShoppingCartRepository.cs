using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
	{
		int IncrementQuantity(ShoppingCart cart, int incrememnt);
		int DecrementQuantity(ShoppingCart cart, int decrement);
		IEnumerable<ShoppingCart> GetUserShoppingCart(string ApplicationUserId);
		void Updat(ShoppingCart cart);
	}
}
