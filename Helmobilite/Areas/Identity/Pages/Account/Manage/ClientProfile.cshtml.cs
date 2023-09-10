#nullable disable

using Helmobilite.Models;
using Helmobilite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Helmobilite.Areas.Identity.Pages.Account.Manage
{
    [ValidateAntiForgeryToken]
	[Authorize(Roles = nameof(Role.Client))]
	public class ClientProfileModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HelmobiliteDbContext _context;
		private readonly IImageService _imageService;

		public ClientProfileModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            HelmobiliteDbContext context,
			IImageService imageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
			_imageService = imageService;
		}

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required(ErrorMessage = "Le nom de l'entreprise ne peut pas être vide.")]
            public string Enterprise { get; set; }

            public Address Address { get; set; }

			public IFormFile EnterpriseImageUploaded { get; set; }
		}

        private void Load(ApplicationUser user)
        {
            var client = _context.Clients.Where(c => c.Id == user.Id).Include(c => c.Address).First();
            Input = new InputModel
            {
                Address = client.Address,
                Enterprise = client.Enterprise,
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            Load(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
			if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                Load(user);
                return Page();
            }

            SaveChanges(user);
            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Vos informations ont été enregistrées";
            return RedirectToPage();
        }

        private void SaveChanges(ApplicationUser user)
        {
			var client = _context.Clients.Where(c => c.Id == user.Id).Include(c => c.Address).First();

			if (Input.EnterpriseImageUploaded != null)
            {
				client.ImageName = _imageService.ReplaceImage(Input.EnterpriseImageUploaded, ImageFor.User, client.ImageName ?? "");
			}

			client.Enterprise = Input.Enterprise;
			client.Address.CountryAddress = Input.Address.CountryAddress;
			client.Address.StreetAddress = Input.Address.StreetAddress;
			client.Address.PostCodeAddress = Input.Address.PostCodeAddress;
			client.Address.NumberAddress = Input.Address.NumberAddress;
			client.Address.LocalityAddress = Input.Address.LocalityAddress;

			_context.SaveChanges();
		}
    }
}
