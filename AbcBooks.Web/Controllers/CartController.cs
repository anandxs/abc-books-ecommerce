using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Models.ViewModels;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stripe.Checkout;
using System.Security.Claims;

namespace AbcBooks.Web.Controllers
{
	[Authorize(Roles = ProjectConstants.CUSTOMER)]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly ILogger<CartController> _logger;

		public ShoppingCartViewModel ShoppingCartVM { get; set; }

		public CartController(
			IUnitOfWork unitOfWork,
			IWebHostEnvironment webHostEnvironment,
			IHttpContextAccessor httpContextAccessor,
			ILogger<CartController> logger)
		{
			_unitOfWork = unitOfWork;
			ShoppingCartVM = new();
			_webHostEnvironment = webHostEnvironment;
			_httpContextAccessor = httpContextAccessor;
			_logger = logger;
		}

		[HttpGet]
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			ShoppingCartVM.CartItems = _unitOfWork.ShoppingCart.GetUserShoppingCart(claim!.Value).ToList();
			ShoppingCartVM.Order = new();
			ShoppingCartVM.ProductImages = _unitOfWork.ProductImages.GetAll();

			foreach (var cart in ShoppingCartVM.CartItems)
			{
				cart.ListPrice = GetListPrice(cart.Product.Mrp, cart.Product.Discount);

				ShoppingCartVM.Order.OrderTotal += cart.ListPrice * cart.Quantity;
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Plus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart
				.GetFirstOrDefault(x => x.Id == cartId);

			if (cart is not null)
			{
				if (cart.Quantity >= _unitOfWork.Products.GetFirstOrDefault(x => x.Id == cart.ProductId).Stock)
				{
					TempData["OrderExceedsStock"] = $"Order exceeds stock!";
				}
				else
				{
					_unitOfWork.ShoppingCart.IncrementQuantity(cart, 1);
					_unitOfWork.Save();
				}
			}

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart
				.GetFirstOrDefault(x => x.Id == cartId);

			if (cart is not null)
			{
				if (cart.Quantity == 1)
				{
					Remove(cartId);
				}

				_unitOfWork.ShoppingCart.DecrementQuantity(cart, 1);
				_unitOfWork.Save();
			}

			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cart = _unitOfWork.ShoppingCart
				.GetFirstOrDefault(x => x.Id == cartId);

			if (cart is not null)
			{
				_unitOfWork.ShoppingCart.Remove(cart);
				_unitOfWork.Save();
			}

			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			HttpContext.Session.SetInt32(
					ProjectConstants.SESSION_CART,
					_unitOfWork.ShoppingCart
						.GetUserShoppingCart(claim!.Value).ToList().Count);

			return RedirectToAction(nameof(Index));
		}

		[HttpGet]
		public IActionResult Summary()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			var existingOrder = _unitOfWork.Orders
					.GetFirstOrDefault(
					x =>
						x.ApplicationUserId == claim!.Value &&
						x.OrderStatus == ProjectConstants.STATUS_PENDING &&
						x.PaymentStatus == ProjectConstants.PAYMENT_STATUS_PENDING &&
						x.PaymentMethod == ProjectConstants.PAYMENT_METHOD_ONLINE
					);

			try
			{
				if (existingOrder != null)
				{
					var prevService = new SessionService();
					prevService.Expire(existingOrder.SessionId);

					_unitOfWork.Orders.Remove(existingOrder);
					_unitOfWork.Save();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Stripe threw an exception!");
				_logger.LogError(ex.ToString());
			}

			ShoppingCartVM = new()
			{
				CartItems = _unitOfWork.ShoppingCart.GetUserShoppingCart(claim!.Value),
				Order = new(),
				AddressList = _unitOfWork.Addresses.GetAllAddressesByUser(claim!.Value).Select(x => new SelectListItem()
				{
					Text = $"{x.FirstName} {x.LastName}, {x.HouseNameOrNumber}, {x.StreetAddress}, {x.City}, {x.District}, PIN - {x.PinCode}, {x.State}",
					Value = x.Id.ToString()
				}),
				Wallet = _unitOfWork.Wallets.GetFirstOrDefault(x => x.ApplicationUserId == claim!.Value),
			};

			if (!ShoppingCartVM.CartItems.Any())
			{
				TempData["Invalid Operation"] = "There is nothing in cart to checkout!";
				return RedirectToAction("Browse", "Home");
			}

			foreach (var cart in ShoppingCartVM.CartItems)
			{
				cart.ListPrice = GetListPrice(cart.Product.Mrp, cart.Product.Discount);

				ShoppingCartVM.Order.OrderTotal += cart.ListPrice * cart.Quantity;
			}

			return View(ShoppingCartVM);
		}

