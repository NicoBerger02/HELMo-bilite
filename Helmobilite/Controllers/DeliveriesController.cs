using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Extensions;
using Helmobilite.Models;
using Microsoft.VisualBasic;
using Helmobilite.Models.ViewModels;
using System.Diagnostics;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.AspNetCore.SignalR;

namespace Helmobilite.Controllers
{
    [Authorize]
    public class DeliveriesController : Controller
    {
        private readonly HelmobiliteDbContext _context;
        private readonly IHubContext<DeliveryHub> _hubContext;

		private readonly UserManager<ApplicationUser> _userManager;
        private readonly ScheduleCalculator _scheduleCalculator;

        public DeliveriesController(UserManager<ApplicationUser> userManager, HelmobiliteDbContext context, IHubContext<DeliveryHub> hubContext)
        {
            _context = context;
            _userManager = userManager;
			_scheduleCalculator = new ScheduleCalculator();
            _hubContext = hubContext;
		}

        [Authorize(Roles = nameof(Role.Client))]
        public async Task<IActionResult> ClientIndex()
        {
			string clientId = _userManager.GetUserAsync(User).Result.Id;
           
            List<Delivery> deliveries = 
                await _context.Deliveries
                .Where(d => d.ClientId == clientId && (d.Status == Status.WAITING || d.Status == Status.ONGOING))
                .Include(ch => ch.Chauffeur)
                .OrderByDescending(d => d.Status)
                .ToListAsync();

			return _context.Deliveries != null ?
						View(deliveries) :
						Problem("Entity set 'HelmobiliteDbContext.Deliveries'  is null.");
		}

        [Authorize(Roles = nameof(Role.Chauffeur))]
        public async Task<IActionResult> ChauffeurIndex(int weekOffset = 0)
        {
			string chauffeurId = _userManager.GetUserAsync(User).Result.Id;

			var datesOfWeek = _scheduleCalculator.CalculateDatesOfWeekByWeekOffset(weekOffset);

			List<Delivery> deliveries =
			    await _context.Deliveries
			       .Where(d => d.Chauffeur != null && d.Chauffeur.Id == chauffeurId && d.LoadingDateTime >= datesOfWeek[0] && d.LoadingDateTime <= datesOfWeek[0].AddDays(7))
			       .OrderByDescending(d => d.LoadingDateTime)
			       .ToListAsync();

            IDictionary<DateTime, IList<Delivery>> deliveriesPerDate = new Dictionary<DateTime, IList<Delivery>>();

            foreach(var dateOfWeek in datesOfWeek)
            {
                deliveriesPerDate.Add(dateOfWeek, deliveries.Where(d => d.LoadingDateTime.Date == dateOfWeek).ToList());
            }

			var deliveriesPerWeekModel = new DeliveriesPerWeekModel
            {
                WeekOffset = weekOffset,
                DeliveriesPerDate = deliveriesPerDate
            };

            return View(deliveriesPerWeekModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = nameof(Role.Chauffeur))]
		public async Task<IActionResult> SucceededDelivery([Bind("DeliveryId", "WeekOffset", "Comment")] ChauffeurSucceededDeliveryModel model)
		{
			if (!ModelState.IsValid)
			{
				ViewBag.ErrorMessage = "Une erreur est survenue, veuillez réessayer plus tard";
			}
            await SetDeliveryAsFinished(false, model.DeliveryId, model.Comment);

			return RedirectToAction("ChauffeurIndex", new { weekOffset = model.WeekOffset });
		}

		[HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = nameof(Role.Chauffeur))]
        public async Task<IActionResult> FailedDelivery([Bind("DeliveryId", "WeekOffset", "Motif")] ChauffeurFailedDeliveryModel model)
        {
            if (!ModelState.IsValid)
            {
				ViewBag.ErrorMessage = "Une erreur est survenue, veuillez réessayer plus tard";
			}
            await SetDeliveryAsFinished(true, model.DeliveryId, motif: model.Motif);

			return RedirectToAction("ChauffeurIndex", new { weekOffset =  model.WeekOffset });
        }

