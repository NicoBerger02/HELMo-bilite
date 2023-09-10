namespace Helmobilite.Models.ViewModels
{
	public class ChauffeurLicenseViewModel
	{
		public string ChauffeurId { get; set; }
		public List<ChauffeurLicenseCheckBox> SelectedCheckboxes { get; set; }

		public List<License> SelectedLicenses => SelectedCheckboxes.Where(l => l.IsChecked).Select(l => (License)Enum.Parse(typeof(License), l.Text)).ToList();

		public ChauffeurLicenseViewModel() { }

		public ChauffeurLicenseViewModel(Chauffeur chauffeur)
		{
			ChauffeurId = chauffeur.Id;

			var licenses = Enum.GetValues(typeof(License)).Cast<License>().ToList();
			SelectedCheckboxes = licenses.Select(l => new ChauffeurLicenseCheckBox { Text = l.GetEnumDisplayName(), IsChecked = chauffeur.HasLicense(l) }).ToList();
		}
	}

	public class ChauffeurLicenseCheckBox
	{
		public string Text { get; set; }
		public bool IsChecked { get; set; }
	}
}
