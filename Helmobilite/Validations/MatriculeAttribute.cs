using Helmobilite.Models;
using Helmobilite.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using static Helmobilite.Areas.Identity.Pages.Account.RegisterModel;

namespace Helmobilite.Validations
{
	public class MatriculeAttribute : ValidationAttribute
	{
		protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
		{
			var dbContext = validationContext.GetService(typeof(HelmobiliteDbContext)) as HelmobiliteDbContext;
			string? memberId = GetMemberId(validationContext);

			if (memberId == null)
			{
				if (dbContext.Members.Any(m => m.Matricule == (value as string)))
				{
					return new ValidationResult("Le matricule est déjà utilisé");
				}
			}
			else
			{
				if (dbContext.Members.Any(m => m.Matricule == (value as string) && m.Id != memberId))
				{
					return new ValidationResult("Le matricule est déjà utilisé");
				}
			}

            return ValidationResult.Success;
        }

		private static string? GetMemberId(ValidationContext validationContext)
		{
			if (validationContext.ObjectInstance is Member member)
			{
				return member.Id;
			}

			return null;
		}
	}
}
