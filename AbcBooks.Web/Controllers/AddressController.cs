using AbcBooks.DataAccess.Repository.IRepository;
using AbcBooks.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AbcBooks.Web.Controllers
{
	[Authorize]
	public class AddressController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;

		public AddressController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		[HttpGet]
		public IActionResult Create()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity!;
			var claim = claimsIdentity!.FindFirst(ClaimTypes.NameIdentifier);

			Address model = new()
			{
				ApplicationUserId = claim!.Value
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(Address model)
		{
			if (ModelState.IsValid)
			{
				var addressFromDb = _unitOfWork.Addresses.GetFirstOrDefault(x => x.ApplicationUserId == model.ApplicationUserId && x.IsDefault);

				if (addressFromDb == null)
				{
					model.IsDefault = true;
					_unitOfWork.Addresses.Add(model);
					_unitOfWork.Save();
				}
				else
				{
					if (model.IsDefault)
					{
						addressFromDb.IsDefault = false;
						_unitOfWork.Save();
					}

					_unitOfWork.Addresses.Add(model);
					_unitOfWork.Save();
				}

				return LocalRedirect("/Identity/Account/Manage/Addresses");
			}

			return View(model);
		}

		[HttpGet]
		public IActionResult Edit(int id)
		{
			Address model = _unitOfWork.Addresses.GetFirstOrDefault(x => x.Id == id);

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Edit(Address model)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Addresses.Update(model);
				_unitOfWork.Save();

				return LocalRedirect("/Identity/Account/Manage/Addresses");
			}

			return View(model);
		}

		public IActionResult MakeDefault(int id)
		{
			_unitOfWork.Addresses.MakeAddressDefault(id);
			_unitOfWork.Save();

			return LocalRedirect("/Identity/Account/Manage/Addresses");
		}

		#region API CALLS

		[HttpDelete]
		public IActionResult Delete(int id)
		{
			Address model = _unitOfWork.Addresses.GetFirstOrDefault(x => x.Id == id);

			if (model is not null)
			{
				bool isDefault = model.IsDefault;
				string applicationUserId = model.ApplicationUserId;

				_unitOfWork.Addresses.Delete(model);
				_unitOfWork.Save();

				if (isDefault)
				{
					var newDefault = _unitOfWork.Addresses.GetFirstOrDefault(x => x.ApplicationUserId == applicationUserId);

					if (newDefault is not null)
					{
						newDefault.IsDefault = true;
						_unitOfWork.Save();
					}
				}
				
				return Json(new { success = true, message = "Success" });
			}

			return Json(new { success = false, message = "Failed" });
		}

		#endregion
	}
}
