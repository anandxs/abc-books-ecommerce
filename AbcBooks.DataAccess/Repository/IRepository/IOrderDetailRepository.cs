using AbcBooks.Models;

namespace AbcBooks.DataAccess.Repository.IRepository;

public interface IOrderDetailRepository : IRepository<OrderDetail>
{
    void Update(OrderDetail orderDetail);
    IEnumerable<OrderDetail> GetOrderDetailsWithProducts(int orderId);
}
