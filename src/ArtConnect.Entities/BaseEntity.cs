using System.ComponentModel.DataAnnotations;

namespace ArtConnect.Entities
{
    /// <summary>
    /// Common fields for every table (except Identity users).
    /// IsDeleted is used for soft delete.
    /// </summary>
    public abstract class BaseEntity
    {
        [Key]
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
