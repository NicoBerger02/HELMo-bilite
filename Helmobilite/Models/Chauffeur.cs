using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;

namespace Helmobilite.Models
{
	public class Chauffeur : Member
	{
		public List<ChauffeurLicense>? Licenses { get; set; }

		public List<Delivery>? Deliveries { get; set; }

		[NotMapped]
		public string DisplayName => Name + " " + FirstName;

		[NotMapped]
		public string NameWithMatricule => FirstName + " " + Name + " ("  + Matricule + ")";

		public Chauffeur() { }

		public Chauffeur(string email, string name, string firstname, string matricule, List<License> licenses)
		{
			Email = email;
			Name = name;
			FirstName = firstname;
			Matricule = matricule;
			UpdateLicenses(licenses);
		}

		public void UpdateLicenses(List<License> newLicenses)
		{
			if (Licenses.IsNullOrEmpty())
			{
				Licenses = newLicenses.Select(l => new ChauffeurLicense { License = l }).ToList();
			} else
			{
				var chauffeurLicensesToRemove = Licenses
					.Where(l => !newLicenses.Contains(l.License))
					.ToList();

				RemoveLicenses(chauffeurLicensesToRemove);

				foreach (var license in newLicenses)
				{
					if (!Licenses.Any(l => l.License == license))
					{
						Licenses.Add(new ChauffeurLicense { License = license });
					}
				}
			}
		}

		private void RemoveLicenses(List<ChauffeurLicense> chauffeurLicensesToRemove)
		{
			if (chauffeurLicensesToRemove.Count > 0)
			{
				foreach (var chauffeurLicense in chauffeurLicensesToRemove)
				{
					Licenses.Remove(chauffeurLicense);
				}

				UpdateDeliveriesAfterLicenseDeletion();
			}
		}

		public void UpdateDeliveriesAfterLicenseDeletion()
		{
			if (!Deliveries.IsNullOrEmpty())
			{
				var onGoingDeliveries = Deliveries.Where(d => d.Status == Status.ONGOING).ToList();

				foreach (var delivery in onGoingDeliveries)
				{
					if (delivery.Truck != null && !CanDriveTruck(delivery.Truck))
					{
						Deliveries.Remove(delivery);
						delivery.RemoveAssignment();
					}
				}
			}
		}

		public string GetLicensesToString()
		{
			List<string> licenses = new();
			if (Licenses.IsNullOrEmpty())
			{
				return "aucune license n'a été trouvée";
			}
			else
			{
				return string.Join(", ", Licenses.OrderBy(l => l.License).Select(objet => objet.License.GetEnumDisplayName()));
			}
		}

		public bool HasLicense(License license) => !Licenses.IsNullOrEmpty() && Licenses.Where(l => l.License == license).Any();

		public override string ToString()
		{
			return Matricule + " - " + FirstName + " " + Name;
		}

		public bool IsAvailableForNewDelivery(Delivery newDelivery)
		{
			if (!Deliveries.IsNullOrEmpty())
			{
				return !Deliveries.Where(d => d.CollidesWithAnotherDelivery(newDelivery, 1)).Any();
			}
			return true;
		}

		public bool CanDriveTruck(Truck truck)
		{
			if (Licenses.IsNullOrEmpty())
			{
				return false;
			} else
			{
				return truck.License switch
				{
					License.CE => Licenses.Where(l => l.License == License.CE).Any(),
					License.C => Licenses.Where(l => l.License == License.C || l.License == License.CE).Any(),
					_ => false
				};
            }
		}
    }
}