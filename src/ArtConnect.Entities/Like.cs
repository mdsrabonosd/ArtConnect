using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class Like : BaseEntity
    {
        public int ArtworkId { get; set; }
        public int UserId { get; set; }

        [ForeignKey(nameof(ArtworkId))]
        public virtual Artwork Artwork { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
