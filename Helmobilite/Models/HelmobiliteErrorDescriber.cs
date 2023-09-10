using Microsoft.AspNetCore.Identity;

namespace Helmobilite.Models
{
	public class HelmobiliteErrorDescriber : IdentityErrorDescriber
	{
        public override IdentityError DuplicateEmail(string email)
		{
			var error = base.DuplicateEmail(email);
			error.Description = "L'adresse email est déjà utilisée.";
			return error;
		}

		public override IdentityError DuplicateUserName(string userName)
		{
			return DuplicateEmail(userName);
		}

		public override IdentityError PasswordMismatch()
		{
			var error = base.PasswordMismatch();
			error.Description = "Le mot de passe est incorrect";
			return error;
		}

	}
}
