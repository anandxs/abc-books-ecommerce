using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using System.Linq.Expressions;

namespace AbcBooks.DataAccess.Repository
{
	public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _db;

        public CategoryRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public new IEnumerable<Category> GetAll()
        {
            return _db.Categories
                .Where(x => x.IsDeleted == false);
        }

        public new Category GetFirstOrDefault(Expression<Func<Category, bool>> filter)
        {
            return _db.Categories
                .Where(x => x.IsDeleted == false)
				.Where(filter)
                .FirstOrDefault()!;
        }

        public new void Add(Category category)
        {
            var temp = _db.Categories.FirstOrDefault(x => x.Name == category.Name);

            if (temp is null)
            {
                base.Add(category);
            }
            else
            {
                temp.IsDeleted = false;
                Update(temp);
            }
        }

        public void Update(Category category)
        {
            _db.Categories.Update(category);
        }

		public void UpdateListingStatus(Category category)
		{
			category.IsListed = !category.IsListed;
		}

		public void UpdateCategoryDiscount(int id, float discount)
		{
			Category category = _db.Categories.First(c => c.Id == id);
            category.Discount = discount;
		}

		public void Delete(Category category)
		{
            category.IsDeleted = true;
		}
	}
}
