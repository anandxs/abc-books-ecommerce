namespace AbcBooks.Models.ViewModels;

public class OrderViewModel
{
    public Order Order { get; set; } = null!;
    public IEnumerable<OrderDetail> OrderDetails { get; set; } = null!;
}
