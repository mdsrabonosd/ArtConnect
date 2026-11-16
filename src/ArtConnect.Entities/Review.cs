using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class Review : BaseEntity
    {
        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public int ArtistProfileId { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey(nameof(CustomerId))]
        public virtual ApplicationUser Customer { get; set; } = null!;

        [ForeignKey(nameof(ArtistProfileId))]
        public virtual ArtistProfile ArtistProfile { get; set; } = null!;
    }
}
