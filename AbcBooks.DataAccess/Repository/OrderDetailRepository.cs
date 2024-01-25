using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.Repository;

public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
{
    private readonly ApplicationDbContext _db;

    public OrderDetailRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public IEnumerable<OrderDetail> GetOrderDetailsWithProducts(int orderId)
    {
        return _db.OrderDetails
            .Where(x => x.OrderId == orderId)
            .Include(x => x.Product);
    }

    public void Update(OrderDetail orderDetail)
    {
        _db.OrderDetails.Update(orderDetail);
    }
}
