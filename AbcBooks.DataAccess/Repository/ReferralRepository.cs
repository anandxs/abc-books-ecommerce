using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository
{
	public class ReferralRepository : Repository<Referral>, IReferralRepository
	{
		private readonly ApplicationDbContext _db;
		public ReferralRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void UpdateFullFillmentStatus(Referral referral)
		{
			referral.IsFulfilled = true;
		}
	}
}
