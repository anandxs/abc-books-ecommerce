using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models;

public class Coupon
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "Coupon Code")]
    public string CouponCode { get; set; } = null!;

    [Required]
    public string Description { get; set; } = null!;

    [Required]
    [Display(Name = "Discount Type")]
    public int DiscountTypeId { get; set; }

    [ForeignKey(nameof(DiscountTypeId))]
    [ValidateNever]
    public DiscountType DiscountType { get; set; } = null!;

    [Display(Name = "Discount Value")]
    public float DiscountValue { get; set; }

    [Display(Name = "Maximum Discount Amount")]
    public float? MaxAmount { get; set; }
    [Display(Name = "Expiration Date")]
    public DateTime? ExpirateDate { get; set; }

    [Display(Name = "Usage Limit")]
    public int? UsageLimit { get; set; }

    [Display(Name = "Uses Left")]
    public int? UsesLeft { get; set; }

    [Required]
    [Display(Name = "Minimum Purchase Amount")]
    public float MinimumPurchaseAmount { get; set; }
}
