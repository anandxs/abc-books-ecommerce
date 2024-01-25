using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository
{
	public class DiscountTypeRepository : Repository<DiscountType>, IDiscountTypeRepository
	{
		private readonly ApplicationDbContext _db;
		public DiscountTypeRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
	}
}
