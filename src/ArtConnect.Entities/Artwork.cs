using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class Artwork : BaseEntity
    {
        [Display(Name = "Artist")]
        public int ArtistProfileId { get; set; }

        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000)]
        public string? Description { get; set; }

        [Required, StringLength(100)]
        public string Medium { get; set; } = string.Empty;

        [StringLength(50), Display(Name = "Size (e.g. 24 x 30 in)")]
        public string? Size { get; set; }

        [Column(TypeName = "decimal(18, 2)"), Range(0.01, 100000000)]
        public decimal Price { get; set; }

        [Display(Name = "Original painting")]
        public bool IsOriginal { get; set; } = true;

        public ArtworkStatus Status { get; set; } = ArtworkStatus.Available;

        [ForeignKey(nameof(ArtistProfileId))]
        public virtual ArtistProfile ArtistProfile { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<ArtworkImage> Images { get; set; } = new List<ArtworkImage>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}
