using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBooks.Models.ViewModels;

public class ProductViewModel
{
    public Product Product { get; set; } = null!;
    [ValidateNever]
    public IEnumerable<SelectListItem> CategoryList { get; set; } = null!;
    [ValidateNever]
    public IEnumerable<ProductImage> ProductImages { get; set; } = null!;
}
