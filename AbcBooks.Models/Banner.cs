using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AbcBooks.Models;

public class Banner
{
    public int Id { get; set; }
    [ValidateNever]
    [DisplayName("Banner Image")]
    public string ImageUrl { get; set; }
    [Required]
    [DisplayName("Banner Link")]
    public string Href { get; set; }
}
