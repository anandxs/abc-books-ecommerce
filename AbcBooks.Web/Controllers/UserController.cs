using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Models.ViewModels;
using AbcBooks.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AbcBooks.Web.Controllers;

[Authorize(Roles = ProjectConstants.ADMIN)]
public class UserController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<IdentityUser> _userManager;

    public UserController(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        UserManager<IdentityUser> userManager)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Index(UserViewModel userViewModel)
    {
        userViewModel.Users = _unitOfWork.ApplicationUsers.GetAll();

        if (userViewModel.SearchString is not null)
        {
            userViewModel.Users = userViewModel.Users.Where(x => x.Email.Contains(userViewModel.SearchString));
        }

        return View(userViewModel);
    }

    [HttpGet]
    public IActionResult Details(string id)
    {
        ApplicationUser appUser = _unitOfWork.ApplicationUsers
            .GetFirstOrDefault(x => x.Id == id);

        ApplicationUserViewModel model = new();

        _mapper.Map(appUser, model);

        return View(model);
    }

    public IActionResult ToggleBlock(string id)
    {
        ApplicationUser model = _unitOfWork.ApplicationUsers
            .GetFirstOrDefault(x => x.Id == id);

        model.IsBlocked = !model.IsBlocked;

        if (model.IsBlocked)
        {
            _userManager.UpdateSecurityStampAsync(model);
        }

        _unitOfWork.ApplicationUsers.Update(model);
        _unitOfWork.Save();

        return RedirectToAction("Index");
    }
}
