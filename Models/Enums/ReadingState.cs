using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Phrook.Models.Enums
{
    public enum ReadingState
    {
		[Description("Non letto")]
		[Display(Name = "Non letto")]
		NotRead = 0,
		[Description("In lettura")]
		[Display(Name = "In lettura")]
		Reading = 1,
		[Description("Interrotto")]
		[Display(Name = "Interrotto")]
		Interrupted = 2,
		[Description("Letto")]
		[Display(Name = "Letto")]
		Read = 3
    }
}