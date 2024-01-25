using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.EntityFrameworkCore;

namespace AbcBooks.DataAccess.Repository;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    private readonly ApplicationDbContext _db;

    public OrderRepository(ApplicationDbContext db) : base(db)
    {
        _db = db;
    }

    public IEnumerable<Order> GetAllOrders()
    {
        return _db.Orders
            .Include(x => x.ApplicationUser)
            .Include(x => x.ShippingAddress);
    }

    public IEnumerable<Order> GetAllOrdersByCustomer(string ApplicationUserId)
    {
        return _db.Orders
            .Include(x => x.ApplicationUser)
            .Include(x => x.ShippingAddress)
            .Where(x => x.ApplicationUserId == ApplicationUserId);
    }

    public Order GetOrderWithCustomer(int orderId)
    {
        return _db.Orders
            .Include(x => x.ApplicationUser)
            .Include(x => x.ShippingAddress)
            .FirstOrDefault(x => x.Id == orderId)!;
    }

    public void Update(Order order)
    {
        Order temp = _db.Orders.First(x => x.Id == order.Id);

        if (order.TrackingNumber is not null)
        {
            temp.TrackingNumber = order.TrackingNumber;
        }

        if (order.Carrier is not null)
        {
            temp.Carrier = order.Carrier;
        }
    }

    public void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId)
    {
        Order order = _db.Orders.First(x => x.Id == id);

        order.SessionId = sessionId;
        order.PaymentIntendId = paymentIntendId;
        order.PaymentDate = DateTime.Now;
    }

    public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
    {
        var order = _db.Orders.First(x => x.Id == id);
        order.OrderStatus = orderStatus;

        if (paymentStatus is not null)
        {
            order.PaymentStatus = paymentStatus;
        }
    }

    public void UpdateShippingDetails(int id, string carrier, string trackingId)
    {
        var order = _db.Orders.First(x => x.Id == id);

        order.ShippingDate = DateTime.Now;
        order.Carrier = carrier;
        order.TrackingNumber = trackingId;
    }

    public void UpdateCashOnDeliveryStatus(int id)
    {
        var order = _db.Orders.First(x => x.Id == id);

        order.DeliveryDate = DateTime.Now;
        order.PaymentDate = DateTime.Now;
    }

    public void UpdateOnlineDeliveryStatus(int id)
    {
        var order = _db.Orders.First(x => x.Id == id);
        order.DeliveryDate = DateTime.Now;
    }

    public IEnumerable<Order> GetAllOrdersBetweenDates(DateOnly fromDate, DateOnly toDate)
    {
        DateTime f = fromDate.ToDateTime(new TimeOnly(0, 0, 0));
        DateTime t = toDate.ToDateTime(new TimeOnly(23, 59, 59));

        return _db.Orders.Where(x =>
            x.OrderDate >= f &&
            x.OrderDate <= t);
    }
}
