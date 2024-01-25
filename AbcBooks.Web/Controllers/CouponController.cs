using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Models.ViewModels;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AbcBooks.Web.Controllers
{
	[Authorize(Roles = ProjectConstants.ADMIN)]
	public class CouponController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CouponController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Create()
		{
			CouponViewModel model = new()
			{
				Coupon = new(),
				DiscountTypes = _unitOfWork.DiscountTypes
					.GetAll()
					.Select(x => new SelectListItem
					{
						Text = x.Value,
						Value = x.Id.ToString()
					})
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(CouponViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Coupon.UsageLimit is not null)
				{
					model.Coupon.UsesLeft = model.Coupon.UsageLimit;
				}

				var discountType = _unitOfWork.DiscountTypes.GetFirstOrDefault(x => x.Id == model.Coupon.DiscountTypeId);

				if (discountType.Value == "PERCENTAGE")
				{
					bool err = false;

					if (model.Coupon.MaxAmount is null)
					{
						ModelState.AddModelError(String.Empty, "Please add a maximum discount amount");

						err = true;
					}

					if (model.Coupon.DiscountValue >= 100 || model.Coupon.DiscountValue < 0)
					{
						ModelState.AddModelError(String.Empty, "Discount percentage can only be between 1 and 100");

						err = true;
					}

					if (err)
					{
						model.DiscountTypes = _unitOfWork.DiscountTypes
							.GetAll()
							.Select(x => new SelectListItem
							{
								Text = x.Value,
								Value = x.Id.ToString()
							});

						return View(model);
					}
				}

				if (discountType.Value == "FLAT" && model.Coupon.DiscountValue > model.Coupon.MinimumPurchaseAmount)
				{
					ModelState.AddModelError(String.Empty, "Discount value must be less than minimum purhcase amount");

					model.DiscountTypes = _unitOfWork.DiscountTypes
						.GetAll()
						.Select(x => new SelectListItem
						{
							Text = x.Value,
							Value = x.Id.ToString()
						});

					return View(model);
				}

				_unitOfWork.Coupons.Add(model.Coupon);
				_unitOfWork.Save();

				return RedirectToAction(nameof(Index));
			}

			model.DiscountTypes = _unitOfWork.DiscountTypes
					.GetAll()
					.Select(x => new SelectListItem
					{
						Text = x.Value,
						Value = x.Id.ToString()
					});

			return View(model);
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			CouponViewModel model = new()
			{
				Coupon = _unitOfWork.Coupons.GetFirstOrDefault(x => x.Id == id),
				DiscountTypes = _unitOfWork.DiscountTypes
					.GetAll()
					.Select(x => new SelectListItem
					{
						Text = x.Value,
						Value = x.Id.ToString()
					})
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(CouponViewModel model)
		{
			if (ModelState.IsValid)
			{
				if (model.Coupon.UsageLimit is not null)
				{
					model.Coupon.UsesLeft = model.Coupon.UsageLimit;
				}

				var discountType = _unitOfWork.DiscountTypes.GetFirstOrDefault(x => x.Id == model.Coupon.DiscountTypeId);

				if (discountType.Value == "PERCENTAGE")
				{
					bool err = false;

					if (model.Coupon.MaxAmount is null)
					{
						ModelState.AddModelError(String.Empty, "Please add a maximum discount amount");

						err = true;
					}

					if (model.Coupon.DiscountValue >= 100 || model.Coupon.DiscountValue < 0)
					{
						ModelState.AddModelError(String.Empty, "Discount percentage can only be between 1 and 100");

						err = true;
					}

					if (err)
					{
						model.DiscountTypes = _unitOfWork.DiscountTypes
							.GetAll()
							.Select(x => new SelectListItem
							{
								Text = x.Value,
								Value = x.Id.ToString()
							});

						return View(model);
					}
				}

				if (discountType.Value == "FLAT" && model.Coupon.DiscountValue > model.Coupon.MinimumPurchaseAmount)
				{
					ModelState.AddModelError(String.Empty, "Discount value must be less than minimum purhcase amount");

					model.DiscountTypes = _unitOfWork.DiscountTypes
						.GetAll()
						.Select(x => new SelectListItem
						{
							Text = x.Value,
							Value = x.Id.ToString()
						});

					return View(model);
				}

				_unitOfWork.Coupons.Update(model.Coupon);
				_unitOfWork.Save();

				return RedirectToAction(nameof(Index));
			}

			return View(model.Coupon.Id);
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			IEnumerable<Coupon> coupons = _unitOfWork.Coupons.GetAllCouponsWithDiscountType();

			return Json(new { data = coupons });
		}

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			Coupon coupon = _unitOfWork.Coupons.GetFirstOrDefault(x => x.Id == id);

			if (coupon is null)
			{
				return Json(new { success = false, message = "Error while deleting" });
			}

			_unitOfWork.Coupons.Remove(coupon);
			_unitOfWork.Save();

			return Json(new { success = true, message = "Successfully deleted coupon!" });
		}

		#endregion
	}
}
