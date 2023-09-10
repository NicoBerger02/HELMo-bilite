using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using Helmobilite.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Helmobilite.Services;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = nameof(Role.Administrateur))]
    public class TrucksController : Controller
    {
        private readonly HelmobiliteDbContext _context;
        private readonly IImageService _imageService;

        public TrucksController(HelmobiliteDbContext context, IImageService imageService)
        {
            _context = context;
            _imageService = imageService;
        }

        // GET: Trucks
        public async Task<IActionResult> Index()
        {
              return _context.Trucks != null ? 
                          View(await _context.Trucks.ToListAsync()) :
                          Problem("Entity set 'HelmobiliteDbContext.Trucks'  is null.");
        }

        // GET: Trucks/Create
        public IActionResult Create()
        {
            var model = new TruckViewModel { PhotoIsRequired = true };
            return View(model);
        }

        // POST: Trucks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LicensePlate,Brand,Model,ImageUploaded,Payload,PhotoIsRequired")] TruckViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
				var truck = new Truck
				{
					Brand = viewModel.Brand,
					Model = viewModel.Model,
					LicensePlate = viewModel.LicensePlate,
					Payload = viewModel.Payload,
					ImageName = _imageService.ReplaceImage(viewModel.ImageUploaded, ImageFor.Truck)
				};

				_context.Add(truck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: Trucks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Trucks == null)
            {
				return RedirectToAction(nameof(Index));
			}

            var truck = await _context.Trucks.FindAsync(id);
            if (truck == null)
            {
				return RedirectToAction(nameof(Index));
			}
            var truckViewModel = new TruckViewModel(truck);

            return View(truckViewModel);
        }

        // POST: Trucks/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LicensePlate,Brand,Model,License,ImageUploaded,Payload,PhotoIsRequired,TruckId")] TruckViewModel viewModel)
        {
            if (id != viewModel.TruckId)
            {
				return RedirectToAction(nameof(Index));
			}

            if (ModelState.IsValid)
            {
                try
                {
                    await SaveChanges(viewModel);
                }
                catch (DbUpdateConcurrencyException)
                {
				}
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        private async Task SaveChanges(TruckViewModel viewModel)
        {
            var truck = _context.Trucks.Where(t => t.Id == viewModel.TruckId).Include(t => t.Deliveries).ThenInclude(l => l.Chauffeur).ThenInclude(c => c.Licenses).First();
            if (truck != null)
            {
                if (viewModel.ImageUploaded != null)
                {
                    truck.ImageName = _imageService.ReplaceImage(viewModel.ImageUploaded, ImageFor.Truck, truck.ImageName);
                }

                truck.UpdateInfos(viewModel.LicensePlate, viewModel.Brand, viewModel.Model, viewModel.Payload);
                _context.Update(truck);
                await _context.SaveChangesAsync();
            } else
            {
                throw new DbUpdateConcurrencyException();
            }
        }

		public IActionResult Delete(int id)
		{
			var truck = _context.Trucks.Include(t => t.Deliveries).ThenInclude(l => l.Chauffeur).FirstOrDefault(f => f.Id == id);
			if (truck != null)
			{
                truck.RemoveOnGoingDeliveries();
				_context.Trucks.Remove(truck);

                _imageService.DeleteImage(truck.ImageName, ImageFor.Truck);

				_context.SaveChanges();
			}
			return RedirectToAction("Index");
		}

		private bool TruckExists(int id)
        {
          return (_context.Trucks?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
