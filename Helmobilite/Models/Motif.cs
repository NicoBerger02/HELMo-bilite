using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public enum Motif
    {
		[Display(Name = "Maladie")]
		SICKNESS,
		[Display(Name = "Accident")]
		ACCIDENT,
		[Display(Name = "Client absent / Livraison impossible")]
		CLIENTSIDE
    }
}
