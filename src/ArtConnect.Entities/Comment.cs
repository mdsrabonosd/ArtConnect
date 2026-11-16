using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class Comment : BaseEntity
    {
        public int ArtworkId { get; set; }
        public int UserId { get; set; }

        [Required, StringLength(300)]
        public string Text { get; set; } = string.Empty;

        [ForeignKey(nameof(ArtworkId))]
        public virtual Artwork Artwork { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; } = null!;
    }
}
