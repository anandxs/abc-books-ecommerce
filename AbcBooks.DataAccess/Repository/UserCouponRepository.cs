using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.Repository
{
	public class UserCouponRepository : Repository<UserCoupon>, IUserCouponRepository
	{
		private readonly ApplicationDbContext _db;

		public UserCouponRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public UserCoupon? GetUserCouponWithCoupon(string applicationUserid, int orderId)
		{
			return _db.UserCoupons
				.Include(x => x.Coupon)
				.FirstOrDefault(x => x.ApplicationUserId == applicationUserid && x.OrderId == orderId);
		}
	}
}
