using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Phrook.Models.Enums
{
    public enum ReadingState
    {
		[Display(Name = "Non letto")]
		NotRead = 0,
		[Display(Name = "In lettura")]
		Reading = 1,
		[Display(Name = "Interrotto")]
		Interrupted = 2,
		[Display(Name = "Letto")]
		Read = 3
    }
}