        private async Task SetDeliveryAsFinished(bool hasFailed, int deliveryId, string comment = "", Motif motif = Motif.ACCIDENT)
        {
            if (!DeliveryExists(deliveryId))
            {
				ViewBag.ErrorMessage = "La livraison n'a pas été trouvée";
			} else
            {
				var delivery = await _context.Deliveries.FindAsync(deliveryId);
                if (hasFailed)
                {
					delivery.HasFailed(motif);
				}
                else
                {
					delivery.HasSucceeded(comment);
				}
				_context.SaveChanges();
			}
        }


		[Authorize(Roles = nameof(Role.Dispatcher))]
        public async Task<IActionResult> DispatcherIndex()
        {
			if (TempData.ContainsKey("ErrorMessage"))
            {
				ViewBag.ErrorMessage = TempData["ErrorMessage"] as string;
			}

            if (TempData.ContainsKey("SuccessMessage"))
            {
                ViewBag.SuccessMessage = TempData["SuccessMessage"] as string;
            }

                List<Delivery> deliveries =
                    await _context.Deliveries
                       .Where(d => d.Status == Status.WAITING && d.LoadingDateTime > DateTime.Now)
                       .Include(d => d.Client)
				       .OrderBy(d => d.LoadingDateTime)
                       .ToListAsync();

            return View(deliveries.OrderBy(d => d.Client.IsBadPayer));
        }

