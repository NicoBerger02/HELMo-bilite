using Helmobilite.Validations;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Helmobilite.Models.ViewModels
{
    public class TruckViewModel
    {
        public int TruckId { get; set; }

        public List<string> Brands => new()
        {
            "Volvo",
            "Scania",
            "DAF",
            "Iveco",
            "Renault",
            "MAN",
            "Khrone",
            "Fiat",
            "Nissan",
            "Fruehauf",
            "Schmitz Cargobull"
        };

        [Required(ErrorMessage = "Veuillez fournir la plaque d'immatriculation")]
        [Display(Name = "Plaque d'immatriculation")]
        [LicensePlate]
        [RegularExpression("^[0-9]-[A-Z]{3}-[0-9]{3}$", ErrorMessage = "La plaque d'immatriculation doit être constituée d'un chiffre, trois lettres, trois chiffres, le tout séparé par des tirets")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Veuillez fournir la marque")]
        [Display(Name = "Marque")]
		[MaxLength(30)]
		public string Brand { get; set; }

        [Required(ErrorMessage = "Veuillez fournir le modèle")]
        [Display(Name = "Modèle")]
		[MaxLength(50)]
		public string Model { get; set; }

        [Required(ErrorMessage = "Veuillez fournir la charge utile")]
        [Display(Name = "Charge utile (kg)")]
		[Range(0, int.MaxValue, ErrorMessage = "La charge utile doit être positive")]
		public int Payload { get; set; }

        [TruckPhotoRequired]
        [Display(Name = "Photo du camion")]
        public IFormFile? ImageUploaded { get; set; }

        public bool PhotoIsRequired { get; set; }

        public TruckViewModel() { }

        public TruckViewModel(Truck truck)
        {
            TruckId = truck.Id;
            Brand = truck.Brand;
            Model = truck.Model;
            LicensePlate = truck.LicensePlate;
            Payload = truck.Payload;
		}
    }
}
