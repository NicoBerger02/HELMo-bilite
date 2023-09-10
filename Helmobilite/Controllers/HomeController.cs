using Helmobilite.Models;
using Helmobilite.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;

namespace Helmobilite.Controllers
{
    public class HomeController : Controller
    {
		private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
		private readonly HelmobiliteDbContext _context;
		private readonly ILogger<HomeController> _logger;

        public HomeController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILogger<HomeController> logger, HelmobiliteDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

				return user switch
				{
					Client => RedirectToAction("Index", "Client"),
					Chauffeur => RedirectToAction("Index", "Chauffeur"),
					Dispatcher => RedirectToAction("Index", "Dispatcher"),
					_ => RedirectToAction("Index", "Admin"),
				};
			}
            List<Client> clients = _context.Clients.Where(c => c.ImageName != null).ToList();
            var dispatcher = _context.Dispatchers.FirstOrDefault(d => d.ImageName != null);
            var chauffeur = _context.Chauffeurs.FirstOrDefault(c => c.ImageName != null);

            var trucks = _context.Trucks.Where(t => t.ImageName != null).ToList();
            
            return View(new HomePageViewModel(clients, dispatcher, chauffeur, trucks));
        }

		[ResponseCache(Duration = 60 * 60 * 24, Location = ResponseCacheLocation.Any, NoStore = false)]
		public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
	}
}