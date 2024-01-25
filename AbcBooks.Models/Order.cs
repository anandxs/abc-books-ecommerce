using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models;

public class Order
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    [ForeignKey(nameof(ApplicationUserId))]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; } = null!;
    [Required]
    [Display(Name = "Shipping Address")]
    public int ShippingAddressId { get; set; }
    [ForeignKey(nameof(ShippingAddressId))]
    [ValidateNever]
    public Address ShippingAddress { get; set; } = null!;
    [Required]
    public DateTime OrderDate { get; set; }
    public DateTime ShippingDate { get; set; }
    public float OrderTotal { get; set; }
    public string? OrderStatus { get; set; }
    public string? PaymentStatus { get; set; }
    public string? TrackingNumber { get; set; }
    public string? Carrier { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = null!;
    public string? SessionId { get; set; }
    public string? PaymentIntendId { get; set; }
    public DateTime DeliveryDate { get; set; }
}
