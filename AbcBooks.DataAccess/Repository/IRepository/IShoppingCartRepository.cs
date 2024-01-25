using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IShoppingCartRepository : IRepository<ShoppingCart>
{
    int IncrementQuantity(ShoppingCart cart, int incrememnt);
    int DecrementQuantity(ShoppingCart cart, int decrement);
    IEnumerable<ShoppingCart> GetUserShoppingCart(string ApplicationUserId);
    void Updat(ShoppingCart cart);
}
