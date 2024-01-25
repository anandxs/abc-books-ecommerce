using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Previewer;
using System.Data;

namespace AbcBooks.Web.Controllers
{
	public class DownloadController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public DownloadController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
			QuestPDF.Settings.License = LicenseType.Community;
		}



		public IActionResult DownloadOrderInvoice(int orderId)
		{
			Order order = _unitOfWork.Orders.GetOrderWithCustomer(orderId);

			float preDiscountTotal;
			DataTable dataTable = GetBillAsTable(orderId, out preDiscountTotal);

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(50);

					page.Header().Element(ComposeHeader);

					void ComposeHeader(IContainer container)
					{
						var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);

						container.Row(row =>
						{
							row.RelativeItem().Column(column =>
							{
								column.Item().Text($"Invoice # {orderId}").Style(titleStyle);

								column.Item().Text(text =>
								{
									text.Span("Order date: ").SemiBold();
									text.Span($"{order.OrderDate}");
								});

								if (order.ShippingDate != new DateTime())
								{
									column.Item()
										.Text(text =>
										{
											text.Span("Shipping date: ").SemiBold();
											text.Span($"{order.ShippingDate}");
										});
								}

								if (order.DeliveryDate != new DateTime())
								{
									column.Item()
										.Text(text =>
										{
											text.Span("Delivery date: ").SemiBold();
											text.Span($"{order.DeliveryDate}");
										});
								}

								column.Item().Text(text =>
								{
									text.Span("Payment Method: ").SemiBold();
									text.Span($"{order.PaymentMethod}");
								});

								column.Item().Text(text =>
								{
									text.Span("Payment status: ").SemiBold();
									text.Span($"{order.PaymentStatus}");
								});

								column.Item().Text(text =>
								{
									text.Span("Order status: ").SemiBold();
									text.Span($"{order.OrderStatus}");
								});
							});

							row.ConstantItem(100)
								.Text($"ABC BOOKS")
								.Bold();
						});
					}

					page.Content().Element(ComposeContent);

					void ComposeContent(IContainer container)
					{
						container.Column(column =>
						{
							column.Spacing(10);

							column.Item().Element(ComposeAddress);
							column.Item().Element(ComposeTable);
							column.Item()
								.Height(50)
								.AlignMiddle()
								.AlignCenter()
								.Text("Thank you for your business!").Italic().Bold();
						});
					}

					void ComposeTable(IContainer container)
					{
						container.Table(table =>
						{
							table.ColumnsDefinition(x =>
							{
								x.RelativeColumn();
								x.RelativeColumn();
								x.RelativeColumn();
								x.RelativeColumn();
								x.RelativeColumn();
							});

							table.Cell()
								.Element(HeaderBlock)
								.Text("Sl. No.");

							table.Cell()
								.Element(HeaderBlock)
								.Text("Item");

							table.Cell()
								.Element(HeaderBlock)
								.Text("Price");

							table.Cell()
								.Element(HeaderBlock)
								.Text("Quantity");

							table.Cell()
								.Element(HeaderBlock)
								.Text("Subtotal");

							foreach (DataRow row in dataTable.Rows)
							{
								foreach (DataColumn column in dataTable.Columns)
								{
									table.Cell()
										.Element(CellBlock)
										.Text(row[column].ToString());
								}
							}

							if (preDiscountTotal != order.OrderTotal)
							{
								var temp = _unitOfWork.UserCoupons.GetUserCouponWithCoupon(order.ApplicationUserId, orderId);

								table.Cell()
									.ColumnSpan(3)
									.Element(SubheaderBlock)
									.Text(text =>
									{
										text.Span("Coupon Applied : ").Bold();
										text.Span($"{temp.Coupon.CouponCode}");
									});

								table.Cell()
									.Element(CellBlock)
									.Text($"Discount");

								table.Cell()
									.Element(CellBlock)
									.Text($"-{preDiscountTotal - order.OrderTotal}");
							}

							table.Cell()
									.ColumnSpan(4)
									.Element(SubheaderBlock)
									.Text("Order Total");

							table.Cell()
									.Element(CellBlock)
									.Text($"{order.OrderTotal:c}");
						});
					}

					static IContainer HeaderBlock(IContainer container)
					{
						return container
							.Border(0.5f)
							.Background(Colors.Grey.Lighten3)
							.ShowOnce()
							.AlignCenter()
							.AlignMiddle();
					}

					static IContainer SubheaderBlock(IContainer container)
					{
						return container
							.Border(0.5f)
							.Background(Colors.Grey.Lighten3)
							.ShowOnce()
							.AlignRight()
							.AlignMiddle()
							.PaddingRight(10);
					}

					static IContainer CellBlock(IContainer container)
					{
						return container
							.Border(0.5f)
							.AlignCenter()
							.AlignMiddle();
					}

					void ComposeAddress(IContainer container)
					{
						container.PaddingTop(40).Row(row =>
						{
							row.RelativeItem()
								.Text(text =>
								{
									text.Span("SHIPPING ADDRESS").Bold();
									text.Span($"\n{order.ShippingAddress.FirstName} {order.ShippingAddress.LastName}");
									text.Span($"\n{order.ShippingAddress.HouseNameOrNumber}");
									text.Span($"\n{order.ShippingAddress.StreetAddress}");
									text.Span($"\n{order.ShippingAddress.District}");
									text.Span($"\n{order.ShippingAddress.PinCode}");
									text.Span($"\n{order.ShippingAddress.State}");
								});

							row.RelativeItem()
								.Text(text =>
								{
									text.Span("BILLING ADDRESS").Bold();
									text.Span($"\n{order.ShippingAddress.FirstName} {order.ShippingAddress.LastName}");
									text.Span($"\n{order.ShippingAddress.HouseNameOrNumber}");
									text.Span($"\n{order.ShippingAddress.StreetAddress}");
									text.Span($"\n{order.ShippingAddress.District}");
									text.Span($"\n{order.ShippingAddress.PinCode}");
									text.Span($"\n{order.ShippingAddress.State}");
								});
						});
					}

					page.Footer()
						.AlignCenter()
						.Text(x =>
						{
							x.CurrentPageNumber();
							x.Span(" / ");
							x.TotalPages();
						});
				});
			});

			using (var stream = new MemoryStream())
			{
				document.GeneratePdf(stream);
				return File(stream.ToArray(), "application/pdf", $"Invoice-{order.Id}.pdf");
			}
		}

		private DataTable GetBillAsTable(int orderId, out float preDiscountTotal)
		{
			preDiscountTotal = 0;

			var orderDetails = _unitOfWork.OrderDetails.GetOrderDetailsWithProducts(orderId);

			DataTable dt = new DataTable("Grid");
			dt.Columns.AddRange(new DataColumn[5]
			{
				new DataColumn("Sl. no."),
				new DataColumn("Item"),
				new DataColumn("Price"),
				new DataColumn("Quantity"),
				new DataColumn("Subtotal")
			});

			int i = 1;
			foreach (var orderDetail in orderDetails)
			{
				preDiscountTotal += (orderDetail.Price * orderDetail.Quantity);
				dt.Rows.Add(
					i,
					orderDetail.Product.Title,
					orderDetail.Price,
					orderDetail.Quantity,
					(orderDetail.Price * orderDetail.Quantity)
					);
				i++;
			}

			return dt;
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ProjectConstants.ADMIN)]
		public IActionResult DownloadSalesReportAsExcel(DateOnly fromDate, DateOnly toDate, string filter)
		{
			float revenue;
			DataTable dt = GetOrderListTable(fromDate, toDate, filter, out revenue);

			using (var workbook = new XLWorkbook())
			{
				//workbook.Worksheets.Add(dt, "Sales Report", $"SALES REPORT {fromDate} - {toDate}");

				var worksheet = workbook.Worksheets.Add($"{filter} Sales Report");

				var headingRange = worksheet.Range("C1:H1");
				headingRange.Merge();

				var headingCell = worksheet.Cell("C1");

				headingCell.Value = $"SALES REPORT - {filter.ToUpper()} - {fromDate} to {toDate}";
				headingCell.Style.Font.Bold = true;
				headingCell.Style.Font.FontSize = 16;
				headingCell.Style.Fill.BackgroundColor = XLColor.BlizzardBlue;

				worksheet.Range("J1:L1").Merge();
				worksheet.Range("J2:L2").Merge();
				var totalRevenueHeader = worksheet.Cell("J1");
				totalRevenueHeader.Value = "TOTAL REVENUE";
				totalRevenueHeader.Style.Font.Bold = true;
				totalRevenueHeader.Style.Font.FontSize = 16;
				totalRevenueHeader.Style.Fill.BackgroundColor = XLColor.BlizzardBlue;

				var totalRevenue = worksheet.Cell("J2");
				totalRevenue.Value = $"{revenue.ToString("c")}";
				totalRevenue.Style.Font.Bold = true;
				totalRevenue.Style.Font.FontSize = 16;

				var tableStartingCell = worksheet.Cell("C3");
				var table = worksheet.Cell(tableStartingCell.Address).InsertTable(dt);

				using (MemoryStream stream = new MemoryStream())
				{
					workbook.SaveAs(stream);
					return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"SALES_REPORT-{filter.ToUpper()}-{fromDate} to {toDate}.xlsx");
				}
			}
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = ProjectConstants.ADMIN)]
		public IActionResult DownloadSalesReportAsPdf(DateOnly fromDate, DateOnly toDate, string filter)
		{
			float totalRev;
			DataTable dt = GetOrderListTable(fromDate, toDate, filter, out totalRev);

			var document = Document.Create(container =>
			{
				container.Page(page =>
				{
					page.Margin(50);

					page.Header()
						.AlignCenter()
						.Text($"SALES REPORT - {filter} - ({fromDate} to {toDate})")
						.FontSize(24)
						.Bold()
						.Underline();

					page.Content().Element(ComposeContent);

					void ComposeContent(IContainer container)
					{
						container
							.PaddingTop(20)
							.Column(column =>
							{
								column.Spacing(10);

								column.Item()
									.Text($"TOTAL REVENUE : {totalRev:c}")
									.Bold()
									.FontSize(16);

								column.Item().Element(ComposeTable);
							});
					}

					void ComposeTable(IContainer container)
					{
						container.Table(table =>
						{
							table.ColumnsDefinition(x =>
							{
								x.ConstantColumn(40);
								x.RelativeColumn();
								x.ConstantColumn(50);
								x.ConstantColumn(60);
								x.ConstantColumn(50);
								x.ConstantColumn(100);
							});

							table.Cell()
								.Element(HeaderBlock)
								.Text("Order Id");
							table.Cell()
								.Element(HeaderBlock)
								.Text("Customer Id");
							table.Cell()
								.Element(HeaderBlock)
								.Text("Product Id");
							table.Cell()
								.Element(HeaderBlock)
								.Text("Quantity");
							table.Cell()
								.Element(HeaderBlock)
								.Text("Price");
							table.Cell()
								.Element(HeaderBlock)
								.Text("Payment Method");

							foreach (DataRow row in dt.Rows)
							{
								foreach (DataColumn column in dt.Columns)
								{
									table.Cell()
										.Element(CellBlock)
										.Text(row[column].ToString());
								}
							}
						});
					}

					static IContainer HeaderBlock(IContainer container)
					{
						return container
							.Border(0.5f)
							.Background(Colors.Grey.Lighten3)
							.ShowOnce()
							.AlignCenter()
							.AlignMiddle();
					}

					static IContainer CellBlock(IContainer container)
					{
						return container
							.Border(0.5f)
							.AlignCenter()
							.AlignMiddle();
					}

					page.Footer()
						.AlignCenter()
						.Text(x =>
						{
							x.CurrentPageNumber();
							x.Span(" / ");
							x.TotalPages();
						});
				});
			});

			using (var stream = new MemoryStream())
			{
				document.GeneratePdf(stream);
				return File(stream.ToArray(), "application/pdf", $"SALES_REPORT-{filter.ToUpper()}-{fromDate} to {toDate}.pdf");
			}
		}

		private DataTable GetOrderListTable(DateOnly fromDate, DateOnly toDate, string filter, out float totalRev)
		{
			totalRev = 0;
			List<Order> orders;

			switch (filter)
			{
				case "Delivered":
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).Where(x => x.OrderStatus == ProjectConstants.STATUS_DELIVERED).ToList();
					break;

				case "Shipped":
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).Where(x => x.OrderStatus == ProjectConstants.STATUS_SHIPPED).ToList();
					break;

				case "In Process":
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).Where(x => x.OrderStatus == ProjectConstants.STATUS_IN_PROCESS).ToList();
					break;

				case "Pending":
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).Where(x => x.OrderStatus == ProjectConstants.STATUS_PENDING).ToList();
					break;

				case "Cancelled":
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).Where(x => x.OrderStatus == ProjectConstants.STATUS_CANCELLED).ToList();
					break;

				case "Returns":
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).Where(x =>
					x.OrderStatus == ProjectConstants.STATUS_RETURN_REQUESTED || x.OrderStatus == ProjectConstants.STATUS_RETURN_APPROVED || x.OrderStatus == ProjectConstants.STATUS_RETURN_COMPLETE).ToList();
					break;

				default:
					orders = _unitOfWork.Orders.GetAllOrdersBetweenDates(fromDate, toDate).ToList();
					break;
			}

			List<OrderDetail> orderDetails = new();
			foreach (var order in orders)
			{
				var orderDetailsForId = _unitOfWork.OrderDetails.GetOrderDetailsWithProducts(order.Id);
				orderDetails.AddRange(orderDetailsForId);
			}

			DataTable dt = new DataTable("Grid");
			dt.Columns.AddRange(new DataColumn[6]
			{
				new DataColumn("Order Id"),
				new DataColumn("Customer Id"),
				new DataColumn("Product Id"),
				new DataColumn("Quantity"),
				new DataColumn("Price"),
				new DataColumn("Payment Method")
			});

			foreach (var orderDetail in orderDetails)
			{
				var order = _unitOfWork.Orders.GetFirstOrDefault(x => x.Id == orderDetail.OrderId);

				totalRev += orderDetail.Price;

				dt.Rows.Add(
					orderDetail.OrderId,
					order.ApplicationUserId,
					orderDetail.ProductId,
					orderDetail.Quantity,
					orderDetail.Price.ToString("c"),
					order.PaymentMethod
					);
			}

			return dt;
		}
	}
}
