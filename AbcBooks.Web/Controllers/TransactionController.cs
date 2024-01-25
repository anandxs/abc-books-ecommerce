using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbcBooks.Web.Controllers
{
	[Authorize]
	public class TransactionController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		public TransactionController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public IActionResult GetAll()
		{
			IEnumerable<Transaction> transactions;

			if (User.IsInRole(ProjectConstants.ADMIN))
			{
				transactions = _unitOfWork.Transactions.GetAll();	
			}
            else
            {
				var claimsIdentity = (ClaimsIdentity)User.Identity!;
				var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

				Wallet wallet = _unitOfWork.Wallets.GetWalletWithUser(x => x.ApplicationUserId == claim!.Value);
				
				transactions = _unitOfWork.Transactions.GetAllTransactionForUser(wallet.Id);
            }

			return Json(new { data = transactions });
        }
	}
}
