namespace Helmobilite.Models.ViewModels
{
	public class HomePageViewModel
	{
		public List<ImageGroup> EnterpriseGroups { get; set; }

		public string DispatcherName { get; set; }
		public string DispatcherPhoto { get; set; }

		public string ChauffeurName { get; set; }
		public string ChauffeurPhoto { get; set; }

		public List<ImageGroup> TruckGroups { get; set; }

		public HomePageViewModel(List<Client> clients, Dispatcher? dispatcher, Chauffeur? chauffeur, List<Truck> trucks)
		{
			EnterpriseGroups = 
				clients.Select(c => new ImageViewModel { LogoPath = c.ImageName, Description = c.Enterprise })
				.Chunk(3)
				.Select((imageViewModels, i) => new ImageGroup { Index = i, ImageViewModels = imageViewModels.ToList() })
				.ToList();

			DispatcherName = dispatcher == null ? "Patrick Ledent" : dispatcher.DisplayFirstName;
			DispatcherPhoto = dispatcher == null ? "default_dispatcher_photo.jpg" : dispatcher.ImageName;

			ChauffeurName = chauffeur == null ? "Martine Duprets" : chauffeur.DisplayFirstName;
			ChauffeurPhoto = chauffeur == null ? "default_chauffeur_photo.jpg" : chauffeur.ImageName;

			TruckGroups =
				trucks.Select(t => new ImageViewModel { LogoPath = t.ImageName, Description = t.Brand + " " + t.Model })
				.Chunk(4)
				.Select((imageViewModels, i) => new ImageGroup { Index = i, ImageViewModels = imageViewModels.ToList() })
				.ToList();
		}
	}

	public class ImageGroup
	{
		public int Index { get; set; }
		public List<ImageViewModel> ImageViewModels { get; set; }
	}

	public class ImageViewModel
	{
		public string LogoPath { get; set; }
		public string Description { get; set; }
	}
}
