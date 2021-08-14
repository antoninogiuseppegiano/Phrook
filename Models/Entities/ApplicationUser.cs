using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Phrook.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
		public virtual ICollection<LibraryBook> Library { get; set; }
    }
}