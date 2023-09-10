using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Helmobilite.Models
{
    public enum Status
    {
		[Display(Name = "En attente de validation")]
		WAITING,
		[Display(Name = "En cours")]
		ONGOING,
		[Display(Name = "Ratée")]
        FAILED,
		[Display(Name = "Effectuée")]
		DONE
	}

}
