using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class ArtistProfile : BaseEntity
    {
        [Display(Name = "Artist account")]
        public int UserId { get; set; }

        [Required, StringLength(1000)]
        public string Bio { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Skills { get; set; }

        [Range(0, 80), Display(Name = "Experience (years)")]
        public int ExperienceYears { get; set; }

        [StringLength(500), Display(Name = "Profile photo URL")]
        public string? ProfilePhotoUrl { get; set; }

        public bool IsVerified { get; set; } = false;

        public DateTime? VerifiedAt { get; set; }

        public int? VerifiedByUserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        public virtual ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
        public virtual ICollection<ArtistProposal> Proposals { get; set; } = new List<ArtistProposal>();
    }
}
