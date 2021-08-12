using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Phrook.Customizations.Identity
{
	public class CommonPasswordValidator<TUser> : IPasswordValidator<TUser> where TUser : class
	{
		private readonly string[] commons;

		
		 public CommonPasswordValidator()
		{
			//TODO: implementare un elenco migliore: https://github.com/danielmiessler/SecLists/tree/master/Passwords/Common-Credentials
			this.commons = new[] {
				"password", "abc", "123", "qwerty"
			};
		}
		public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user, string password)
		{
			IdentityResult result;
			if(commons.Any(common => password.Contains(common, System.StringComparison.CurrentCultureIgnoreCase)))
			{
				result = IdentityResult.Failed(new IdentityError { Description = "Password troppo comune" });
			}
			else 
			{
				result = IdentityResult.Success;
			}
			return Task.FromResult(result);
		}
	}
}