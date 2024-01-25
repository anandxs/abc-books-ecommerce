using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcBooks.Web.Controllers
{
    [Authorize(Roles = ProjectConstants.ADMIN)]
	public class CategoryController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public CategoryController(IUnitOfWork unitOfWork)
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
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Category model)
		{
			if (ModelState.IsValid)
			{
				Category category = _unitOfWork.Categories.GetFirstOrDefault(x => x.Name == model.Name);

				if (category is not null)
				{
					ModelState.AddModelError("", "Category already exists!");
					return View(model);
				}

				model.IsListed = true;
				_unitOfWork.Categories.Add(model);
				_unitOfWork.Save();

				return RedirectToAction("Index");
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult Edit(int? id)
		{
			Category category = _unitOfWork.Categories.GetFirstOrDefault(x => x.Id == id)!;

			if (category is not null)
			{
				return View(category);
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Category model)
		{
			if (ModelState.IsValid)
			{
				Category category = _unitOfWork.Categories.GetFirstOrDefault(x => x.Name == model.Name)!;

				if (category is not null)
				{
					ModelState.AddModelError("", "Category already exists!");
					return View(model);
				}

				_unitOfWork.Categories.Update(model);
				_unitOfWork.Save();

				return RedirectToAction("Index");
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult AddOffer(int id)
		{
			Category category = _unitOfWork.Categories.GetFirstOrDefault(x => x.Id == id)!;

			if (category is not null)
			{
				return View(category);
			}

			return NotFound();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult AddOffer(Category model)
		{
			_unitOfWork.Categories.UpdateCategoryDiscount(model.Id, model.Discount);
			_unitOfWork.Save();

			List<Product> products = _unitOfWork.Products.GetAllProductsOfCategory(model).ToList();

			foreach (Product product in products)
			{
				_unitOfWork.Products.UpdateDiscount(product.Id, model.Discount);
				_unitOfWork.Save();
			}

			return RedirectToAction(nameof(Index));
		}

		public IActionResult ToggleListing(int id)
		{
			Category category = _unitOfWork.Categories
				.GetFirstOrDefault(x => x.Id == id);

			if (category is not null)
			{
				_unitOfWork.Categories.UpdateListingStatus(category);
				_unitOfWork.Save();

				_unitOfWork.Products.UpdateListingStatus(category.IsListed, category.Id);
				_unitOfWork.Save();
			}

			return RedirectToAction(nameof(Index));
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll(string status)
		{
			IEnumerable<Category> categories;

			switch(status)
			{
				case "unlisted":
					categories = _unitOfWork.Categories.GetAll().Where(x => !x.IsListed);
					break;
				case "listed":
					categories = _unitOfWork.Categories.GetAll().Where(x => x.IsListed);
					break;
				default:
					categories = _unitOfWork.Categories.GetAll();
					break;
			}

			return Json(new { data = categories});
		}

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			Category category = _unitOfWork.Categories.GetFirstOrDefault(x => x.Id == id)!;

			if (category is not null)
			{
				_unitOfWork.Categories.Delete(category);
				_unitOfWork.Save();

				return Json(new { success = true, message = "Delete successfull" });
			}

			return Json(new { success = false, message = "Error while deleting" });
		}

		#endregion
	}
}