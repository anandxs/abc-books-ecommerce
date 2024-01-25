using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace AbcBooks.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailSender _emailSender;
    private readonly IUserStore<IdentityUser> _userStore;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        IUnitOfWork unitOfWork,
        IEmailSender emailSender,
        IUserStore<IdentityUser> userStore,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _signInManager = signInManager;
        _unitOfWork = unitOfWork;
        _emailSender = emailSender;
        _userStore = userStore;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public IActionResult LoginWithEmailOtp()
    {
        EmailOtp model = new();

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetEmailOtp(EmailOtp model)
    {
        var otp = GenerateOtp();
        model.Otp = otp.ToString();
        model.CreatedTime = DateTime.Now;
        _unitOfWork.EmailOtps.Add(model);
        _unitOfWork.Save();

        string subject = "One Time Password";
        string message = $"OTP for login is {model.Otp}. Do not share with anyone!";

        await _emailSender.SendEmailAsync(model.Email, subject, message);

        EmailOtp newModel = new()
        {
            Email = model.Email
        };

        return View("EnterEmailOtp", newModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ValidateOtp(EmailOtp model)
    {
        if (ModelState.IsValid)
        {
            EmailOtp modelFromDb = _unitOfWork.EmailOtps.GetFirstOrDefault(x => x.Email == model.Email);

            TimeSpan timeDifference = DateTime.Now - modelFromDb.CreatedTime;
            TimeSpan expirationTime = TimeSpan.FromMinutes(2);

            if (timeDifference > expirationTime)
            {
                TempData["OtpExpired"] = "OTP expired. Try again!";

                _unitOfWork.EmailOtps.Remove(modelFromDb);
                _unitOfWork.Save();

                return RedirectToAction("LoginWithEmailOtp");
            }

            ApplicationUser user = _unitOfWork.ApplicationUsers.GetFirstOrDefault(x => x.Email == model.Email);

            if (user == null)
            {
                var newUser = CreateUser();
                await _userStore.SetUserNameAsync(newUser, model.Email, CancellationToken.None);
                await _userManager.CreateAsync(newUser);

                var userId = await _userManager.GetUserIdAsync(newUser);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                await _userManager.ConfirmEmailAsync(newUser, code);

                var temp = _unitOfWork.ApplicationUsers.GetFirstOrDefault(x => x.Id == userId);
                temp.Email = model.Email;
                temp.NormalizedEmail = model.Email.ToUpper();
                _unitOfWork.ApplicationUsers.Update(temp);
                _unitOfWork.Save();

                Wallet wallet = new()
                {
                    ApplicationUserId = userId,
                    Balance = 0
                };
                _unitOfWork.Wallets.Add(wallet);
                _unitOfWork.Save();

                await _userManager.AddToRoleAsync(newUser, ProjectConstants.CUSTOMER);

                await _signInManager.SignInAsync(newUser, isPersistent: false);
            }
            else
            {
                if (model.Otp != modelFromDb.Otp)
                {
                    ModelState.AddModelError(String.Empty, "Incorrect OTP.");

                    return View("EnterEmailOtp", model);
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
            }

            _unitOfWork.EmailOtps.Remove(modelFromDb);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Home");
        }

        IEnumerable<EmailOtp> entities = _unitOfWork.EmailOtps.GetAll().Where(x => (DateTime.Now - x.CreatedTime) > TimeSpan.FromMinutes(2));
        _unitOfWork.EmailOtps.RemoveRange(entities);
        _unitOfWork.Save();

        return View("EnterEmailOtp", model);
    }

    private ApplicationUser CreateUser()
    {
        try
        {
            return Activator.CreateInstance<ApplicationUser>();
        }
        catch
        {
            throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
                $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        }
    }

    private int GenerateOtp()
    {
        int min = 100000;
        int max = 999999;
        Random random = new Random();
        return random.Next(min, max);
    }
}
