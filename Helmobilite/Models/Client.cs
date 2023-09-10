using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public class Client : ApplicationUser
    {
		[Required(ErrorMessage = "Le nom de l'entreprise ne peut pas être vide.")]
		[DataType(DataType.Text)]
		[MaxLength(50, ErrorMessage = "50 caractères maximum")]
		public string Enterprise { get; set; }
        public bool IsBadPayer { get; set; }
        public Address Address { get; set; }
        public IEnumerable<Delivery>? Deliveries { get; set; }
		public string FullAddress => Address.ToString();

		public Client() { }

		public Client(string enterpise, string email, Address address) 
		{
			Enterprise = enterpise;
			Email = email;
			Address = address;
		}
	}
}