		public IActionResult AddCoupon(ShoppingCartViewModel model, string? coupon)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			model = new()
			{
				CartItems = _unitOfWork.ShoppingCart.GetUserShoppingCart(claim!.Value),
				Order = new(),
				AddressList = _unitOfWork.Addresses.GetAllAddressesByUser(claim!.Value).Select(x => new SelectListItem()
				{
					Text = $"{x.FirstName} {x.LastName}, {x.HouseNameOrNumber}, {x.StreetAddress}, {x.City}, {x.District}, PIN - {x.PinCode}, {x.State}",
					Value = x.Id.ToString()
				}),
				Wallet = _unitOfWork.Wallets.GetFirstOrDefault(x => x.ApplicationUserId == claim!.Value),
			};

			foreach (var cart in model.CartItems)
			{
				cart.ListPrice = GetListPrice(cart.Product.Mrp, cart.Product.Discount);

				model.Order.OrderTotal += cart.ListPrice * cart.Quantity;
			}

			if (coupon is null || coupon == String.Empty)
			{
				TempData["InvalidCoupon"] = "Enter a coupon and then try again!";
				return View("Summary", model);
			}

			var dbCoupon = _unitOfWork.Coupons.GetCouponWithDiscountType(x => x.CouponCode == coupon);

			if (dbCoupon is null ||
				(dbCoupon is not null && dbCoupon.UsageLimit is not null && dbCoupon.UsesLeft == 0) ||
				(dbCoupon is not null && dbCoupon.ExpirateDate is not null && (dbCoupon.ExpirateDate < DateTime.Now)))
			{
				TempData["InvalidCoupon"] = "Invalid coupon!";
				return View("Summary", model);
			}

			var userCoupon = _unitOfWork.UserCoupons.GetFirstOrDefault(x => x.ApplicationUserId == claim!.Value && x.CouponId == dbCoupon!.Id);

			if (userCoupon is not null)
			{
				TempData["InvalidCoupon"] = "Invalid coupon!";
				return View("Summary", model);
			}

			if (model.Order.OrderTotal < dbCoupon!.MinimumPurchaseAmount)
			{
				TempData["InvalidCoupon"] = $"Add {(model.Order.OrderTotal - dbCoupon.MinimumPurchaseAmount).ToString("c")} worth to cart to avail this coupon";
				return View("Summary", model);
			}

			model.CouponCode = coupon;
			model.PreDiscountOrderTotal = model.Order.OrderTotal;

			switch (dbCoupon.DiscountType.Value)
			{
				case "FLAT":
					model.Order.OrderTotal -= dbCoupon.DiscountValue;
					break;

				case "PERCENTAGE":
					var discountAmount = model.Order.OrderTotal * dbCoupon.DiscountValue / 100;
					discountAmount = (float)(discountAmount > dbCoupon.MaxAmount ? dbCoupon.MaxAmount : discountAmount);
					model.Order.OrderTotal -= discountAmount;
					break;

				default:
					break;
			}

			TempData["CouponInfo"] = $"{dbCoupon.Description}";

