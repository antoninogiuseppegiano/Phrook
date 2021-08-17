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
		public string BookId { get; set; }

		// [Required]
		public string Title { get; set; }

		[Range(0, 5, ErrorMessage = "La valutazione può andare da {1} a {2}.")]
		[Display(Name = "Valutazione")]
		public double Rating { get; set; }

		[Display(Name = "Tag")]
		public string Tag { get; set; }

		[Display(Name = "Stato")]
		public string ReadingState { get; set; }
		public string RowVersion { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			int.TryParse(Tag, out int intTag);
			int.TryParse(ReadingState, out int intRS);
			if (!Enum.IsDefined(typeof(Tag), intTag))
			{
				yield return new ValidationResult("Il tag non è accetabile.", new[] { nameof(Tag) });
			}
			if (!Enum.IsDefined(typeof(ReadingState), intRS))
			{
				yield return new ValidationResult("Lo stato non è accetabile.", new[] { nameof(ReadingState) });
			}
		}
	}
}