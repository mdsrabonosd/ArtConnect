using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class ArtistProposal : BaseEntity
    {
        public int RequestId { get; set; }
        public int ArtistProfileId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Price { get; set; }

        public int DeliveryDays { get; set; }

        [Required, StringLength(500)]
        public string Message { get; set; } = string.Empty;

        public int RevisionsIncluded { get; set; }

        public ProposalStatus Status { get; set; } = ProposalStatus.Pending;

        [ForeignKey(nameof(RequestId))]
        public virtual CustomRequest CustomRequest { get; set; } = null!;

        [ForeignKey(nameof(ArtistProfileId))]
        public virtual ArtistProfile ArtistProfile { get; set; } = null!;
    }
}
