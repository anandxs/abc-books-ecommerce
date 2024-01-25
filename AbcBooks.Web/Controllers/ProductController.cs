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
	public class ProductController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
		{
			_unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
		}

		[HttpGet]
		public IActionResult Index()
		{
			return View();
		}

		[HttpGet]
		public IActionResult Create()
		{
			ProductViewModel model = new ProductViewModel()
			{
				Product = new(),
				CategoryList = _unitOfWork.Categories
					.GetAll()
					.Select(x => new SelectListItem
					{
						Text = x.Name,
						Value = x.Id.ToString()
					})
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(ProductViewModel model, IFormFileCollection? formFiles = null)
		{
			List<ProductImage> imageList = new List<ProductImage>();

			if (ModelState.IsValid)
			{
				if (formFiles.Count is not 0)
				{
					int len = formFiles.Count <= 4 ? formFiles.Count : 4;

					for (int i = 0; i < len; i++)
					{
						var image = formFiles[i];
						string imageUrl = UploadImage(image);
						imageList.Add(new ProductImage { ImageUrl = imageUrl });
					}
                }
				else
				{
					ModelState.AddModelError("", "Please add an image!");

					model.CategoryList = _unitOfWork.Categories
						.GetAll()
						.Select(x => new SelectListItem
						{
							Text = x.Name,
							Value = x.Id.ToString()
						});

					return View(model);
				}

				_unitOfWork.Products.Add(model.Product);
				_unitOfWork.Save();

				foreach (var image in imageList)
				{
					image.ProductId = model.Product.Id;
				}

				_unitOfWork.ProductImages.AddRange(imageList);
				_unitOfWork.Save();

				return RedirectToAction("Index");
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			ProductViewModel model = new ProductViewModel()
			{
				Product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == id),
				CategoryList = _unitOfWork.Categories
					.GetAll()
					.Select(x => new SelectListItem
					{
						Text = x.Name,
						Value = x.Id.ToString()
					}),
				ProductImages = _unitOfWork.ProductImages.GetAllProductImages(id)
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(ProductViewModel model)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Products.Update(model.Product);
				_unitOfWork.Save();

				return RedirectToAction("Index");
			}

			return View(model);
		}

		public IActionResult AddNewImages(ProductViewModel model, IFormFileCollection formFiles)
		{
			List<ProductImage> imageList = new();

			if (formFiles.Count is not 0)
			{
				int productImageCount = _unitOfWork.ProductImages.GetImageCountForProduct(model.Product.Id);

				int length = Math.Min(4 - productImageCount, formFiles.Count);
				for (int i = 0; i < length; i++)
				{
                    var image = formFiles[i];
                    string imageUrl = UploadImage(image);
                    imageList.Add(new ProductImage 
					{ 
						ImageUrl = imageUrl, 
						ProductId = model.Product.Id 
					});
                }

				_unitOfWork.ProductImages.AddRange(imageList);
				_unitOfWork.Save();
			}
			else
			{
				ModelState.AddModelError(String.Empty, "Please add an image!");
			}

			return RedirectToAction(nameof(Edit), new { Id = model.Product.Id});
		}

		public IActionResult RemoveImage(int imageId)
		{
			ProductImage image = _unitOfWork.ProductImages.GetFirstOrDefault(x => x.Id == imageId);

			if (image is not null)
			{
				string wwwRootPath = _webHostEnvironment.WebRootPath;

				var imagePath = Path.Combine(
					wwwRootPath, 
					image.ImageUrl.TrimStart('\\'));

				if (System.IO.File.Exists(imagePath))
				{
					System.IO.File.Delete(imagePath);
				}

				_unitOfWork.ProductImages.Remove(image);
				_unitOfWork.Save();

				return RedirectToAction(nameof(Edit), new { Id = image.ProductId });
			}

			return NotFound();
		}

		public IActionResult ToggleListing(int id)
		{
			if (id == 0)
			{
				return NotFound();
			}

			Product product = _unitOfWork.Products
				.GetFirstOrDefault(x => x.Id == id);

			product.IsListed = !product.IsListed;

			_unitOfWork.Products.Update(product);
			_unitOfWork.Save();

			return RedirectToAction("Index");
		}

		private string UploadImage(IFormFile file)
		{
			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string fileName = Guid.NewGuid().ToString();
			string uploads = Path.Combine(wwwRootPath, @"images\products");
			string extension = Path.GetExtension(file.FileName);

			using (var fileStreams = new FileStream(
				Path.Combine(uploads, fileName + extension), FileMode.Create))
			{
				file.CopyTo(fileStreams);
			}

			return @"\images\products\" + fileName + extension;
		}

		#region API CALLS

		public IActionResult GetAll(string status)
        {
			IEnumerable<Product> products = _unitOfWork.Products.GetAllProductsWithCategory();

            switch (status)
            {
                case "listed":
					products = products.Where(x => x.IsListed);
                    break;
                case "unlisted":
					products = products.Where(x => x.IsListed == false);
                    break;
                default:
                    break;
            }

            return Json(new { data = products });
        }

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			Product product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == id);

            if (product is null)
            {
				return Json(new { success = false, message = "Something went wrong" });
            }

			var images = _unitOfWork.ProductImages.GetAllProductImages(product.Id).ToList();

			foreach (var image in images)
			{
				if (image is not null)
				{
					string wwwRootPath = _webHostEnvironment.WebRootPath;

					var imagePath = Path.Combine(
						wwwRootPath,
						image.ImageUrl.TrimStart('\\'));

					if (System.IO.File.Exists(imagePath))
					{
						System.IO.File.Delete(imagePath);
					}

					_unitOfWork.ProductImages.Remove(image);
					_unitOfWork.Save();
				}
			}

			_unitOfWork.Products.Delete(product);
			_unitOfWork.Save();

            return Json(new { success = true, message = "Deleted successfully" });
		}

        #endregion
    }
}
