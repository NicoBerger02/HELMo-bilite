namespace Helmobilite.Models
{
    public enum License
    {
        B,
        C,
        CE
    }

    public class ChauffeurLicense
    {
        public string ChauffeurId { get; set; }
        public Chauffeur? Chauffeur { get; set; }
        public License License { get; set; }
    }
}
