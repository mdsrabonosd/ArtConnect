using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class ArtworkImage : BaseEntity
    {
        public int ArtworkId { get; set; }

        [Required, StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsMain { get; set; }

        [ForeignKey(nameof(ArtworkId))]
        public virtual Artwork Artwork { get; set; } = null!;
    }
}
