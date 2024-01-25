using AbcBooks.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AbcBooks.Web.Controllers
{
    public class ErrorController : Controller
	{
		private readonly ILogger<ErrorController> _logger;
		private readonly IEmailSender _emailSender;

		public ErrorController(ILogger<ErrorController> logger, IEmailSender emailSender)
		{
			_logger = logger;
			_emailSender = emailSender;
		}

		[Route("Error/{statusCode}")]
		public IActionResult HttpStatusCodeHandler(int statusCode)
		{
			switch(statusCode)
			{
				case 404:
					ViewBag.ErrorMessage = "Sorry! The resource you requested could not be found";
					break;
				default:
					ViewBag.ErrorMessage = "Something went wrong!";
					break;
			}

			return View("NotFound");
		}

		[Route("/Error")]
		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		[AllowAnonymous]
		public IActionResult Error()
		{
			var exceptionDetails = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

			if (exceptionDetails is not null)
			{
				_logger.LogError($"The path {exceptionDetails.Path} threw an exception");
				_logger.LogError($"Exception message : {exceptionDetails.Error.Message}");
				_logger.LogError($"\n\n STACKTRACE\n\n{exceptionDetails.Error.StackTrace} ");

				//string htmlString = $@"<!DOCTYPE html>
				//				<html>
				//				<body>
				//					<h1>Path</h1>
				//					<p>{exceptionDetails.Path}</p>
				//					<h1>Message</h1>
				//					<p>{exceptionDetails.Error.Message}</p>
				//					<h1>Stack Trace</h1>
				//					<p>{exceptionDetails.Error.StackTrace}</p>
				//					<h1>Inner Exception</h1>
				//				</body>
				//				</html>";

				//await _emailSender.SendEmailAsync("anandsatheeshk@gmail.com", "UNHANDLED EXCEPTION", htmlString); 
			}

			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
