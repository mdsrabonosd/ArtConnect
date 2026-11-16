using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class CustomRequest : BaseEntity
    {
        public int CustomerId { get; set; }
        public int CategoryId { get; set; }

        [Required, StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required, StringLength(2000)]
        public string Description { get; set; } = string.Empty;

        [StringLength(500)]
        public string? ReferenceImageUrl { get; set; }

        [Required, StringLength(50)]
        public string CanvasSize { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BudgetMin { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal BudgetMax { get; set; }

        public DateTime Deadline { get; set; }

        public RequestStatus Status { get; set; } = RequestStatus.Open;

        [ForeignKey(nameof(CustomerId))]
        public virtual ApplicationUser Customer { get; set; } = null!;

        [ForeignKey(nameof(CategoryId))]
        public virtual Category Category { get; set; } = null!;

        public virtual ICollection<ArtistProposal> Proposals { get; set; } = new List<ArtistProposal>();
    }
}
