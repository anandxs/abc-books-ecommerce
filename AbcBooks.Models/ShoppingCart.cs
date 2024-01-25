using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbcBooks.Models;

public class ShoppingCart
{
    public int Id { get; set; }
    public string ApplicationUserId { get; set; } = null!;
    [ForeignKey(nameof(ApplicationUserId))]
    [ValidateNever]
    public ApplicationUser ApplicationUser { get; set; } = null!;

    public int ProductId { get; set; }
    [ForeignKey(nameof(ProductId))]
    [ValidateNever]
    public Product Product { get; set; } = null!;

    [Range(1, 5, ErrorMessage = "Please enter a value between 1 and {2}!")]
    public int Quantity { get; set; }

    [NotMapped]
    public float ListPrice { get; set; }
    [NotMapped]
    public List<ProductImage> ProductImages { get; set; } = null!;
    [NotMapped]
    public bool IsInWishList { get; set; }
}
