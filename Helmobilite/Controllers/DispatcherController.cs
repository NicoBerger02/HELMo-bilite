using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Helmobilite.Models;
using Microsoft.AspNetCore.Authorization;

namespace Helmobilite.Controllers
{
    [Authorize(Roles = nameof(Role.Dispatcher))]
    public class DispatcherController : Controller
    {
        public DispatcherController(HelmobiliteDbContext context) { }

        public IActionResult Index()
        {
            return RedirectToAction("DispatcherIndex", "Deliveries");
        }
    }
}
