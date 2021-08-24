using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Phrook.Models.Entities;

namespace Phrook.Customizations.Identity
{
	public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser>
	{
		public CustomClaimsPrincipalFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor){}

		protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
		{
			//generates a claim (a pair key-value) that contains the user's fullname and adds that to the existing ones
			ClaimsIdentity identity = await base.GenerateClaimsAsync(user);
			identity.AddClaim(new Claim("FullName", user.FullName));
			return identity;
		}
	}
}