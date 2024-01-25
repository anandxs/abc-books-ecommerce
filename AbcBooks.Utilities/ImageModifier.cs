using AbcBooks.Utilities.Interfaces;
using Microsoft.AspNetCore.Http;

namespace AbcBooks.Utilities
{
	public class ImageModifier : IImageModifier
	{
		public bool IsValidImage(IFormFile formFile)
		{
			string extension = Path.GetExtension(formFile.FileName);
			string[] fileExtensions = { ".jpg", ".jpeg", ".png" };
			return fileExtensions.Contains(extension);
		}
	}
}
