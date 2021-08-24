using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Phrook.Models.Entities;

namespace Phrook.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
			[Required(ErrorMessage = "L'email è obbligatoria")]
            [EmailAddress(ErrorMessage = "Deve essere un indirizzo email valido")]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Ti è stato inviato un link di conferma via email. Per favore, controlla la tua email.");/* Verification email sent. Please check your email. */
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
			try
			{
				await _emailSender.SendEmailAsync(
                Input.Email,
                "Conferma la tua email",/* Confirm your email */
                // $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                $"Per favore, conferma il tuo account <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>cliccando qui</a>.");
			}
			catch
			{
				ModelState.AddModelError(string.Empty, "Non è stato possibile inviare l'email. Riprova più tardi.");
            	return Page();
			}

            ModelState.AddModelError(string.Empty, "Ti è stato inviato un link di conferma via email. Per favore, controlla la tua email.");/* Verification email sent. Please check your email. */
            return Page();
        }
    }
}
