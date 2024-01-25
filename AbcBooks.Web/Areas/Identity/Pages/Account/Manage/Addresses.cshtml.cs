using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AbcBooks.Web.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = ProjectConstants.CUSTOMER)]
    public class AddressesModel : PageModel
    {
		private readonly IUnitOfWork _unitOfWork;

		public AddressesModel(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [BindProperty]
        public IEnumerable<Address> Address { get; set; } = null!;

        public void OnGet()
        {
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

			Address = _unitOfWork.Addresses.GetAllAddressesByUser(claim.Value);
        }
    }
}
