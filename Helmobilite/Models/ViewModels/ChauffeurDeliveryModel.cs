using Microsoft.Build.ObjectModelRemoting;
using Microsoft.EntityFrameworkCore;

namespace Helmobilite.Models.ViewModels
{
	public class ChauffeurDeliveryModel
	{
		public int WeekOffset { get; set; }

		public string ClientEmail { get; set; }
		public string ClientAddress { get; set; }
		public string ClientEnterprise { get; set; }

		public int DeliveryId { get; set; }
		public string LoadingPlace { get; set; }
		public string UnloadingPlace { get; set; }
		public string LoadingDateTime{ get; set; }
		public string UnloadingDateTime { get; set; }
		public string Content { get; set; }
		public Status Status { get; set; }
		public Motif Motif { get; set; }
		public string Comment { get; set; }

		public string Truck { get; set; }

		public ChauffeurDeliveryModel(int weekOffset, Delivery delivery) 
		{
			WeekOffset = weekOffset;

			ClientEmail = delivery.Client.Email;
			ClientAddress = delivery.Client.Address.ToString();
			ClientEnterprise = delivery.Client.Enterprise;

			DeliveryId = delivery.Id;
			LoadingPlace = delivery.LoadingPlace;
			UnloadingPlace = delivery.UnloadingPlace;
			LoadingDateTime = delivery.LoadingDateTime.ToString("Le dd MMMM à HH:mm");
			UnloadingDateTime = delivery.UnloadingDateTime.ToString("Le dd MMMM à HH:mm");
			Content = delivery.Content;
			Status = delivery.Status;
			Motif = delivery.Motif ?? 0;
			Comment = delivery.Comment ?? "";

			Truck = delivery.Truck == null ? "Camion non trouvé" : delivery.Truck.DisplayName;
		}
	}
}
