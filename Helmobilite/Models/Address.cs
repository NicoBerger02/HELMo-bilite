using Humanizer;
using System.ComponentModel.DataAnnotations;

namespace Helmobilite.Models
{
    public class Address
    {
        public int Id { get; set; }

		[Required(ErrorMessage = "La rue ne peut pas être vide.")]
		[DataType(DataType.Text)]
		[MaxLength(100, ErrorMessage = "100 caractères maximum")]
		public string StreetAddress { get; set; }

		[Required(ErrorMessage = "La numéro ne peut pas être vide.")]
		[DataType(DataType.Text)]
		[MaxLength(10, ErrorMessage = "10 caractères maximum")]
		public string NumberAddress { get; set; }

		[Required(ErrorMessage = "Le code postal ne peut pas être vide.")]
		[DataType(DataType.PostalCode, ErrorMessage = "Veuillez spécifier un code postal correct.")]
		public int PostCodeAddress { get; set; }

		[Required(ErrorMessage = "La localité ne peut pas être vide.")]
		[MaxLength(20, ErrorMessage = "20 caractères maximum")]
		[DataType(DataType.Text)]
		public string LocalityAddress { get; set; }

		[Required(ErrorMessage = "Le pays ne peut pas être vide.")]
		[DataType(DataType.Text)]
		[MaxLength(20, ErrorMessage = "20 caractères maximum")]
		public string CountryAddress { get; set; }

		public string ClientId { get; set; }

		public Client? Client { get; set; }

		public Address() { }

		public Address(string streetAddress, string numberAddress, int postCodeAddress, string localityAddress, string countryAddress)
		{
			StreetAddress = streetAddress;
			NumberAddress = numberAddress;
			PostCodeAddress = postCodeAddress;
			LocalityAddress = localityAddress;
			CountryAddress = countryAddress;
		}

		public override string ToString()
		{
			return StreetAddress + " " + NumberAddress + ", " + PostCodeAddress + " " + LocalityAddress;
		}
	}
}
