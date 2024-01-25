using AbcBooks.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbcBooks.DataAccess.Repository.IRepository
{
    public interface IOrderRepository : IRepository<Order>
	{
		void Update(Order order);
		void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
		void UpdateStripePaymentId(int id, string sessionId, string paymentIntendId);
		void UpdateShippingDetails(int id, string carrier, string trackingId);
		void UpdateCashOnDeliveryStatus(int id);
		void UpdateOnlineDeliveryStatus(int id);
		IEnumerable<Order> GetAllOrders();
		IEnumerable<Order> GetAllOrdersByCustomer(string ApplicationUserId);
		Order GetOrderWithCustomer(int orderId);
		IEnumerable<Order> GetAllOrdersBetweenDates(DateOnly fromDate, DateOnly toDate);
	}
}
