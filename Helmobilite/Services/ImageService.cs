using Helmobilite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Helmobilite.Services
{
	public class ImageService : IImageService
	{
		private readonly IWebHostEnvironment _webHostEnvironment;

		public ImageService([FromServices] IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;
		}

		public string ReplaceImage(IFormFile newImage, ImageFor imageFor, string oldImageName = "")
		{
			string newImageName;
			if (!oldImageName.IsNullOrEmpty())
			{
				DeleteImage(oldImageName, imageFor);
			}

			newImageName = Guid.NewGuid().ToString() + "_" + newImage.FileName;
			string imagePath = $"{_webHostEnvironment.WebRootPath}/images/uploads/{(imageFor == ImageFor.Truck ? "trucks" : "users")}/{newImageName}";

			try
			{
				using var stream = new FileStream(imagePath, FileMode.Create);
				newImage.CopyTo(stream);
			}
			catch { }

			return newImageName;
		}

		public void DeleteImage(string imageName, ImageFor imageFor)
		{
			string imagePath = $"{_webHostEnvironment.WebRootPath}/images/uploads/{(imageFor == ImageFor.Truck ? "trucks" : "users")}/{imageName}";
			if (File.Exists(imagePath))
			{
				try
				{
					File.Delete(imagePath);
				}
				catch { }
			}
		}
	}
}
