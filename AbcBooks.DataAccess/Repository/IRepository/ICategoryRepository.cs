using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface ICategoryRepository : IRepository<Category>
{
    void Update(Category category);
    void UpdateListingStatus(Category category);
    void UpdateCategoryDiscount(int id, float discount);
    void Delete(Category category);
}
