using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IProductImageRepository : IRepository<ProductImage>
{
    void Update(ProductImage productImage);
    IEnumerable<ProductImage> GetAllProductImages(int id);
    int GetImageCountForProduct(int productId);
}
