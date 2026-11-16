using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class OrderItem : BaseEntity
    {
        public int OrderId { get; set; }
        public int ArtworkId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        [ForeignKey(nameof(OrderId))]
        public virtual Order Order { get; set; } = null!;

        [ForeignKey(nameof(ArtworkId))]
        public virtual Artwork Artwork { get; set; } = null!;
    }
}
