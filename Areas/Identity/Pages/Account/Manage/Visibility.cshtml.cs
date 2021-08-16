using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Phrook.Models.Entities;

namespace Phrook.Areas.Identity.Pages.Account.Manage
{
    public partial class VisibilityModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public VisibilityModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        //public string Username { get; set; }


        
        [TempData]
		public string StatusMessage { get; set; }
        
        [BindProperty]
		public InputModel Input { get; set; }

		public class InputModel
        {
            [Display(Name = "Visibile")]
            public bool Visibility { get; set; }
        }

		private async Task LoadAsync(ApplicationUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            bool isVisible = user.Visibility;

            // Username = userName;

            Input = new InputModel
            {
                Visibility = isVisible
            };
        }

		public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile caricare l'utente con ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

		public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile caricare l'utente con ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

			user.Visibility = Input.Visibility;
			var setVisibilityResult = await _userManager.UpdateAsync(user);
			if (!setVisibilityResult.Succeeded)
			{
				StatusMessage = "Errore imprevisto durante il tentativo di impostare la visibilità.";
				return RedirectToPage();
			}

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Il tuo profilo è stato aggiornato";
            return RedirectToPage();
        }

    }
}