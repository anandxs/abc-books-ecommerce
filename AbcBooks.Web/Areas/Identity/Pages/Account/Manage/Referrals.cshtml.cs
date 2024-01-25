using AbcBooks.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Security.Claims;

namespace AbcBooks.Web.Areas.Identity.Pages.Account.Manage
{
	[Authorize(Roles = ProjectConstants.CUSTOMER)]
    public class ReferralsModel : PageModel
    {
        public class InputModel
        {
            public string ReferralCode { get; set; } = null!;
        }

        public InputModel Input { get; set; }

        public ReferralsModel()
        {
            Input = new InputModel();
        }

        public IActionResult OnGet()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity!;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            Input.ReferralCode = claim!.Value;

            return Page();
        }
    }
}
