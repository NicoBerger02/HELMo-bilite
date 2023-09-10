using Helmobilite.Validations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helmobilite.Models
{
    public abstract class Member : ApplicationUser
    {
        [Required(ErrorMessage = "Le nom ne peut pas être vide.")]
        [MaxLength(20, ErrorMessage = "20 caractères maximum")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Le prénom ne peut pas être vide.")]
		[MaxLength(20, ErrorMessage = "20 caractères maximum")]
		public string FirstName { get; set; }

        [Matricule]
        [Required(ErrorMessage = "Le matricule ne peut pas être vide.")]
        [RegularExpression("^[A-Z]\\d{6}$", ErrorMessage = "Le matricule doit être composé d'une lettre majuscule suivie de 6 chiffres.")]
		[MaxLength(7)]
		[MinLength(7)]
		public string Matricule { get; set; }
        public DateTime? BirthDate { get; set; }

        public string DisplayBirthDate => BirthDate == null ? "Inconnue" : BirthDate?.ToString("Le dd MMMM yyyy");

		[NotMapped]
		public string DisplayFirstName => FirstName + " " + Name;
	}
}
