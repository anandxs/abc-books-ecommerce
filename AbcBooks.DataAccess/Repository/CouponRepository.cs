using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository
{
	public class CouponRepository : Repository<Coupon>, ICouponRepository
	{
		private readonly ApplicationDbContext _db;

		public CouponRepository(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}

		public void DecrementUsesLeft(int couponId, int decrement = 1)
		{
			var coupon = _db.Coupons.First(x => x.Id == couponId);
			coupon.UsesLeft -= decrement;
		}

		public IEnumerable<Coupon> GetAllCouponsWithDiscountType()
		{
			return _db.Coupons.Include(x => x.DiscountType);
		}

		public Coupon? GetCouponWithDiscountType(Expression<Func<Coupon, bool>> filter)
		{
			return _db.Coupons
				.Include(x => x.DiscountType)
				.Where(filter)
				.FirstOrDefault();
		}

		public void Update(Coupon coupon)
		{
			_db.Coupons.Update(coupon);
		}
	}
}
