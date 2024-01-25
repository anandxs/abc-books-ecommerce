using AbcBooks.Models;
using System.Linq.Expressions;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface ICouponRepository : IRepository<Coupon>
{
    Coupon? GetCouponWithDiscountType(Expression<Func<Coupon, bool>> filter);
    IEnumerable<Coupon> GetAllCouponsWithDiscountType();
    void DecrementUsesLeft(int couponId, int decrement = 1);
    void Update(Coupon coupon);
}
