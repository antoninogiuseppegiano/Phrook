using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Phrook.Models.Entities;

namespace Phrook.Areas.Identity.Pages.Account.Manage
{
    public class Disable2faModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<Disable2faModel> _logger;

        public Disable2faModel(
            UserManager<ApplicationUser> userManager,
            ILogger<Disable2faModel> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        [TempData]
        public string StatusMessage { get; set; }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile caricare l'utente con ID '{_userManager.GetUserId(User)}'.");/* Unable to load user with ID  */
            }

            if (!await _userManager.GetTwoFactorEnabledAsync(user))
            {
                throw new InvalidOperationException($"Impossibile disabilitare 2FA per l'utente con ID '{_userManager.GetUserId(User)}' perché non era abilitata.");/* Cannot disable 2FA for user with ID  */
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Impossibile caricare l'utente con ID '{_userManager.GetUserId(User)}'.");/* Unable to load user with ID  */
            }

            var disable2faResult = await _userManager.SetTwoFactorEnabledAsync(user, false);
            if (!disable2faResult.Succeeded)
            {
                throw new InvalidOperationException($"Si è verificato un errore imprevisto durante la disattivazione della 2FA per l'utente con ID '{_userManager.GetUserId(User)}'.");/* Unexpected error occurred disabling 2FA for user with ID  */
            }

            _logger.LogInformation("User with ID '{UserId}' has disabled 2fa.", _userManager.GetUserId(User));
            StatusMessage = "L'autenticazione a due fattori è stato disabilitato. La potrai abilitare nuovamente configurando un'app di autenticazione";/* 2fa has been disabled. You can reenable 2fa when you setup an authenticator app */
            return RedirectToPage("./TwoFactorAuthentication");
        }
    }
}