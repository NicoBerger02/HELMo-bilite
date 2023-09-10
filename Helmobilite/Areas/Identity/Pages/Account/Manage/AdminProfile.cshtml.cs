#nullable disable

using Helmobilite.Models;
using Helmobilite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helmobilite.Areas.Identity.Pages.Account.Manage
{
	[ValidateAntiForgeryToken]
	[Authorize(Roles = nameof(Role.Administrateur))]
    public class AdminProfileModel : PageModel
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly HelmobiliteDbContext _context;
		private readonly IImageService _imageService;

		public AdminProfileModel(
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
			public Administrator Administrator { get; set; }

			public IFormFile ProfileImageUploaded { get; set; }
		}

		private void Load(ApplicationUser user)
		{
			var administrator = _context.Administrators.Where(d => d.Id == user.Id).First();
			Input = new InputModel
			{
				Administrator = administrator,
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
			var administrator = _context.Administrators.Where(d => d.Id == user.Id).First();

			if (Input.ProfileImageUploaded != null)
			{
				administrator.ImageName = _imageService.ReplaceImage(Input.ProfileImageUploaded, ImageFor.User, administrator.ImageName ?? "");
			}

			administrator.FirstName = Input.Administrator.FirstName;
			administrator.Name = Input.Administrator.Name;
			administrator.Matricule = Input.Administrator.Matricule;
			administrator.BirthDate = Input.Administrator.BirthDate;

			_context.SaveChanges();
		}
	}
}
