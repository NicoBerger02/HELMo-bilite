using System.Drawing;

namespace Helmobilite.Services
{
	public interface IImageService
	{
		public string ReplaceImage(IFormFile newImage, ImageFor imageFor, string oldImageName = "");

		public void DeleteImage(string imageName, ImageFor imageFor);
	}

	public enum ImageFor
	{
		Truck,
		User
	}
}
