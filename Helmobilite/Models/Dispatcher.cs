using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public class Dispatcher : Member
    {
        [Required(ErrorMessage = "Veuillez sélectionner votre niveau d'étude.")]
        public Degree Degree { get; set; }

        public Dispatcher() { }

        public Dispatcher(string email, string name, string firstName, string matricule, Degree degree)
        {
            Email = email;
            Name = name;
            FirstName = firstName;
            Matricule = matricule;
            Degree = degree;
        }
    }
}
