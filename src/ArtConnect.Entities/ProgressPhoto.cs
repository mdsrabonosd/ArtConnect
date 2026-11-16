using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class ProgressPhoto : BaseEntity
    {
        public int OrderId { get; set; }

        [Required, StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(300)]
        public string? Note { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;
    }
}
