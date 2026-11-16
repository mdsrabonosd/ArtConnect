using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }
        public int ArtworkId { get; set; }

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(ArtworkId))]
        public virtual Artwork Artwork { get; set; } = null!;
    }
}
