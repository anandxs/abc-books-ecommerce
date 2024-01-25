using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Models.ViewModels;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbcBooks.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<OrderController> _logger;

    public OrderController(IUnitOfWork unitOfWork, ILogger<OrderController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Details(int orderId)
    {
        OrderViewModel model = new()
        {
            Order = _unitOfWork.Orders.GetOrderWithCustomer(orderId),
            OrderDetails = _unitOfWork.OrderDetails.GetOrderDetailsWithProducts(orderId)
        };

        if (User.IsInRole(ProjectConstants.ADMIN))
        {
            return View(model);
        }

        var claimsIdentity = (ClaimsIdentity)User.Identity;
        var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        if (model.Order.ApplicationUserId != claim.Value)
        {
            return Unauthorized();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult UpdateOrderDetail(OrderViewModel model)
    {
        model.OrderDetails = _unitOfWork.OrderDetails.GetOrderDetailsWithProducts(model.Order.Id);

        _unitOfWork.Orders.Update(model.Order);
        _unitOfWork.Save();

        model.Order = _unitOfWork.Orders.GetOrderWithCustomer(model.Order.Id);

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult ConfirmOrder(OrderViewModel model)
    {
        _unitOfWork.Orders.UpdateStatus(
            model.Order.Id,
            ProjectConstants.STATUS_IN_PROCESS);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult ShipOrder(OrderViewModel model)
    {
        if (model.Order.TrackingNumber is null || model.Order.Carrier is null)
        {
            TempData["TrackingIdOrCarrierIsEmpty"] = "Tracking Id and/or Carrier is Empty";
            return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
        }

        var order = _unitOfWork.Orders.GetFirstOrDefault(x => x.Id == model.Order.Id);

        if (order.OrderStatus != ProjectConstants.STATUS_IN_PROCESS && order.OrderStatus != ProjectConstants.STATUS_APPROVED)
        {
            return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
        }

        _unitOfWork.Orders.UpdateShippingDetails(model.Order.Id, model.Order.Carrier, model.Order.TrackingNumber);
        _unitOfWork.Save();

        _unitOfWork.Orders.UpdateStatus(
            model.Order.Id,
            ProjectConstants.STATUS_SHIPPED);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult CashOnDelivery(OrderViewModel model)
    {
        _unitOfWork.Orders.UpdateStatus(
            model.Order.Id,
            ProjectConstants.STATUS_DELIVERED,
            ProjectConstants.PAYMENT_STATUS_APPROVED);
        _unitOfWork.Save();

        _unitOfWork.Orders.UpdateCashOnDeliveryStatus(model.Order.Id);
        _unitOfWork.Save();

        try
        {
            _logger.LogInformation("Checking for pending referral fulfillment");
            FulfillReferrer(model);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to do referrer fulfillment!");
            _logger.LogError(ex.ToString());
        }

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CancelOrder(OrderViewModel model)
    {
        var order = _unitOfWork.Orders.GetOrderWithCustomer(model.Order.Id);
        var orderDetails = _unitOfWork.OrderDetails.GetOrderDetailsWithProducts(order.Id).ToList();

        if (order.OrderStatus == ProjectConstants.STATUS_CANCELLED)
        {
            return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
        }

        foreach (var orderDetail in orderDetails)
        {
            _unitOfWork.Products.UpdateStock(orderDetail.ProductId, -(orderDetail.Quantity));
            _unitOfWork.Save();
        }

        if (order.PaymentMethod == ProjectConstants.PAYMENT_METHOD_CASH_ON_DELIVERY)
        {
            _unitOfWork.Orders.UpdateStatus(
            model.Order.Id,
            ProjectConstants.STATUS_CANCELLED,
            ProjectConstants.PAYMENT_STATUS_CANCELLED);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
        }

        _unitOfWork.Wallets.UpdateBalance(order.ApplicationUserId, order.OrderTotal);
        _unitOfWork.Save();

        Transaction transaction = new()
        {
            WalletId = _unitOfWork.Wallets.GetWalletWithUser(x => x.ApplicationUserId == order.ApplicationUserId).Id,
            Amount = order.OrderTotal,
            TransactionDate = DateTime.Now,
            TransactionType = ProjectConstants.TRANSFER_CREDIT
        };
        _unitOfWork.Transactions.Add(transaction);
        _unitOfWork.Save();

        _unitOfWork.Orders.UpdateStatus(
            model.Order.Id,
            ProjectConstants.STATUS_CANCELLED,
            ProjectConstants.PAYMNET_STATUS_REFUND_COMPLETE);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ReturnOrder(OrderViewModel model)
    {
        _unitOfWork.Orders.UpdateStatus(model.Order.Id, ProjectConstants.STATUS_RETURN_REQUESTED);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult ApproveReturn(OrderViewModel model)
    {
        _unitOfWork.Orders.UpdateStatus(model.Order.Id, ProjectConstants.STATUS_RETURN_APPROVED);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult DeclineReturn(OrderViewModel model)
    {
        _unitOfWork.Orders.UpdateStatus(model.Order.Id, ProjectConstants.STATUS_RETURN_REJECTED);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = ProjectConstants.ADMIN)]
    public IActionResult RefundToWallet(OrderViewModel model)
    {
        var order = _unitOfWork.Orders.GetFirstOrDefault(x => x.Id == model.Order.Id);

        if (order == null)
        {
            return NotFound("Could not find the order");
        }
        else if (order.OrderStatus == ProjectConstants.PAYMNET_STATUS_REFUND_COMPLETE)
        {
            return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
        }

        _unitOfWork.Wallets.UpdateBalance(order.ApplicationUserId, order.OrderTotal);
        _unitOfWork.Save();

        Transaction transaction = new()
        {
            WalletId = _unitOfWork.Wallets.GetWalletWithUser(x => x.ApplicationUserId == order.ApplicationUserId).Id,
            Amount = order.OrderTotal,
            TransactionDate = DateTime.Now,
            TransactionType = ProjectConstants.TRANSFER_CREDIT
        };
        _unitOfWork.Transactions.Add(transaction);
        _unitOfWork.Save();

        _unitOfWork.Orders.UpdateStatus(model.Order.Id, ProjectConstants.STATUS_RETURN_COMPLETE, ProjectConstants.PAYMNET_STATUS_REFUND_COMPLETE);
        _unitOfWork.Save();

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult OrderDelivered(OrderViewModel model)
    {
        _unitOfWork.Orders.UpdateStatus(model.Order.Id, ProjectConstants.STATUS_DELIVERED);
        _unitOfWork.Save();

        _unitOfWork.Orders.UpdateOnlineDeliveryStatus(model.Order.Id);
        _unitOfWork.Save();

        try
        {
            _logger.LogInformation("Checking for pending referral fulfillment");
            FulfillReferrer(model);
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to do referrer fulfillment!");
            _logger.LogError(ex.ToString());
        }

        return RedirectToAction(nameof(Details), new { orderId = model.Order.Id });
    }

    private void FulfillReferrer(OrderViewModel model)
    {
        var order = _unitOfWork.Orders.GetFirstOrDefault(x => x.Id == model.Order.Id);

        Referral referralFulfillment = _unitOfWork.Referral.GetFirstOrDefault(x => x.ReferredId == order.ApplicationUserId && !x.IsFulfilled);

        if (referralFulfillment != null)
        {
            Wallet wallet = _unitOfWork.Wallets.GetFirstOrDefault(x => x.ApplicationUserId == referralFulfillment.ReferrerId);

            _unitOfWork.Wallets.UpdateBalance(referralFulfillment.ReferrerId, 5);
            //_unitOfWork.Save();

            Transaction transaction = new()
            {
                Amount = 50,
                TransactionDate = DateTime.Now,
                TransactionType = ProjectConstants.TRANSFER_CREDIT,
                WalletId = wallet.Id
            };
            _unitOfWork.Transactions.Add(transaction);
            //_unitOfWork.Save();

            _unitOfWork.Referral.UpdateFullFillmentStatus(referralFulfillment);
            _unitOfWork.Save();
        }
    }

    #region API CALLS

    [HttpGet]
    public IActionResult GetAll(string status)
    {
        IEnumerable<Order> orders;

        if (User.IsInRole(ProjectConstants.ADMIN))
        {
            orders = _unitOfWork.Orders.GetAllOrders();
        }
        else
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            orders = _unitOfWork.Orders.GetAllOrdersByCustomer(claim!.Value);
        }

        switch (status)
        {
            case "returns":
                orders = orders.Where(u => u.OrderStatus == ProjectConstants.STATUS_RETURN_APPROVED || u.OrderStatus == ProjectConstants.STATUS_RETURN_REQUESTED || u.OrderStatus == ProjectConstants.STATUS_RETURN_COMPLETE);
                break;
            case "pending":
                orders = orders.Where(u => u.OrderStatus == ProjectConstants.STATUS_PENDING);
                break;
            case "inprocess":
                orders = orders.Where(u => u.OrderStatus == ProjectConstants.STATUS_IN_PROCESS || u.OrderStatus == ProjectConstants.STATUS_APPROVED);
                break;
            case "completed":
                orders = orders.Where(u => u.OrderStatus == ProjectConstants.STATUS_DELIVERED);
                break;
            case "cancelled":
                orders = orders.Where(u => u.OrderStatus == ProjectConstants.STATUS_CANCELLED);
                break;
            case "shipped":
                orders = orders.Where(u => u.OrderStatus == ProjectConstants.STATUS_SHIPPED);
                break;
            default:
                break;
        }

        return Json(new { data = orders });
    }

    #endregion
}
