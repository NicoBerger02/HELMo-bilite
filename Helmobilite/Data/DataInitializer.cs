using Helmobilite.Models;
using Microsoft.AspNetCore.Identity;

namespace Helmobilite.Data
{
	public static class DataInitializer
	{
		public static async Task SeedData(UserManager<ApplicationUser> manager, RoleManager<IdentityRole> roleManager, HelmobiliteDbContext dbContext)
		{
			if (!await roleManager.RoleExistsAsync(nameof(Role.Administrateur)))
			{
				await AddRoles(roleManager);
				await SeedUsers(manager);
				await SeedTrucks(dbContext);
			}
		}

		private static async Task SeedTrucks(HelmobiliteDbContext dbContext)
		{
			var truck1 = new Truck
			{
				Brand = "Mercede-Benz",
				Model = "Actros 1845 LS",
				LicensePlate = "1-DOL-777",
				Payload = 40000,
				ImageName = "mercedes_benz_actros_1845_LS.jpg"
			};
			dbContext.Add(truck1);

			var truck2 = new Truck
			{
				Brand = "Iveco",
				Model = "PowerStar 420 E5",
				LicensePlate = "1-FRA-432",
				Payload = 45000,
				ImageName = "iveco_powerstar_420_e5.jpg"
			};
			dbContext.Add(truck2);

			var truck3 = new Truck
			{
				Brand = "DAF",
				Model = "XF EURO 6",
				LicensePlate = "1-BEL-657",
				Payload = 44000,
				ImageName = "daf_xf_euro_6.jpg"
			};
			dbContext.Add(truck3);

			var truck4 = new Truck
			{
				Brand = "Iveco",
				Model = "Eurocargo 180E28",
				LicensePlate = "1-GER-239",
				Payload = 13000,
				ImageName = "iveco_eurocargo_180e28.jpg"
			};
			dbContext.Add(truck4);

			var truck5 = new Truck
			{
				Brand = "Renault",
				Model = "Premium 460 DXI",
				LicensePlate = "1-HON-356",
				Payload = 19000,
				ImageName = "renault_premium_460_dxi.jpg"
			};
			dbContext.Add(truck5);

			var truck6 = new Truck
			{
				Brand = "Volvo",
				Model = "FH 12460",
				LicensePlate = "1-NEE-126",
				Payload = 19000,
				ImageName = "volvo_fh_12460.jpg"
			};
			dbContext.Add(truck6);

			var truck7 = new Truck
			{
				Brand = "Mercedes-Benz",
				Model = "Sprinter",
				LicensePlate = "1-DEN-761",
				Payload = 3000,
				ImageName = "mercedes_benz_sprinter.jpg"
			};
			dbContext.Add(truck7);

			var truck8 = new Truck
			{
				Brand = "Ford",
				Model = "Transit",
				LicensePlate = "1-AUS-545",
				Payload = 2400,
				ImageName = "ford_transit.jpg"
			};
			dbContext.Add(truck8);

			var truck9 = new Truck
			{
				Brand = "MAN",
				Model = "TGE 3.140",
				LicensePlate = "1-CZH-349",
				Payload = 3300,
				ImageName = "man_tge_3140.png"
			};
			dbContext.Add(truck9);

			var truck10 = new Truck
			{
				Brand = "Nissan",
				Model = "Atleon 45.13",
				LicensePlate = "1-POL-437",
				Payload = 2700,
				ImageName = "nissan_atleon_4513.jpg"
			};
			dbContext.Add(truck10);

			await dbContext.SaveChangesAsync();
		}


		private static async Task AddRoles(RoleManager<IdentityRole> manager)
		{

			await manager.CreateAsync(new IdentityRole() { Name = nameof(Role.Administrateur) });

			await manager.CreateAsync(new IdentityRole() { Name = nameof(Role.Client) });

			await manager.CreateAsync(new IdentityRole() { Name = nameof(Role.Dispatcher) });

			await manager.CreateAsync(new IdentityRole() { Name = nameof(Role.Chauffeur) });
		}

