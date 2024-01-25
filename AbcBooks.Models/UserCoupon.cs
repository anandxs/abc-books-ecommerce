using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models;

#nullable disable
public class UserCoupon
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; }
    [ForeignKey(nameof(ApplicationUserId))]
    public ApplicationUser ApplicationUser { get; set; }
    public int CouponId { get; set; }
    [ForeignKey(nameof(CouponId))]
    public Coupon Coupon { get; set; }
    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order Order { get; set; }
}
