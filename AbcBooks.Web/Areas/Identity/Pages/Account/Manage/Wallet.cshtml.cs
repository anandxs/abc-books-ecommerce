using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AbcBooks.Web.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = ProjectConstants.CUSTOMER)]
    public class WalletModel : PageModel
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly UserManager<IdentityUser> _userManager;

		public Wallet Wallet { get; set; } = null!;

		public WalletModel(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager)
		{
			_unitOfWork = unitOfWork;
			_userManager = userManager;

		}

		public IActionResult OnGet()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			if (claim == null)
			{
				return RedirectToAction("Error", "Error");
				//return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
			}

			Wallet = _unitOfWork.Wallets.GetWalletWithUser(x => x.ApplicationUserId == claim!.Value);

			if (Wallet == null)
			{
				return RedirectToAction("Error", "Error");
                //return NotFound($"Unable to load user wallet with ID '{_userManager.GetUserId(User)}'.");
			}

			return Page();
		}
	}
}