        [Authorize(Roles = nameof(Role.Dispatcher))]
		public async Task<IActionResult> DispatcherAssignChauffeur(int deliveryId)
		{
			var delivery = _context.Deliveries.Where(d => d.Id == deliveryId).Include(d => d.Client).FirstOrDefault();
            if (delivery == null || (delivery.Client != null && delivery.Client.IsBadPayer))
            {
				TempData["ErrorMessage"] = "Une erreur est survenue, veuillez réessayer plus tard.";
				return RedirectToAction("DispatcherIndex");
			}
			var chauffeurs = await _context.Chauffeurs.Include(c => c.Licenses).Include(c => c.Deliveries).ToListAsync();
			var trucks = await _context.Trucks.Include(c => c.Deliveries).ToListAsync();

			var availableTrucks = trucks.Where(t => t.IsAvailableForNewDelivery(delivery)).ToList();
            if (availableTrucks.Count == 0)
            {
				TempData["ErrorMessage"] = "Aucun camion n'est disponible pour effectuer la livraison sélectionnée";
				return RedirectToAction("DispatcherIndex");
			}

            var availableChauffeurs = chauffeurs.Where(c => c.IsAvailableForNewDelivery(delivery) && availableTrucks.Any(truck => c.CanDriveTruck(truck))).ToList();
			if (availableChauffeurs.Count == 0)
			{
                TempData["ErrorMessage"] = "Aucun chauffeur n'est disponible pour effectuer la livraison sélectionnée";
				return RedirectToAction("DispatcherIndex");
			}

			var model = new ChauffeurAssignmentModel
			{
				AvailableChauffeurs = availableChauffeurs,
				DeliveryId = deliveryId
			};

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = nameof(Role.Dispatcher))]
		public async Task<IActionResult> AssignChauffeur([Bind("DeliveryId", "SelectedChauffeurId", "SelectedTruckId")] ChauffeurAssignmentModel assignmentModel)
		{
            if (ModelState.IsValid)
            {
                var delivery = await _context.Deliveries.FindAsync(assignmentModel.DeliveryId);
                var chauffeur = await _context.Chauffeurs.FindAsync(assignmentModel.SelectedChauffeurId);
                var truck = await _context.Trucks.FindAsync(assignmentModel.SelectedTruckId);
                if (delivery == null || chauffeur == null || truck == null)
                {
					TempData["ErrorMessage"] = "Une erreur est survenue, veuillez réessayer plus tard";
				}
                else
                {
					delivery.AddAssignment(chauffeur, truck);
					_context.SaveChanges();
					TempData["SuccessMessage"] = "Assignation validée";
				}
                
            }
            return RedirectToAction("DispatcherIndex");
        }

		[Authorize(Roles = nameof(Role.Client))]
		public async Task<IActionResult> ClientHistory()
		{
			string id = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            ViewBag.Status = "Terminée";

            return View(await _context.Deliveries.Where(c => c.ClientId == id && (c.Status == Status.FAILED || c.Status == Status.DONE)).ToListAsync());
						
		}

        // GET: Deliveries/Create
        [Authorize(Roles = nameof(Role.Client))]
		public IActionResult Create()
        {
            ViewBag.ClientId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
			return View();
        }

        // POST: Deliveries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Roles = nameof(Role.Client))]
		public async Task<IActionResult> Create([Bind("ClientId,LoadingPlace,UnloadingPlace,LoadingDateTime,UnloadingDateTime,Content")] Delivery delivery)
        {
            string ClientId = _userManager.GetUserAsync(HttpContext.User).Result.Id;
            if (ModelState.IsValid && ClientId.Equals(delivery.ClientId))
            {
				Client client = _context.Clients.Where(c => c.Id.Equals(ClientId)).Single();
				delivery.ClientId = client.Id;
				delivery.Status = Status.WAITING;

				_context.Add(delivery);
                await _context.SaveChangesAsync();

				await _hubContext.Clients.All.SendAsync("NewDeliveryEncoded");

				return RedirectToAction("ClientIndex");
            }
			ViewBag.ClientId = ClientId;
			return View(delivery);
        }

		[Authorize(Roles = nameof(Role.Client))]
		public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Deliveries == null)
            {
				return RedirectToAction("ClientIndex");
			}

            var delivery = await _context.Deliveries.FindAsync(id);
            if (delivery == null || delivery.Status != Status.WAITING)
            {
                return RedirectToAction("ClientIndex");
            }
            return View(delivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = nameof(Role.Client))]
		public async Task<IActionResult> Edit(int id, [Bind("Id,ClientId,LoadingPlace,UnloadingPlace,LoadingDateTime,UnloadingDateTime,Content,Status")] Delivery delivery)
        {
            if (id != delivery.Id)
            {
                return RedirectToAction("ClientIndex");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(delivery);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
					return RedirectToAction("ClientIndex");
				}
				return RedirectToAction("Index", "Client");
			}
            return View(delivery);
        }

        private bool DeliveryExists(int id)
        {
          return (_context.Deliveries?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [Produces("application/json")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> GetAvailableTrucksForSelectedChauffeur(string chauffeurId, int deliveryId)
        {
            var delivery = _context.Deliveries.Where(d => d.Id == deliveryId).First();

            var chauffeur = _context.Chauffeurs.Where(c => c.Id == chauffeurId).Include(c => c.Licenses).First();

            var trucks = await _context.Trucks.Include(t => t.Deliveries).ToListAsync();
            var availableTrucks = trucks.Where(t => t.IsAvailableForNewDelivery(delivery) && chauffeur.CanDriveTruck(t)).ToList();

            return Json(availableTrucks.Select(truck => new { truck.Id, truck.DisplayName}).ToList());
        }

		[ValidateAntiForgeryToken]
		public IActionResult GetDeliveryFailedForm(int deliveryId, int weekOffset)
        {
            var model = new ChauffeurFailedDeliveryModel
            {
                DeliveryId = deliveryId,
                WeekOffset = weekOffset
			};
			return PartialView("_ChauffeurFailedDeliveryFormPartial", model);
		}

		[ValidateAntiForgeryToken]
		public IActionResult GetDeliverySucceededForm(int deliveryId, int weekOffset)
		{
			var model = new ChauffeurSucceededDeliveryModel
			{
				DeliveryId = deliveryId,
				WeekOffset = weekOffset
			};
			return PartialView("_ChauffeurSucceededDeliveryFormPartial", model);
		}

		[ValidateAntiForgeryToken]
		public IActionResult GetChauffeurDeliveryDetails(int deliveryId, int weekOffset)
		{
			var delivery = 
                _context.Deliveries
                .Where(d => d.Id == deliveryId)
                .Include(d => d.Truck)
                .Include(d => d.Client)
                .ThenInclude(c => c.Address)
                .FirstOrDefault();

            if (delivery == null || delivery.Client == null)
            {
                return BadRequest("Une erreur s'est produite, veuillez réessayer plus tard.");
            }

            var model = new ChauffeurDeliveryModel(weekOffset, delivery);

			return PartialView("_ChauffeurDetailsModalPartial", model);
		}

        [ValidateAntiForgeryToken]
        public IActionResult GetClientDeliveryDetails(int deliveryId)
		{
            var delivery = _context.Deliveries.Where(d => d.Id == deliveryId).Include(ch => ch.Chauffeur).First();
			return PartialView("_ClientDetailsModalPartial", delivery);
		}
	}
}
