using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models.ViewModels
{
	public class ChauffeurSucceededDeliveryModel
	{
		public int DeliveryId { get; set; }

		public int WeekOffset { get; set; }
		
		[Required]
		[MaxLength(500)]
		public string Comment { get; set; }
	}
}
