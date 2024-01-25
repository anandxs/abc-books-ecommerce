using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository
{
	public interface IUserCouponRepository : IRepository<UserCoupon>
	{
		UserCoupon? GetUserCouponWithCoupon(string applicationUserid, int orderId);
	}
}
