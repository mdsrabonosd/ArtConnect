using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ArtConnect.Entities
{
    /// <summary>
    /// Account for Admin, Artist and Customer. The role is stored by ASP.NET Core Identity.
    /// </summary>
    public class ApplicationUser : IdentityUser<int>
    {
        [Required, StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        public bool IsBlocked { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ArtistProfile? ArtistProfile { get; set; }
    }
}
