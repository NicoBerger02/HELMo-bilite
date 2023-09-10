using System.Linq;

namespace Helmobilite.Models.ViewModels
{
	public class DeliveryViewModel
	{

		public readonly DateTime LoadingDateTime;

		public readonly string ClientEnterprise;
		public string Date => LoadingDateTime.ToString("Le dd MMMM yyyy à HH:mm");
		public readonly Status Status;
		public readonly string Chauffeur;

		public DeliveryViewModel(Delivery delivery)
		{
			ClientEnterprise = delivery.Client.Enterprise;
			LoadingDateTime = delivery.LoadingDateTime;
			Status = delivery.Status;
			Chauffeur = delivery.Chauffeur == null ? "inconnu" : delivery.Chauffeur.DisplayName;
		}
	}
}
