using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbcBooks.Web.Controllers
{
	[Authorize]
	public class WishlistController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public WishlistController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public IActionResult AddToCart(int productId)
		{
			return RedirectToAction("Details", "Home", new ShoppingCart { ProductId = productId });
		}

		public IActionResult RemoveFromWishlist(int productId)
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			Wishlist wishlist = _unitOfWork.Wishlist.GetFirstOrDefault(x => x.ApplicationUserId == claim!.Value && x.ProductId == productId);

			_unitOfWork.Wishlist.Remove(wishlist);
			_unitOfWork.Save();

			return LocalRedirect("/Identity/Account/Manage/Wishlist");
		}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			IEnumerable<Wishlist> list = _unitOfWork.Wishlist.GetUserWishlist(claim!.Value);

			return Json(new { data = list });
		}

		#endregion
	}
}
