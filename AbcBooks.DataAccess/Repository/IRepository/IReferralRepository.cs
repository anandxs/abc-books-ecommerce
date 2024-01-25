using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface IReferralRepository : IRepository<Referral>
	{
		void UpdateFullFillmentStatus(Referral referral);
	}
}
