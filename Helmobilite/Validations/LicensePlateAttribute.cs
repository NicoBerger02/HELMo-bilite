using Helmobilite.Models;
using Helmobilite.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Helmobilite.Validations
{
    public class LicensePlateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var dbContext = validationContext.GetService(typeof(HelmobiliteDbContext)) as HelmobiliteDbContext;
            int truckId = GetTruckId(validationContext);

            if (truckId == -1)
            {
                if (dbContext.Trucks.Any(t => t.LicensePlate == (value as string)))
                {
                    return new ValidationResult("La plaque d'immatriculation est déjà utilisée");
                }
            } else
            {
                if (dbContext.Trucks.Any(t => t.LicensePlate == (value as string) && t.Id != truckId))
                {
                    return new ValidationResult("La plaque d'immatriculation est déjà utilisée");
                }
            }

            return ValidationResult.Success;
        }

        private static int GetTruckId(ValidationContext validationContext)
        {
            if (validationContext.ObjectInstance is Truck truck)
            {
                return truck.Id;
            }
            if (validationContext.ObjectInstance is TruckViewModel viewModel)
            {
                return viewModel.TruckId == 0 ? -1 : viewModel.TruckId;
            }

            return -1;
        }


    }
}
