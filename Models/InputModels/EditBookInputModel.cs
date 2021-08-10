using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Phrook.Customizations.ModelBinders;
using Phrook.Models.Enums;

namespace Phrook.Models.InputModels
{
	public class EditBookInputModel : IValidatableObject
	{
		// [Required]
		public int Id { get; set; }

		// [Required]
		public string Title { get; set; }

		[Range(0.5, 5, ErrorMessage = "La valutazione puù andare da {1} a {2}.")]
		[Display(Name = "Valutazione")]
		public double Rating { get; set; }

		[Display(Name = "Tag")]
		public string Tag { get; set; }

		[Display(Name = "Stato")]
		public string ReadingState { get; set; }
		public string RowVersion { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (!Enum.IsDefined(typeof(Tag), Tag))
			{
				yield return new ValidationResult("Il tag non è accetabile.", new[] { nameof(Tag) });
			}
			if (!Enum.IsDefined(typeof(ReadingState), ReadingState))
			{
				yield return new ValidationResult("Lo stato non è accetabile.", new[] { nameof(ReadingState) });
			}
		}
	}
}