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
	[Authorize(Roles = nameof(Role.Dispatcher))]
	public class DispatcherProfileModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly HelmobiliteDbContext _context;
		private readonly IImageService _imageService;

		public DispatcherProfileModel(
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
			public Dispatcher Dispatcher { get; set; }

			public IFormFile ProfileImageUploaded { get; set; }
		}

		private void Load(ApplicationUser user)
		{
			var dispatcher = _context.Dispatchers.Where(d => d.Id == user.Id).First();
			Input = new InputModel
			{
				Dispatcher = dispatcher,
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
			var dispatcher = _context.Dispatchers.Where(d => d.Id == user.Id).First();

			if (Input.ProfileImageUploaded != null)
			{
				dispatcher.ImageName = _imageService.ReplaceImage(Input.ProfileImageUploaded, ImageFor.User, dispatcher.ImageName ?? "");
			}

			dispatcher.FirstName = Input.Dispatcher.FirstName;
			dispatcher.Name = Input.Dispatcher.Name;
			dispatcher.Matricule = Input.Dispatcher.Matricule;
			dispatcher.BirthDate = Input.Dispatcher.BirthDate;
			dispatcher.Degree = Input.Dispatcher.Degree;

			_context.SaveChanges();
		}
	}
}
