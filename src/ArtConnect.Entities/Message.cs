using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ArtConnect.Entities
{
    public class Message : BaseEntity
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public int? OrderId { get; set; }

        [Required, StringLength(500)]
        public string Content { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(SenderId))]
        public virtual ApplicationUser Sender { get; set; } = null!;

        [ForeignKey(nameof(ReceiverId))]
        public virtual ApplicationUser Receiver { get; set; } = null!;

        [ForeignKey(nameof(OrderId))]
        public virtual Order? Order { get; set; }
    }
}
