using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.AspNetCore.Identity;

namespace Helmobilite.Controllers
{

    [Authorize(Roles = nameof(Role.Chauffeur))]
    public class ChauffeurController : Controller
    {

        public ChauffeurController() { }

		public IActionResult Index()
		{
			return RedirectToAction("ChauffeurIndex", "Deliveries");
		}
    }
}