			return View("Summary", model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Summary(ShoppingCartViewModel model, string paymentMethod, string couponCode)
		{
			int errorCount = 0;

			if (paymentMethod is null)
			{
				TempData["InvalidPayment"] = "Choose a payment method";
				errorCount++;
			}

			if (model.Order.ShippingAddressId is 0)
			{
				TempData["InvalidShippingAddress"] = "Choose a shipping address";
				errorCount++;
			}

			if (errorCount > 0)
			{
				return RedirectToAction(nameof(Summary));
			}

			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			model.CartItems = _unitOfWork.ShoppingCart.GetUserShoppingCart(claim!.Value).ToList();

			int productStock = 1;
			foreach (var item in model.CartItems)
			{
				var product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == item.ProductId);

				if (item.Quantity > product.Stock)
				{
					if (product.Stock is 0)
					{
						TempData[$"error{productStock}"] = $"{product.Title} is out of stock!";
					}
					else
					{
						TempData[$"error{productStock}"] = $"Only {product.Stock} of {product.Title} left in stock";
					}
					productStock++;
				}
			}

			if (productStock > 1)
			{
				return RedirectToAction(nameof(Summary));
			}

			switch (paymentMethod)
			{
				case ProjectConstants.PAYMENT_METHOD_CASH_ON_DELIVERY:
					model.Order.PaymentMethod = ProjectConstants.PAYMENT_METHOD_CASH_ON_DELIVERY;
					break;

				case ProjectConstants.PAYMENT_METHOD_WALLET:
					model.Order.PaymentMethod = ProjectConstants.PAYMENT_METHOD_WALLET;
					break;

				case ProjectConstants.PAYMENT_METHOD_ONLINE:
					model.Order.PaymentMethod = ProjectConstants.PAYMENT_METHOD_ONLINE;
					break;

				default:
					break;
			}

			model.Order.PaymentStatus = ProjectConstants.PAYMENT_STATUS_PENDING;
			model.Order.OrderStatus = ProjectConstants.STATUS_PENDING;
			model.Order.OrderDate = DateTime.Now;
			model.Order.ApplicationUserId = claim.Value;

			int itemCount = 0;

			foreach (var cart in model.CartItems)
			{
				cart.ListPrice = GetListPrice(cart.Product.Mrp, cart.Product.Discount);

				model.Order.OrderTotal += cart.ListPrice * cart.Quantity;
				itemCount++;
			}

			float discountAmount = 0;
			UserCoupon userCoupon = new();

			if (couponCode is not null)
			{
				var dbCoupon = _unitOfWork.Coupons.GetCouponWithDiscountType(x => x.CouponCode == couponCode);

				if (dbCoupon is null ||
					(dbCoupon is not null && dbCoupon.UsageLimit is not null && dbCoupon.UsesLeft <= 0) ||
					(dbCoupon is not null && dbCoupon.ExpirateDate is not null && (dbCoupon.ExpirateDate < DateTime.Now)))
				{
					TempData["InvalidCoupon"] = "Invalid coupon!";
					return View("Summary", model);
				}

				if (model.Order.OrderTotal < dbCoupon!.MinimumPurchaseAmount)
				{
					TempData["InvalidCoupon"] = $"Add {(model.Order.OrderTotal - dbCoupon.MinimumPurchaseAmount).ToString("c")} worth to cart to avail this coupon";
					return View("Summary", model);
				}

				switch (dbCoupon.DiscountType.Value)
				{
					case "FLAT":
						model.Order.OrderTotal -= dbCoupon.DiscountValue;
						break;

					case "PERCENTAGE":
						discountAmount = model.Order.OrderTotal * dbCoupon.DiscountValue / 100;
						discountAmount = (float)(discountAmount > dbCoupon.MaxAmount ? dbCoupon.MaxAmount : discountAmount);
						model.Order.OrderTotal -= discountAmount;
						break;

					default:
						break;
				}

				if (dbCoupon.UsageLimit is not null)
				{
					_unitOfWork.Coupons.DecrementUsesLeft(dbCoupon.Id);
					_unitOfWork.Save();
				}

				userCoupon.CouponId = dbCoupon.Id;
				userCoupon.ApplicationUserId = claim!.Value;
			}

			if (model.Order.PaymentMethod == ProjectConstants.PAYMENT_METHOD_WALLET)
			{
				Wallet wallet = _unitOfWork.Wallets.GetFirstOrDefault(x => x.ApplicationUserId == claim.Value);

				if (model.Order.OrderTotal > wallet.Balance)
				{
					TempData["InsufficientFunds"] = "You do not have enough credit in wallet";
					return View();
				}

				_unitOfWork.Wallets.UpdateBalance(claim.Value, model.Order.OrderTotal, false);
				_unitOfWork.Save();

				Transaction transaction = new()
				{
					WalletId = wallet.Id,
					Amount = model.Order.OrderTotal,
					TransactionDate = DateTime.Now,
					TransactionType = ProjectConstants.TRANSFER_DEBIT
				};

				_unitOfWork.Transactions.Add(transaction);
				_unitOfWork.Save();

				model.Order.PaymentStatus = ProjectConstants.PAYMENT_STATUS_APPROVED;
				model.Order.OrderStatus = ProjectConstants.STATUS_APPROVED;
				model.Order.PaymentDate = DateTime.Now;
			}

			if (paymentMethod == ProjectConstants.PAYMENT_METHOD_ONLINE)
			{
				float deduction = 0;

				if (couponCode is not null)
				{
					deduction = discountAmount / itemCount;
				}

				foreach (var cart in model.CartItems)
				{
					cart.ListPrice -= (deduction / cart.Quantity);
				}

				_unitOfWork.Orders.Add(model.Order);
				_unitOfWork.Save();

				foreach (var cart in model.CartItems)
				{
					OrderDetail orderDetail = new()
					{
						OrderId = model.Order.Id,
						ProductId = cart.ProductId,
						Price = cart.ListPrice,
						Quantity = cart.Quantity
					};

					_unitOfWork.OrderDetails.Add(orderDetail);
					_unitOfWork.Save();
				}

				var httpContext = _httpContextAccessor.HttpContext;
				var domain = $"https://{httpContext!.Request.Host.Value}/";

				var options = new SessionCreateOptions
				{
					LineItems = new List<SessionLineItemOptions>(),
					Mode = "payment",
					SuccessUrl = domain + $"cart/OrderConfirmation?id={model.Order.Id}",
					CancelUrl = domain + $"cart/OrderCancelled"
				};

				foreach (var item in model.CartItems)
				{
					var sessionLineItem = new SessionLineItemOptions
					{
						PriceData = new SessionLineItemPriceDataOptions
						{
							UnitAmount = (long)(item.ListPrice * 100),
							Currency = "usd",
							ProductData = new SessionLineItemPriceDataProductDataOptions
							{
								Name = item.Product.Title
							},

						},
						Quantity = item.Quantity,
					};

					options.LineItems.Add(sessionLineItem);
				}

				var service = new SessionService();
				Session session = service.Create(options);

				_unitOfWork.Orders.UpdateStripePaymentId(model.Order.Id, session.Id, session.PaymentIntentId);
				_unitOfWork.Save();

				if (couponCode is not null)
				{
					userCoupon.OrderId = model.Order.Id;
					_unitOfWork.UserCoupons.Add(userCoupon);
					_unitOfWork.Save();
				}

				TempData["firstTime"] = true;

				Response.Headers.Add("Location", session.Url);
				return new StatusCodeResult(303);
			}

			_unitOfWork.Orders.Add(model.Order);
			_unitOfWork.Save();

			foreach (var cart in model.CartItems)
			{
				OrderDetail orderDetail = new()
				{
					OrderId = model.Order.Id,
					ProductId = cart.ProductId,
					Price = cart.ListPrice,
					Quantity = cart.Quantity
				};

				_unitOfWork.Products.UpdateStock(orderDetail.ProductId, orderDetail.Quantity);
				_unitOfWork.Save();

				_unitOfWork.OrderDetails.Add(orderDetail);
				_unitOfWork.Save();
			}

			_unitOfWork.ShoppingCart.RemoveRange(model.CartItems);
			_unitOfWork.Save();

			if (couponCode is not null)
			{
				userCoupon.OrderId = model.Order.Id;
				_unitOfWork.UserCoupons.Add(userCoupon);
				_unitOfWork.Save();
			}

			HttpContext.Session.Clear();

			TempData["firstTime"] = true;

			return RedirectToAction(nameof(OrderConfirmation), new { Id = model.Order.Id });
		}

