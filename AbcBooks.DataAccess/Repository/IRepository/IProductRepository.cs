using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface IProductRepository : IRepository<Product>
	{
		IEnumerable<Product> GetAllProductsWithCategory();
		IEnumerable<Product> GetAllProductsOfCategory(Category category);
		IEnumerable<Product> GetAllListedProductsWithCategory();
		IEnumerable<Product> GetBestSellers();
		Product GetProductWithCategory(int id);
		Product GetListedProductWithCategory(int id);
		void Update(Product product);
		void UpdateListingStatus(bool status, int categoryId);
		void UpdateStock(int productId, int decrement);
		void UpdateDiscount(int productId, float discount);
		void Delete(Product product);
	}
}
