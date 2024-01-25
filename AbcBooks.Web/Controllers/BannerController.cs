using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using AbcBooks.Utilities;
using AbcBooks.Utilities.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AbcBooks.Web.Controllers;

[Authorize(Roles = ProjectConstants.ADMIN)]
public class BannerController : Controller
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IImageModifier _imageModifier;

    public BannerController(IUnitOfWork unitOfWork, IWebHostEnvironment webHost, IImageModifier imageModifier)
    {
        _unitOfWork = unitOfWork;
        _webHostEnvironment = webHost;
        _imageModifier = imageModifier;
    }

    [HttpGet]
    public IActionResult Index()
    {
        int bannerCount = _unitOfWork.Banner.GetAll().Count();
        ViewBag.BannerCount = bannerCount;

        var model = _unitOfWork.Banner.GetAll();
        return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
        Banner model = new();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Banner model, IFormFile? formFile)
    {
        if (ModelState.IsValid)
        {
            if (formFile is null)
            {
                ModelState.AddModelError(string.Empty, "Add an image");
                return View(model);
            }
            else
            {
                if (!_imageModifier.IsValidImage(formFile))
                {
                    ModelState.AddModelError(String.Empty, "You can add only jpg, jpeg or png files.");
                    return View(model);
                }

                model.ImageUrl = UploadImage(formFile);
            }

            _unitOfWork.Banner.Add(model);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var model = _unitOfWork.Banner.GetFirstOrDefault(x => x.Id == id);

        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }

    [HttpPost]
    public IActionResult Edit(Banner model, IFormFile? formFile)
    {
        if (ModelState.IsValid)
        {
            var banner = _unitOfWork.Banner.GetFirstOrDefault(x => x.Id == model.Id);

            if (banner is null)
                return NotFound();

            if (formFile is not null)
            {
                if (!_imageModifier.IsValidImage(formFile))
                {
                    ModelState.AddModelError(String.Empty, "You can add only jpg, jpeg or png files.");
                    return View(model);
                }

                DeleteImage(banner);
                banner.ImageUrl = UploadImage(formFile);
            }

            banner.Href = model.Href;
            _unitOfWork.Save();

            return RedirectToAction(nameof(Edit), new { id = model.Id });
        }

        return View(model);
    }

    [HttpDelete]
    public JsonResult Delete(int id)
    {
        var model = _unitOfWork.Banner.GetFirstOrDefault(x => x.Id == id);

        if (model is null)
            return Json(new { succes = false, message = "could not find banner" });

        DeleteImage(model);
        _unitOfWork.Banner.Remove(model);
        _unitOfWork.Save();

        return Json(new { succes = true, message = "banner deleted successfully" });
    }

    private void DeleteImage(Banner banner)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;

        var imagePath = Path.Combine(
            wwwRootPath,
            banner.ImageUrl.TrimStart('\\'));

        if (System.IO.File.Exists(imagePath))
        {
            System.IO.File.Delete(imagePath);
        }
    }

    private string UploadImage(IFormFile file)
    {
        string wwwRootPath = _webHostEnvironment.WebRootPath;
        string fileName = Guid.NewGuid().ToString();
        string uploads = Path.Combine(wwwRootPath, @"images\banner");
        string extension = Path.GetExtension(file.FileName);

        using (var fileStreams = new FileStream(
            Path.Combine(uploads, fileName + extension), FileMode.Create))
        {
            file.CopyTo(fileStreams);
        }

        return @"\images\banner\" + fileName + extension;
    }
}
