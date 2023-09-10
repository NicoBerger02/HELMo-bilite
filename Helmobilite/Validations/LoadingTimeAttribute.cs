using Helmobilite.Models;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Validations
{
    public class LoadingTimeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var delivery = (Delivery)validationContext.ObjectInstance;

            var currentDate = DateTime.Now;

            if (delivery.LoadingDateTime < currentDate)
            {
                return new ValidationResult(ErrorMessage = "La date de chargement doit être plus grande que la date d'aujourd'hui");
            }

            if (delivery.LoadingDateTime >= delivery.UnloadingDateTime)
            {
                return new ValidationResult(ErrorMessage = "La date de déchargement doit être postérieure à la date de chargement.");
            }

            return ValidationResult.Success;
        }
    }
}
