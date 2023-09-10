using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public enum Degree
    {
        CESS,
        [Display(Name = "Bachelier")]
        BACHELOR,
		[Display(Name = "Licencier")]
		REDUNDANT
    }
}
