using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Models.ViewModels;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace AbcBooks.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private static string[] _sort = null!;

        public HomeController(
                IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _sort = new string[]
            {
                "Price: Low to High",
                "Price: High to Low",
            };
        }

        [HttpGet]
        public IActionResult Index()
        {
            HomeViewModel model = new()
            {
                Products = _unitOfWork.Products
                    .GetAllListedProductsWithCategory()
                    .Take(4).ToList(),
                ProductImages = _unitOfWork.ProductImages
                    .GetAll(),
                Banners = _unitOfWork.Banner.GetAll().ToList()
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Browse(BrowseViewModel postModel)
        {
            BrowseViewModel viewModel = new()
            {
                CategoryList = _unitOfWork.Categories
                    .GetAll()
                    .Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }),
                ProductImages = _unitOfWork.ProductImages.GetAll(),
                SearchString = postModel.SearchString,
                CategoryId = postModel.CategoryId,
                SortOptions = _sort.Select(x => new SelectListItem()
                {
                    Text = x,
                    Value = x
                }),
                SortOption = postModel.SortOption
            };

            if (postModel.CategoryId == 0)
            {
                switch (postModel.SortOption)
                {
                    case "Price: High to Low":
                        viewModel.Products = _unitOfWork.Products
                            .GetAllListedProductsWithCategory()
                            .Where(x =>
                                x.Title.ToUpper()
                                .Contains((postModel.SearchString ?? "").ToUpper()))
                            .OrderByDescending(x =>
                            {
                                if (x.Discount == 0)
                                {
                                    return x.Mrp;
                                }
                                else
                                {
                                    return x.Mrp * (100 - x.Discount) / 100;
                                }
                            });
                        break;

                    default:
                        viewModel.Products = _unitOfWork.Products
                            .GetAllListedProductsWithCategory()
                            .Where(x =>
                                x.Title.ToUpper()
                                .Contains((postModel.SearchString ?? "").ToUpper()))
                            .OrderBy(x =>
                            {
                                if (x.Discount == 0)
                                {
                                    return x.Mrp;
                                }
                                else
                                {
                                    return x.Mrp * (100 - x.Discount) / 100;
                                }
                            });
                        break;
                }
            }
            else
            {
                switch (postModel.SortOption)
                {
                    case "Price: High to Low":
                        viewModel.Products = _unitOfWork.Products
                            .GetAllListedProductsWithCategory()
                            .Where(x =>
                                x.CategoryId == postModel.CategoryId &&
                                x.Title.ToUpper()
                                .Contains((postModel.SearchString ?? "").ToUpper()))
                            .OrderByDescending(x => x.Mrp);
                        break;
                    default:
                        viewModel.Products = _unitOfWork.Products
                            .GetAllListedProductsWithCategory()
                            .Where(x =>
                                x.CategoryId == postModel.CategoryId &&
                                x.Title.ToUpper()
                                .Contains((postModel.SearchString ?? "").ToUpper()))
                            .OrderBy(x => x.Mrp);
                        break;
                }
            }

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Details(int productId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCart model = new()
            {
                ProductId = productId,
                Quantity = 1,
                ProductImages = _unitOfWork.ProductImages.GetAllProductImages(productId).ToList(),
                IsInWishList = claim is not null ? _unitOfWork.Wishlist.ContainsProduct(claim!.Value, productId) : false
            };

            if (User.IsInRole(ProjectConstants.ADMIN))
            {
                model.Product = _unitOfWork.Products
                    .GetProductWithCategory(productId);

                if (model.Product is null)
                {
                    Response.StatusCode = 404;
                    return View("ProductNotFound", productId);
                }

                return View(model);
            }

            model.Product = _unitOfWork.Products
                .GetListedProductWithCategory(productId);

            if (model.Product is null)
            {
                Response.StatusCode = 404;
                return View("ProductNotFound", productId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ProjectConstants.CUSTOMER)]
        public IActionResult Details(ShoppingCart model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            model.ApplicationUserId = claim!.Value;

            var shoppingCart = _unitOfWork.ShoppingCart
                .GetFirstOrDefault(x => x.ApplicationUserId == model.ApplicationUserId && x.ProductId == model.ProductId);

            Product product = _unitOfWork.Products.GetFirstOrDefault(x => x.Id == model.ProductId);

            if (shoppingCart is null)
            {
                if (model.Quantity > product.Stock)
                {
                    TempData["OrderExceedsStock"] = $"Can order atmost {product.Stock} unit(s) only!";
                    return RedirectToAction(nameof(Details), new { productId = product.Id });
                }

                _unitOfWork.ShoppingCart.Add(model);
                _unitOfWork.Save();

                HttpContext.Session.SetInt32(
                    ProjectConstants.SESSION_CART,
                    _unitOfWork.ShoppingCart
                        .GetUserShoppingCart(claim.Value).ToList().Count);
            }
            else
            {
                if (shoppingCart.Quantity >= product.Stock)
                {
                    TempData["OrderExceedsStock"] = $"Can order atmost {product.Stock} unit(s) only!";
                    return RedirectToAction(nameof(Details), new { productId = product.Id });
                }

                _unitOfWork.ShoppingCart.IncrementQuantity(shoppingCart, model.Quantity);
                _unitOfWork.Save();
            }

            return RedirectToAction(nameof(Browse));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ProjectConstants.CUSTOMER)]
        public IActionResult AddToWishList(ShoppingCart model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            var existingWishlist = _unitOfWork.Wishlist
                .GetFirstOrDefault(x => x.ApplicationUserId == claim.Value && x.ProductId == model.ProductId);

            if (existingWishlist is not null)
            {
                return RedirectToAction(nameof(Details), new { productId = model.ProductId });
            }

            Wishlist wishlist = new()
            {
                ApplicationUserId = claim!.Value,
                ProductId = model.ProductId
            };
            _unitOfWork.Wishlist.Add(wishlist);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { productId = model.ProductId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ProjectConstants.CUSTOMER)]
        public IActionResult RemoveFromWishList(ShoppingCart model)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Wishlist wishlist = _unitOfWork.Wishlist.GetFirstOrDefault(x => x.ApplicationUserId == claim!.Value && x.ProductId == model.ProductId);

            _unitOfWork.Wishlist.Remove(wishlist);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Details), new { productId = model.ProductId });
        }

        //[HttpGet]
        //public IActionResult Privacy()
        //{
        //	_emailSender.SendEmailAsync("anand1361s@gmail.com", "TEST", "This is for testing!");
        //	return View();
        //}
    }
}