using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Helmobilite.Models;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = nameof(Role.Client))]
    public class ClientController : Controller
    {

        public ClientController()
        {
        }

        // GET: Client
        public IActionResult Index()
        {
			return RedirectToAction("ClientIndex", "Deliveries");
		}

        public IActionResult DeliveryHistory()
        {
			return RedirectToAction("ClientHistory", "Deliveries");
		}

	}
}
