using Helmobilite.Models.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Validations
{
    public class TruckPhotoRequiredAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var model = (TruckViewModel) validationContext.ObjectInstance;
            if (model.PhotoIsRequired && model.ImageUploaded == null)
            {
                return new ValidationResult("Veuillez fournir la photo du camion");
            }
            return ValidationResult.Success;
        }
    }
}
