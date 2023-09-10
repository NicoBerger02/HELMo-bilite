using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helmobilite.Models
{
	public class ApplicationUser : IdentityUser
    {
		[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "L'adresse email est obligatoire.")]
		[EmailAddress(ErrorMessage = "Adresse email invalide")]
		public override string Email { get => base.Email; set => base.Email = value; }
		public string? ImageName { get; set; }
	}
}
