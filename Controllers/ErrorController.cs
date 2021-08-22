using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Phrook.Models.Exceptions;

namespace Phrook.Controllers
{
    public class ErrorController : Controller
    {
        [AllowAnonymous]
		public IActionResult Index() {
			var feature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
			
			switch (feature.Error) {
				case BookNotFoundException e:
					ViewData["Title"] = "Libro non trovato";
					Response.StatusCode = 404;
					return View("BookNotFound");

				case BookNotAddedException e:
					ViewData["Title"] = "Libro non aggiunto";
					Response.StatusCode = 404;
					return View("BookNotAdded");

				case UserNotFoundException e:
					ViewData["Title"] = "Utente non trovato";
					Response.StatusCode = 404;
					return View("UserNotFound");

				case UserUnknownException e:
					Response.StatusCode = 404;
					return Redirect("/Identity/Account/Login");
					
				default:
					ViewData["Title"] = "Errore";
					Response.StatusCode = 404;
					return View();
			}
		}
    }
}