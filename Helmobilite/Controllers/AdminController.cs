using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using Microsoft.AspNetCore.Authorization;
using Helmobilite.Models.ViewModels;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.Data;
using Newtonsoft.Json.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = nameof(Role.Administrateur))]
    public class AdminController : Controller
    {
        private readonly HelmobiliteDbContext _context;

        public AdminController(HelmobiliteDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public IActionResult Index()
        {
            return RedirectToAction("Statistics");
        }

        public IActionResult Statistics(string filterClient = "", string filterChauffeur = "", string filterDate = "", string sortOrder = "")
        {
			ViewBag.FilterOptions = new List<string>() { "Client", "Chauffeur", "Date" };
			ViewBag.FilterClient = filterClient;
			ViewBag.FilterChauffeur = filterChauffeur;
			ViewBag.FilterDate = filterDate;

			ViewBag.CurrentSort = sortOrder;
			ViewBag.DateSort = string.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
			ViewBag.ChauffeurSort = sortOrder == "Chauffeur" ? "Chauffeur_desc" : "Chauffeur";
			ViewBag.ClientSort = sortOrder == "Client" ? "Client_desc" : "Client";
			ViewBag.StatusSort = sortOrder == "Status" ? "Status_desc" : "Status";

			var deliveries = _context.Deliveries.Where(d => d.Status == Status.FAILED || d.Status == Status.DONE).Include(d => d.Chauffeur).Include(d => d.Client).ToList();
			if (!string.IsNullOrWhiteSpace(filterClient))
			{
				var client = _context.Clients.Where(c => c.Enterprise == filterClient).FirstOrDefault();
				deliveries = client != null ? deliveries.Where(d => d.ClientId == client.Id).ToList() : new List<Delivery>();
			}

			if (!string.IsNullOrWhiteSpace(filterChauffeur))
			{
				deliveries = deliveries.Where(d => d.Chauffeur != null && d.Chauffeur.DisplayName == filterChauffeur).ToList();
			}

			if (!string.IsNullOrWhiteSpace(filterDate))
			{
				deliveries = DateTime.TryParse(filterDate, out var parsedDate)
							? deliveries.Where(d => d.LoadingDateTime.Date == parsedDate.Date || d.UnloadingDateTime.Date == parsedDate.Date).ToList()
							: new List<Delivery>();
			}
			
			var statisticsViewModel = new StatisticsViewModel(GetDeliveryViewModels(deliveries, sortOrder));

            return View(statisticsViewModel);
        }

		private List<DeliveryViewModel> GetDeliveryViewModels(List<Delivery> deliveries, string sortOrder)
		{
			var deliveryViewModels = deliveries.Select(d => new  DeliveryViewModel(d)).ToList();

			return sortOrder switch
			{
				"Date_desc" => deliveryViewModels.OrderByDescending(d => d.LoadingDateTime).ToList(),
				"Chauffeur" => deliveryViewModels.OrderBy(d => d.Chauffeur).ToList(),
				"Chauffeur_desc" => deliveryViewModels.OrderByDescending(d => d.Chauffeur).ToList(),
				"Client" => deliveryViewModels.OrderBy(d => d.ClientEnterprise).ToList(),
				"Client_desc" => deliveryViewModels.OrderByDescending(d => d.ClientEnterprise).ToList(),
				"Status" => deliveryViewModels.OrderBy(d => d.Status.GetEnumDisplayName()).ToList(),
				"Status_desc" => deliveryViewModels.OrderByDescending(d => d.Status.GetEnumDisplayName()).ToList(),
				_ => deliveryViewModels.OrderBy(d => d.LoadingDateTime).ToList(),
			};
		}

		public IActionResult ManageLicenses()
		{
            var chauffeurs = _context.Chauffeurs.OrderBy(c => c.Matricule).Include(c => c.Licenses).ToList();
			return View(chauffeurs);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChangeLicenses([Bind("ChauffeurId", "SelectedCheckboxes")] ChauffeurLicenseViewModel viewModel)
		{
			if (!ModelState.IsValid)
			{
				return RedirectToAction("ManageLicenses");
			}

			var chauffeur = _context.Chauffeurs.Where(c => c.Id == viewModel.ChauffeurId).Include(c => c.Licenses).Include(c => c.Deliveries).ThenInclude(d => d.Truck).FirstOrDefault();
			if (chauffeur == null)
			{
				return RedirectToAction("ManageLicenses");
			}
			chauffeur.UpdateLicenses(viewModel.SelectedLicenses);

			_context.Update(chauffeur);
			await _context.SaveChangesAsync();

			return RedirectToAction("ManageLicenses");
		}

		public IActionResult ManageClients()
		{
            var clients = _context.Clients.Where(c => c.Deliveries.Any()).Include(c => c.Address).ToList();
            return View(clients);
		}

		public IActionResult MarkAsBadPayer(string id)
        {
			return MarkClient(id, true).Result;
		}

		public IActionResult MarkAsGoodPayer(string id)
		{
            return MarkClient(id, false).Result;
		}

        private async Task<IActionResult> MarkClient(string id, bool isBadPayer)
        {
			var client = _context.Clients.Where(c => c.Id == id).FirstOrDefault();
			if (client != null)
			{
				client.IsBadPayer = isBadPayer;
				_context.Update(client);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("ManageClients");
		}

		public IActionResult ManageTrucks()
		{
            return RedirectToAction("Index", "Trucks");
		}

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchChauffeurName()
		{
            string term = HttpContext.Request.Query["searchName"].ToString().Trim();
            var list = await _context.Chauffeurs.Where(c => c.Name.Contains(term) || c.FirstName.Contains(term)).Select(c => c.Name + " " + c.FirstName).ToListAsync();
            return Ok(list);
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchClientEnterprise()
        {
            string term = HttpContext.Request.Query["searchEnterprise"].ToString().Trim();
            var list = await _context.Clients.Where(c => c.Enterprise.Contains(term)).Select(c => c.Enterprise).ToListAsync();
            return Ok(list);
        }

		[ValidateAntiForgeryToken]
		public IActionResult GetLicensesModalForm(string chauffeurId)
		{
			var chauffeur = _context.Chauffeurs.Where(c => c.Id == chauffeurId).Include(c => c.Licenses).FirstOrDefault();
			if (chauffeur == null)
			{
				return BadRequest("Une erreur s'est produite, veuillez réessayer plus tard.");
			}
			else
			{
				var viewModel = new ChauffeurLicenseViewModel(chauffeur);
				return PartialView("_ChauffeurLicensesFormModal", viewModel);
			}
		}
	}
}
