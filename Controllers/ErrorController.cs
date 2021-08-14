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
			
			//TODO: spostare in servizio applicativo ErrorViewSelector
			switch (feature.Error) {
				case BookNotFoundException e:
					ViewData["Title"] = "Libro non trovato";
					Response.StatusCode = 404;
					return View("BookNotFound");

				default:
					ViewData["Title"] = "Errore";
					Response.StatusCode = 404;
					return View();
			}

			
			
		}
    }
}