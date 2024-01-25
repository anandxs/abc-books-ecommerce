using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AbcBooks.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsDeleted { get; set; } = false;
        public bool IsListed { get; set; }
        [Range(0, 100, ErrorMessage = "Discount can only be between 0 and 100")]
        public float Discount { get; set; }
        public ICollection<Product> Categories { get; set; } = new List<Product>();
    }
}
