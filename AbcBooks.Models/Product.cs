using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AbcBooks.Models;

public class Product
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string ISBN { get; set; } = null!;
    public string Author { get; set; } = null!;
    public string Description { get; set; } = null!;
    public int Stock { get; set; }
    public float Mrp { get; set; }
    public float Discount { get; set; }
    public bool IsListed { get; set; }
    public bool IsDeleted { get; set; }
    public int CategoryId { get; set; }
    [ValidateNever]
    public Category Category { get; set; } = null!;
}
