using Microsoft.AspNetCore.Http;

namespace AbcBooks.Utilities.Interfaces
{
	public interface IImageModifier
	{
		bool IsValidImage(IFormFile formFile);
	}
}
