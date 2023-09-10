using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models.ViewModels
{
	public class ChauffeurAssignmentModel
	{
		public int DeliveryId { get; set; }
		public IList<Chauffeur>? AvailableChauffeurs { get; set; }
		public IList<Truck>? AvailableTrucks { get; set; }

		[Required(ErrorMessage = "Veuillez sélectionner un chauffeur")]
		public string SelectedChauffeurId { get; set; }

		[Required(ErrorMessage = "Veuillez sélectionner un camion")]
		public int SelectedTruckId { get; set; }
	}
}
