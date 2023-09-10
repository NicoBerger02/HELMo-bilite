using Helmobilite.Validations;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Helmobilite.Models
{
    public class Truck
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Plaque d'immatriculation")]
        [RegularExpression("^[0-9]-[A-Z]{3}-[0-9]{3}$", ErrorMessage = "La plaque d'immatriculation doit être constituée d'un chiffre, trois lettres, trois chiffres, le tout séparé par des tirets")]
        [LicensePlate]
        public string LicensePlate { get; set; }

		[Required]
		[Display(Name = "Marque")]
        [MaxLength(30)]
		public string Brand { get; set; }

        [Required]
		[MaxLength(50)]
		[Display(Name = "Modèle")]
		public string Model { get; set; }

        [NotMapped]
        [Display(Name = "Licence")]
        public License License => Payload < 3500 ? License.C : License.CE;

		[Required]
        [Display(Name = "Photo")]
        public string ImageName { get; set; }

        [Required]
        [Display(Name = "Charge utile (kg)")]
        [Range(0, int.MaxValue, ErrorMessage = "La charge utile doit être positive")]
        public int Payload { get; set; }

		public List<Delivery> Deliveries { get; set; }

        public string DisplayName => Brand + " " + Model + " (" + LicensePlate + ")";

		public bool IsAvailableForNewDelivery(Delivery newDelivery)
		{
			if (!Deliveries.IsNullOrEmpty())
			{
				return !Deliveries.Where(d => d.CollidesWithAnotherDelivery(newDelivery)).Any();
			}
			return true;
		}

        public void RemoveOnGoingDeliveries()
        {
			if (Deliveries != null)
			{
				foreach (var delivery in Deliveries.Where(d => d.Status == Status.ONGOING))
				{
					delivery.RemoveAssignment();
				}
				Deliveries.Clear();
			}
		}

		public void UpdateInfos(string licensePlate, string brand, string model, int payload)
		{
			LicensePlate = licensePlate;
			Brand = brand;
			Model = model;
			if (payload != Payload)
			{
				Payload = payload;
				foreach (var delivery in Deliveries.Where(d => d.Status == Status.ONGOING))
				{
					delivery.UpdateOnTruckLicenseChange(this);
				}
			}
		}
	}
}
