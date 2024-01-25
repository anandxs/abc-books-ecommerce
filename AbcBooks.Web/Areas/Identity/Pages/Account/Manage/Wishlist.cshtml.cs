using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AbcBooks.Web.Areas.Identity.Pages.Account.Manage
{
    [Authorize(Roles = ProjectConstants.CUSTOMER)]
	public class WishlistModel : PageModel
	{
        public void OnGet()
		{

		}
	}
}
