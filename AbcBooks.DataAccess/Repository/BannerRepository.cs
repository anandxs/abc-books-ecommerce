using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository
{
	public class BannerRepository : Repository<Banner>, IBannerRepository
	{
		private readonly ApplicationDbContext _db;

		public BannerRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
	}
}
