// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Helmobilite.Models;
using Helmobilite.Validations;

namespace Helmobilite.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> _signInManager;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IUserStore<ApplicationUser> _userStore;
		private readonly IUserEmailStore<ApplicationUser> _emailStore;
		private readonly ILogger<RegisterModel> _logger;
		private readonly IEmailSender _emailSender;

		public RegisterModel(
			UserManager<ApplicationUser> userManager,
			IUserStore<ApplicationUser> userStore,
			SignInManager<ApplicationUser> signInManager,
			ILogger<RegisterModel> logger,
			IEmailSender emailSender)
		{
			_userManager = userManager;
			_userStore = userStore;
			_emailStore = GetEmailStore();
			_signInManager = signInManager;
			_logger = logger;
			_emailSender = emailSender;
		}

		[BindProperty]
		public Role Role { get; set; }

		[BindProperty]
		public InputModel Input { get; set; }

		[BindProperty]
		public List<LicenseCheckBox> SelectedLicenses { get; set; }

		public string ReturnUrl { get; set; }

		public IList<AuthenticationScheme> ExternalLogins { get; set; }

		public class LicenseCheckBox
		{
			public string Text { get; set; }
			public bool IsChecked { get; set; }
		}

		public class InputModel
		{
			public RegisterChauffeurModel ChauffeurModel { get; set; }
			public RegisterClientModel ClientModel { get; set; }
			public RegisterDispatcherModel DispatcherModel { get; set; }
		}

		public class RegisterUserModel
		{
			[Required(ErrorMessage = "L'adresse email est obligatoire.")]
			[EmailAddress(ErrorMessage = "Adresse email invalide")]
			public  string Email { get; set; }

			[Required(ErrorMessage = "Le mot de passe est obligatoire")]
			[DataType(DataType.Password)]
			[Display(Name = "Mot de passe")]
			[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,32}$", ErrorMessage = "Le mot de passe doit contenir au minimum une majuscule, une minuscule, un chiffre et un caractère spécial.")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirmation")]
			[Compare("Password", ErrorMessage = "Le mot de passe et la confirmation sont différents")]
			public string ConfirmPassword { get; set; }
		}

		public class RegisterMemberModel : RegisterUserModel
		{
			[Required(ErrorMessage = "Le nom ne peut pas être vide.")]
			[MaxLength(20, ErrorMessage = "20 caractères maximum")]
			public string Name { get; set; }

			[Required(ErrorMessage = "Le prénom ne peut pas être vide.")]
			[MaxLength(20, ErrorMessage = "20 caractères maximum")]
			public string FirstName { get; set; }

			[Matricule]
			[Required(ErrorMessage = "Le matricule ne peut pas être vide.")]
			[RegularExpression("^[A-Z]\\d{6}$", ErrorMessage = "Le matricule doit être composé d'une lettre majuscule suivie de 6 chiffres.")]
			public string Matricule { get; set; }
		}

		public class RegisterChauffeurModel : RegisterMemberModel
		{
		}

		public class RegisterClientModel : RegisterUserModel
		{
			[Required(ErrorMessage = "Le nom de l'entreprise ne peut pas être vide.")]
			[DataType(DataType.Text)]
			public string Enterprise { get; set; }

			[Required(ErrorMessage = "La rue ne peut pas être vide.")]
			[DataType(DataType.Text)]
			[MaxLength(100, ErrorMessage = "100 caractères maximum")]
			public string StreetAddress { get; set; }

			[Required(ErrorMessage = "La numéro ne peut pas être vide.")]
			[DataType(DataType.Text)]
			[MaxLength(10, ErrorMessage = "10 caractères maximum")]
			public string NumberAddress { get; set; }

			[Required(ErrorMessage = "Le code postal ne peut pas être vide.")]
			[DataType(DataType.PostalCode, ErrorMessage = "Veuillez spécifier un code postal correct.")]
			public int PostCodeAddress { get; set; }

			[Required(ErrorMessage = "La localité ne peut pas être vide.")]
			[MaxLength(20, ErrorMessage = "20 caractères maximum")]
			[DataType(DataType.Text)]
			public string LocalityAddress { get; set; }

			[Required(ErrorMessage = "Le pays ne peut pas être vide.")]
			[DataType(DataType.Text)]
			[MaxLength(20, ErrorMessage = "20 caractères maximum")]
			public string CountryAddress { get; set; }
		}

		public class RegisterDispatcherModel : RegisterMemberModel
		{
			[Required(ErrorMessage = "Veuillez sélectionner votre niveau d'étude.")]
			public Degree Degree { get; set; }
		}

		public async Task<IActionResult> OnGetAsync(string selectedRole, string returnUrl = null)
		{
			ReturnUrl = returnUrl;
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (Enum.TryParse(selectedRole, out Role parsedRole))
			{
				Role = parsedRole;
				SelectedLicenses = new List<LicenseCheckBox>
				{
					new LicenseCheckBox() { IsChecked = false, Text = License.B.GetEnumDisplayName() },
					new LicenseCheckBox() { IsChecked = true, Text = License.C.GetEnumDisplayName() },
					new LicenseCheckBox() { IsChecked = false, Text = License.CE.GetEnumDisplayName() }
				};
				return Page();
			} else
			{
				return RedirectToAction("Index", "Home");
			}
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl ??= Url.Content("~/");
			ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
			if (ModelState.IsValid)
			{
				var user = CreateUser(Role);
				var password = GetPassword(Role);

				await _userStore.SetUserNameAsync(user, user.Email, CancellationToken.None);
				await _emailStore.SetEmailAsync(user, user.Email, CancellationToken.None);
				var result = await _userManager.CreateAsync(user, password);

				if (result.Succeeded)
				{
					_logger.LogInformation("User created a new account with password.");

					if (_userManager.Options.SignIn.RequireConfirmedAccount)
					{
						return RedirectToPage("RegisterConfirmation", new { email = user.Email, returnUrl = returnUrl });
					}
					else
					{
						var resultAddRole = await _userManager.AddToRoleAsync(user, Enum.GetName(typeof(Role), Role));
						if (resultAddRole.Succeeded)
						{
							await _signInManager.SignInAsync(user, isPersistent: false);
							return RedirectToAction("Index", "Home");
						}
						else
						{
							foreach (var error in resultAddRole.Errors)
							{
								ModelState.AddModelError(string.Empty, error.Description);
							}
						}
					}
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}

			return Page();
		}

		private string GetPassword(Role role)
		{
			return role switch
			{
				Role.Client => Input.ClientModel.Password,
				Role.Dispatcher => Input.DispatcherModel.Password,
				Role.Chauffeur => Input.ChauffeurModel.Password,
				_ => ""
			};
		}

		private ApplicationUser CreateUser(Role role)
		{
			try
			{
				switch (role)
				{
					case Role.Client:
						var clientModel = Input.ClientModel;
						var address = new Address(clientModel.StreetAddress, clientModel.NumberAddress, clientModel.PostCodeAddress, clientModel.LocalityAddress, clientModel.CountryAddress);
						return new Client(clientModel.Enterprise, clientModel.Email, address);
					case Role.Dispatcher:
						var dispatcherModel = Input.DispatcherModel;
						return new Dispatcher(dispatcherModel.Email, dispatcherModel.Name, dispatcherModel.FirstName, dispatcherModel.Matricule, dispatcherModel.Degree);
					case Role.Chauffeur:
						var licenses = SelectedLicenses.Where(l => l.IsChecked).Select(l => (License)Enum.Parse(typeof(License), l.Text)).ToList();
						var chauffeurModel = Input.ChauffeurModel;
						var chauffeur = new Chauffeur(chauffeurModel.Email, chauffeurModel.Name ,chauffeurModel.FirstName, chauffeurModel.Matricule, licenses);
						return chauffeur;
					default:
						return null;
				}
			}
			catch
			{
				throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
					$"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
					$"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
			}
		}

		private IUserEmailStore<ApplicationUser> GetEmailStore()
		{
			if (!_userManager.SupportsUserEmail)
			{
				throw new NotSupportedException("The default UI requires a user store with email support.");
			}
			return (IUserEmailStore<ApplicationUser>)_userStore;
		}
	}
}