		public IActionResult OrderConfirmation(int id)
		{
			if (TempData["firstTime"] is null)
			{
				return BadRequest();
			}

			var order = _unitOfWork.Orders.GetFirstOrDefault(x => x.Id == id);

			if (order.PaymentMethod is ProjectConstants.PAYMENT_METHOD_CASH_ON_DELIVERY || order.PaymentMethod is ProjectConstants.PAYMENT_METHOD_WALLET)
			{
				return View(id);
			}

			var service = new SessionService();
			Session? session = service.Get(order.SessionId);

			if (session is not null && session.PaymentStatus.ToLower() == "paid")
			{
				_unitOfWork.Orders.UpdateStripePaymentId(order.Id, order.SessionId!, order.PaymentIntendId!);
				_unitOfWork.Save();

				_unitOfWork.Orders.UpdateStatus(order.Id, ProjectConstants.STATUS_APPROVED, ProjectConstants.PAYMENT_STATUS_APPROVED);
				_unitOfWork.Save();

				List<ShoppingCart> shoppingCarts = _unitOfWork.ShoppingCart.GetUserShoppingCart(order.ApplicationUserId).ToList();

				foreach (var cart in shoppingCarts)
				{
					_unitOfWork.Products.UpdateStock(cart.ProductId, cart.Quantity);
					_unitOfWork.Save();
				}

				HttpContext.Session.Clear();

				_unitOfWork.ShoppingCart.RemoveRange(shoppingCarts);

				_unitOfWork.Save();
				return View(id);
			}
			else if (session is not null && session.PaymentStatus.ToLower() == "unpaid")
			{
				TempData["wasRedirect"] = true;

				return RedirectToAction(nameof(OrderCancelled));
			}

			return BadRequest();
		}

		[HttpGet]
		public IActionResult OrderCancelled()
		{
			if (TempData["firstTime"] is null)
			{
				return BadRequest();
			}

			return View();
		}

		private float GetListPrice(float mrp, float discount)
		{
			float listPrice = mrp;

			if (discount is not 0)
			{
				float t = discount / 100;
				float deduction = mrp * t;
				listPrice -= deduction;
			}

			return listPrice;
		}
	}
}
