using Helmobilite.Validations;
using Humanizer;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Helmobilite.Models
{
    [LoadingTime]
	public class Delivery
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le lieu de chargement est obligatoire")]
        [DisplayName("Lieu de chargement")]
        [MaxLength(100)]
        public string LoadingPlace { get; set; }

        [Required(ErrorMessage = "Le lieu de déchargement est obligatoire")]
        [DisplayName("Lieu de déchargement")]
		[MaxLength(100)]
		public string UnloadingPlace { get; set; }

        [Required(ErrorMessage = "La date de chargement est obligatoire")]
        [DisplayName("Date de chargement")]
		public DateTime LoadingDateTime { get; set; }

        [Required(ErrorMessage = "La date de déchargement est obligatoire")]
        [DisplayName("Date de déchargement")]
        public DateTime UnloadingDateTime { get; set; }

        [DisplayName("Commentaire")]
		[MaxLength(500)]
		public string? Comment { get; set; }

        [Required(ErrorMessage = "Le contenu est obligatoire")]
		[MaxLength(150)]
		[DisplayName("Contenu")]
        public string Content { get; set; }

        [DisplayName("Statut")]
        public Status Status { get; set; }

		[DisplayName("Motif")]
        public Motif? Motif { get; set; }

        public string? ChauffeurId { get; set; }
        public Chauffeur? Chauffeur { get; set; }

		public int? TruckId { get; set; }
		public Truck? Truck { get; set; }

        [Required]
        public string ClientId { get; set; }

        public Client? Client { get; set; }

		public Delivery()
		{
		}

        public void AddAssignment(Chauffeur chauffeur, Truck truck)
        {
            Chauffeur = chauffeur;
            Truck = truck;
            Status = Status.ONGOING;
        }

		public bool CollidesWithAnotherDelivery(Delivery delivery, int hoursOffset = 0)
			=> LoadingDateTime <= delivery.UnloadingDateTime.AddHours(hoursOffset) && delivery.LoadingDateTime <= UnloadingDateTime.AddHours(hoursOffset);

		public void HasSucceeded(string comment)
		{
            Comment = comment;
            Status = Status.DONE;
		}

		public void HasFailed(Motif motif)
		{
            Motif = motif;
            Status = Status.FAILED;
		}

		public void RemoveAssignment()
		{
			Chauffeur = null;
			Truck = null;
			Status = Status.WAITING;
		}

		public void UpdateOnTruckLicenseChange(Truck truck)
		{
			if (Chauffeur != null && !Chauffeur.CanDriveTruck(truck))
			{
				RemoveAssignment();
			}
		}
	}
}
