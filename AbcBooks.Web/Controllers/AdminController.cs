using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcBooks.Web.Controllers;

[Authorize(Roles = ProjectConstants.ADMIN)]
public class AdminController : Controller
{
    private readonly IUnitOfWork _unitOfWork;

    public AdminController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public IActionResult Index()
    {
        Dashboard model = new();

        model.TotalRevenue = _unitOfWork.Orders.GetAll().Sum(x => x.OrderTotal);
        model.LowStock = _unitOfWork.Products.GetAll().Count(x => !x.IsDeleted && x.Stock <= 10);
        model.BlockedUsers = _unitOfWork.ApplicationUsers.GetAll().Where(x => x.IsBlocked).Count();
        model.PendingOrders = _unitOfWork.Orders.GetAll().Where(x => x.OrderStatus == ProjectConstants.STATUS_PENDING).Count();

        return View(model);
    }

    #region API CALLS

    [HttpGet]
    public IActionResult YearlySales()
    {
        List<YearlySales> data = new()
        {
            new YearlySales { Year = DateTime.Now.Year - 4 },
            new YearlySales { Year = DateTime.Now.Year - 3 },
            new YearlySales { Year = DateTime.Now.Year - 2 },
            new YearlySales { Year = DateTime.Now.Year - 1 },
            new YearlySales { Year = DateTime.Now.Year },
        };

        var date = DateTime.Now;

        var orders = _unitOfWork.Orders.GetAllOrders()
            .Where(x =>
            x.OrderDate >= new DateTime(date.Year - 4, 1, 1) &&
            x.OrderDate <= new DateTime(date.Year, 12, 31) &&
            (x.PaymentStatus == ProjectConstants.PAYMENT_STATUS_APPROVED ||
            x.PaymentStatus == ProjectConstants.PAYMNET_STATUS_REFUND_COMPLETE ||
            x.PaymentStatus == ProjectConstants.PAYMNET_STATUS_REFUND_INITIATED)).ToList();

        foreach (var order in orders)
        {
            if (order.OrderDate.Year == date.Year - 4)
            {
                data[0].Revenue += order.OrderTotal;
            }
            else if (order.OrderDate.Year == date.Year - 3)
            {
                data[1].Revenue += order.OrderTotal;
            }
            else if (order.OrderDate.Year == date.Year - 2)
            {
                data[2].Revenue += order.OrderTotal;
            }
            else if (order.OrderDate.Year == date.Year - 1)
            {
                data[3].Revenue += order.OrderTotal;
            }
            else if (order.OrderDate.Year == date.Year)
            {
                data[4].Revenue += order.OrderTotal;
            }
        }

        return Json(new { data });
    }

    [HttpGet]
    public IActionResult MonthlySales()
    {
        List<MonthlySales> data = new()
        {
            new MonthlySales { Month = "January" },
            new MonthlySales { Month = "February" },
            new MonthlySales { Month = "March" },
            new MonthlySales { Month = "April" },
            new MonthlySales { Month = "May" },
            new MonthlySales { Month = "June" },
            new MonthlySales { Month = "July" },
            new MonthlySales { Month = "August" },
            new MonthlySales { Month = "September" },
            new MonthlySales { Month = "October" },
            new MonthlySales { Month = "November" },
            new MonthlySales { Month = "December" }
        };

        var date = DateTime.Now;

        var orders = _unitOfWork.Orders.GetAllOrders()
            .Where(x =>
            x.OrderDate >= new DateTime(date.Year, 1, 1) &&
            x.OrderDate <= new DateTime(date.Year + 1, 1, 1) &&
            (x.PaymentStatus == ProjectConstants.PAYMENT_STATUS_APPROVED ||
            x.PaymentStatus == ProjectConstants.PAYMNET_STATUS_REFUND_COMPLETE ||
            x.PaymentStatus == ProjectConstants.PAYMNET_STATUS_REFUND_INITIATED)).ToList();

        foreach (var order in orders)
        {
            switch (order.OrderDate.Month)
            {
                case 1:
                    data[0].Revenue += order.OrderTotal;
                    break;

                case 2:
                    data[1].Revenue += order.OrderTotal;
                    break;

                case 3:
                    data[2].Revenue += order.OrderTotal;
                    break;

                case 4:
                    data[3].Revenue += order.OrderTotal;
                    break;

                case 5:
                    data[4].Revenue += order.OrderTotal;
                    break;

                case 6:
                    data[5].Revenue += order.OrderTotal;
                    break;

                case 7:
                    data[6].Revenue += order.OrderTotal;
                    break;

                case 8:
                    data[7].Revenue += order.OrderTotal;
                    break;

                case 9:
                    data[8].Revenue += order.OrderTotal;
                    break;

                case 10:
                    data[9].Revenue += order.OrderTotal;
                    break;

                case 11:
                    data[10].Revenue += order.OrderTotal;
                    break;

                case 12:
                    data[11].Revenue += order.OrderTotal;
                    break;

                default:
                    break;
            }
        }

        return Json(new { data });
    }

    [HttpGet]
    public JsonResult DailySales(DateOnly fromDate, DateOnly toDate)
    {
        fromDate = new DateOnly(2023, 7, 1);
        toDate = new DateOnly(2023, 9, 30);

        var orders = _unitOfWork.Orders
        .GetAllOrdersBetweenDates(fromDate, toDate);

        List<DailySales> data = new();

        foreach (var order in orders)
        {
            var orderDate = new DateOnly(
                order.OrderDate.Year,
                order.OrderDate.Month,
                order.OrderDate.Day);

            if (orderDate >= fromDate && orderDate <= toDate)
            {
                var entry = data.FirstOrDefault(x => x.Date == orderDate);

                if (entry is null)
                {
                    data.Add(new DailySales
                    {
                        Date = orderDate
                    });
                }
                else
                {
                    entry.Revenue += order.OrderTotal;
                }
            }
        }

        return Json(new { data });
    }

    [HttpGet]
    public IActionResult RevenueByCategory()
    {
        List<CategoryRevenue> data = new();

        var categories = _unitOfWork.Categories.GetAll();

        foreach (var category in categories)
        {
            data.Add(new CategoryRevenue
            {
                Category = category.Name
            });
        }

        var orders = _unitOfWork.Orders.GetAllOrders()
            .Where(x =>
                (x.PaymentStatus == ProjectConstants.PAYMENT_STATUS_APPROVED ||
                x.PaymentStatus == ProjectConstants.PAYMNET_STATUS_REFUND_COMPLETE ||
                x.PaymentStatus == ProjectConstants.PAYMNET_STATUS_REFUND_INITIATED)).ToList();

        foreach (var order in orders)
        {
            var orderDetails = _unitOfWork.OrderDetails.GetOrderDetailsWithProducts(order.Id).ToList();

            foreach (var item in orderDetails)
            {
                var product = _unitOfWork.Products.GetProductWithCategory(item.ProductId);

                if (product is not null)
                {
                    var x = data.Find(x => x.Category == product.Category.Name);
                    x.Revenue += item.Price;
                }
            }
        }

        return Json(new { data });
    }

    #endregion
}
