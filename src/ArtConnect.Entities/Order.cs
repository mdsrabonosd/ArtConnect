using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    /// <summary>
    /// Purchase order (existing painting) or commission order (accepted proposal).
    /// Table name is "Orders" because ORDER is a reserved word in SQL Server.
    /// </summary>
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public int ArtistProfileId { get; set; }
        public int? ProposalId { get; set; }

        public OrderType OrderType { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal CommissionAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(CustomerId))]
        public virtual ApplicationUser Customer { get; set; } = null!;

        [ForeignKey(nameof(ArtistProfileId))]
        public virtual ArtistProfile ArtistProfile { get; set; } = null!;

        [ForeignKey(nameof(ProposalId))]
        public virtual ArtistProposal? Proposal { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
        public virtual ICollection<ProgressPhoto> ProgressPhotos { get; set; } = new List<ProgressPhoto>();
    }
}