		private static async Task SeedUsers(UserManager<ApplicationUser> manager)
		{
			/* Admin */

			ApplicationUser admin = new Administrator()
			{
				Email = "admin@helmo-bilite.be",
				UserName = "admin@helmo-bilite.be",
				Name = "Berger",
				FirstName = "Nicolas",
				BirthDate = new DateTime(2002, 9, 7),
				Matricule = "A000001",
				ImageName = "admin.jpg"
			};
			var resultAdmin = await manager.CreateAsync(admin, "Test123.");
			if (resultAdmin.Succeeded)
			{
				await manager.AddToRoleAsync(admin, nameof(Role.Administrateur));
			}


			/* Dispatchers */

			ApplicationUser dispatcher1 = new Dispatcher()
			{
				Email = "j.hill@helmo-bilite.be",
				UserName = "j.hill@helmo-bilite.be",
				Matricule = "D000001",
				Name = "Hill",
				FirstName = "Julie",
				BirthDate = new DateTime(1990, 2, 3),
				Degree = Degree.BACHELOR,
				ImageName = "julie_hill.jpg"
			};
			var resultDispatcher1 = await manager.CreateAsync(dispatcher1, "Test123.");
			if (resultDispatcher1.Succeeded)
			{
				await manager.AddToRoleAsync(dispatcher1, nameof(Role.Dispatcher));
			}

			ApplicationUser dispatcher2 = new Dispatcher()
			{
				Email = "h.dupont@helmo-bilite.be",
				UserName = "h.dupont@helmo-bilite.be",
				Matricule = "D000002",
				Name = "Dupont",
				FirstName = "Henry",
				BirthDate = new DateTime(1991, 9, 23),
				Degree = Degree.REDUNDANT,
				ImageName = "henry_dupont.jpg"
			};
			var resultDispatcher2 = await manager.CreateAsync(dispatcher2, "Test123.");
			if (resultDispatcher2.Succeeded)
			{
				await manager.AddToRoleAsync(dispatcher2, nameof(Role.Dispatcher));
			}


			/* Chauffeurs */

			ApplicationUser chauffeur1 = new Chauffeur()
			{
				Email = "n.martin@helmo-bilite.be",
				UserName = "n.martin@helmo-bilite.be",
				Matricule = "C000001",
				Name = "Martin",
				FirstName = "Natalie",
				BirthDate = new DateTime(1987, 1, 13),
				ImageName = "natalie_martin.jpg",
				Licenses = new List<ChauffeurLicense> { new ChauffeurLicense { License = License.C }, new ChauffeurLicense { License = License.B } },
			};
			var resultChauffeur1 = await manager.CreateAsync(chauffeur1, "Test123.");
			if (resultChauffeur1.Succeeded)
			{
				await manager.AddToRoleAsync(chauffeur1, nameof(Role.Chauffeur));
			}

			ApplicationUser chauffeur2 = new Chauffeur()
			{
				Email = "t.dujardin@helmo-bilite.be",
				UserName = "t.dujardin@helmo-bilite.be",
				Matricule = "C000002",
				Name = "Dujardin",
				FirstName = "Thierry",
				BirthDate = new DateTime(1970, 11, 27),
				ImageName = "thierry_dujardin.jpg",
				Licenses = new List<ChauffeurLicense> { new ChauffeurLicense { License = License.C }, new ChauffeurLicense { License = License.CE } }
			};
			var resultChauffeur2 = await manager.CreateAsync(chauffeur2, "Test123.");
			if (resultChauffeur2.Succeeded)
			{
				await manager.AddToRoleAsync(chauffeur2, nameof(Role.Chauffeur));
			}

			ApplicationUser chauffeur3 = new Chauffeur()
			{
				Email = "p.vandenberg@helmo-bilite.be",
				UserName = "p.vandenberg@helmo-bilite.be",
				Matricule = "C000003",
				Name = "VanDenBerg",
				FirstName = "Piet",
				BirthDate = new DateTime(1980, 2, 5),
				ImageName = "piet_vandenberg.jpg",
				Licenses = new List<ChauffeurLicense> { new ChauffeurLicense { License = License.C }, new ChauffeurLicense { License = License.CE } }
			};
			var resultChauffeur3 = await manager.CreateAsync(chauffeur3, "Test123.");
			if (resultChauffeur3.Succeeded)
			{
				await manager.AddToRoleAsync(chauffeur3, nameof(Role.Chauffeur));
			}

			ApplicationUser chauffeur4 = new Chauffeur()
			{
				Email = "m.dubois@helmo-bilite.be",
				UserName = "m.dubois@helmo-bilite.be",
				Matricule = "C000004",
				Name = "Dubois",
				FirstName = "Martine",
				BirthDate = new DateTime(1977, 1, 13),
				ImageName = "martine_dubois.jpg",
				Licenses = new List<ChauffeurLicense> { new ChauffeurLicense { License = License.C }, new ChauffeurLicense { License = License.CE } }
			};
			var resultChauffeur4 = await manager.CreateAsync(chauffeur4, "Test123.");
			if (resultChauffeur4.Succeeded)
			{
				await manager.AddToRoleAsync(chauffeur4, nameof(Role.Chauffeur));
			}

			ApplicationUser chauffeur5 = new Chauffeur()
			{
				Email = "p.demarets@helmo-bilite.be",
				UserName = "p.demarets@helmo-bilite.be",
				Matricule = "C000005",
				Name = "Demarets",
				FirstName = "Pascal",
				BirthDate = new DateTime(1987, 1, 13),
				ImageName = "pascal_demarets.jpg",
				Licenses = new List<ChauffeurLicense> { new ChauffeurLicense { License = License.C }, new ChauffeurLicense { License = License.B } }
			};
			var resultChauffeur5 = await manager.CreateAsync(chauffeur5, "Test123.");
			if (resultChauffeur5.Succeeded)
			{
				await manager.AddToRoleAsync(chauffeur5, nameof(Role.Chauffeur));
			}

			/* Clients */

			ApplicationUser client1 = new Client()
			{
				Email = "e.malone@ablynx.com",
				UserName = "e.malone@ablynx.com",
				Enterprise = "Ablynx",
				ImageName = "ablynx_logo.png",
				Address = new Address
				{
					CountryAddress = "Belgique",
					StreetAddress = "Technologiepark",
					NumberAddress = "4",
					PostCodeAddress = 9052,
					LocalityAddress = "Gand"
				},
				Deliveries = new List<Delivery>
				{
					new Delivery
					{
						LoadingPlace = "Rue de Piétrain 320 Liège",
						UnloadingPlace = "Populierenstraat 297 Aarschot",
						LoadingDateTime = new DateTime(2023, 8, 22, 9, 0, 0),
						UnloadingDateTime = new DateTime(2023, 8, 22, 14, 30, 0),
						Content = "Systèmes d'IRM - ASG MROpen Evo et Fonar Upright",
						Status = Status.WAITING
					},
					new Delivery
					{
						LoadingPlace = "Rue de Virton 44 Ethe",
						UnloadingPlace = "Kapelaniestraat 327 Leval-chaudeville",
						LoadingDateTime = new DateTime(2023, 8, 22, 15, 15, 0),
						UnloadingDateTime = new DateTime(2023, 8, 22, 18, 30, 0),
						Content = "5 x NeuSoft NeuMR",
						Status = Status.WAITING
					},
				}
			};
			var resultClient1 = await manager.CreateAsync(client1, "Test123.");
			if (resultClient1.Succeeded)
			{
				await manager.AddToRoleAsync(client1, nameof(Role.Client));
			}

			ApplicationUser client2 = new Client()
			{
				Email = "g.rehtnug@cisco.com",
				UserName = "g.rehtnug@cisco.com",
				Enterprise = "Cisco System Belgium",
				ImageName = "cisco_logo.png",
				Address = new Address
				{
					CountryAddress = "Belgique",
					StreetAddress = "De Kleetlaan",
					NumberAddress = "6",
					PostCodeAddress = 1831,
					LocalityAddress = "Malines"
				},
				Deliveries = new List<Delivery>
				{
					new Delivery
					{
						LoadingPlace = "Rue de la Sarthe 279 Limbourg",
						UnloadingPlace = "Rue du Chapy 415 Limbourg",
						LoadingDateTime = new DateTime(2023, 8, 25, 7, 30, 0),
						UnloadingDateTime = new DateTime(2023, 8, 25, 10, 0, 0),
						Content = "50 x Bain de soudure T 03, 430 °C - 230V - 360W",
						Status = Status.WAITING
					},
					new Delivery
					{
						LoadingPlace = "Pont du Chêne 53 Blaton",
						UnloadingPlace = "Rue de Sy 331 Forge-philippe",
						LoadingDateTime = new DateTime(2023, 8, 27, 12, 45, 0),
						UnloadingDateTime = new DateTime(2023, 8, 27, 18, 0, 0),
						Content = "30 x Bain de soudure T 10 S, 340 °C - 230V - 130W",
						Status = Status.WAITING
					}
				}
			};
			var resultClient2 = await manager.CreateAsync(client2, "Test123.");
			if (resultClient2.Succeeded)
			{
				await manager.AddToRoleAsync(client2, nameof(Role.Client));
			}

			ApplicationUser client3 = new Client()
			{
				Email = "n.chenard@biocartis.com",
				UserName = "n.chenard@biocartis.com",
				Enterprise = "Biocartis NV",
				ImageName = "biocartis_logo.png",
				Address = new Address
				{
					CountryAddress = "Belgique",
					StreetAddress = "Generaal de Wittelaan",
					NumberAddress = "11",
					PostCodeAddress = 2800,
					LocalityAddress = "Malines"
				},
				Deliveries = new List<Delivery>
				{
					new Delivery
					{
						LoadingPlace = "Lambrechtstraat 286 Antwerp",
						UnloadingPlace = "Chaussée de Tirlemont 189 West Flanders",
						LoadingDateTime = new DateTime(2023, 8, 29, 12, 30, 0),
						UnloadingDateTime = new DateTime(2023, 8, 29, 17, 0, 0),
						Content = "20 x Elixbo PM335 0.35T - XBO Medical System",
						Status = Status.WAITING
					},
					new Delivery
					{
						LoadingPlace = "Route de Botrange 15 Souvret",
						UnloadingPlace = "Lodewijk De Raetlaan 323 Ciplet",
						LoadingDateTime = new DateTime(2023, 8, 25, 10, 30, 0),
						UnloadingDateTime = new DateTime(2023, 8, 25, 16, 0, 0),
						Content = "5 x I_Magnate 1.5T - WDM",
						Status = Status.WAITING
					},
				}
			};

			var resultClient3 = await manager.CreateAsync(client3, "Test123.");
			if (resultClient3.Succeeded)
			{
				await manager.AddToRoleAsync(client3, nameof(Role.Client));
			}

			ApplicationUser client4 = new Client()
			{
				Email = "a.trepanier@huppertzag.com",
				UserName = "a.trepanier@huppertzag.com",
				Enterprise = "Huppertz AG",
				ImageName = "huppertz_logo.png",
				Address = new Address
				{
					CountryAddress = "Belgique",
					StreetAddress = "Steinerberg",
					NumberAddress = "5",
					PostCodeAddress = 4780,
					LocalityAddress = "Sankt Vith"
				},
				Deliveries = new List<Delivery>
				{
					new Delivery
					{
						LoadingPlace = "Place Fayat 423 Hoeselt",
						UnloadingPlace = "Rue du Commerce 56 Wortegem",
						LoadingDateTime = new DateTime(2023, 8, 23, 11, 45, 0),
						UnloadingDateTime = new DateTime(2023, 8, 23, 19, 0, 0),
						Content = "12 x RACK APC 48U",
						Status = Status.WAITING
					},
					new Delivery
					{
						LoadingPlace = "Brixtonlaan 168 Luxembourg",
						UnloadingPlace = "Rue Grande 478 Liège",
						LoadingDateTime = new DateTime(2023, 8, 19, 10, 30, 0),
						UnloadingDateTime = new DateTime(2023, 8, 19, 15, 0, 0),
						Content = "10 x HP ProLiant",
						Status = Status.WAITING
					}
				}
			};
			var resultClient4 = await manager.CreateAsync(client4, "Test123.");
			if (resultClient4.Succeeded)
			{
				await manager.AddToRoleAsync(client4, nameof(Role.Client));
			}

			ApplicationUser client5 = new Client()
			{
				Email = "b.rancourt@cellcarta.com",
				UserName = "b.rancourt@cellcarta.com",
				Enterprise = "CellCarta",
				ImageName = "cellcarta_logo.png",
				Address = new Address
				{
					CountryAddress = "Belgique",
					StreetAddress = "Sint-Bavostraat",
					NumberAddress = "78",
					PostCodeAddress = 2610,
					LocalityAddress = "Anvers"
				},
				Deliveries = new List<Delivery> 
				{
					new Delivery
					{
						LoadingPlace = "Populierenstraat 134 Acosse",
						UnloadingPlace = "Industriestraat 37 Denderbelle",
						LoadingDateTime = new DateTime(2023, 8, 18, 7, 15, 0),
						UnloadingDateTime = new DateTime(2023, 8, 18, 12, 30, 0),
						Content = "3 x Echelon Smart Plus 2t",
						Status = Status.WAITING
					},

					new Delivery
					{
						LoadingPlace = "Port d’Anvers, Quai 1255",
						UnloadingPlace = "Pegasuslaan 1 à 1831 Machelen",
						LoadingDateTime = new DateTime(2023, 8, 18, 5, 0, 0),
						UnloadingDateTime = new DateTime(2023, 8, 18, 10, 0, 0),
						Content = "5 x Vantage Fortan 1.5t",
						Status = Status.WAITING
					}
				}
			};
			var resultClient5 = await manager.CreateAsync(client5, "Test123.");
			if (resultClient5.Succeeded)
			{
				await manager.AddToRoleAsync(client5, nameof(Role.Client));
			}

		}
	}
}
