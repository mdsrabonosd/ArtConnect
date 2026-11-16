using System.ComponentModel.DataAnnotations;

namespace ArtConnect.Entities
{
    public class Category : BaseEntity
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Artwork> Artworks { get; set; } = new List<Artwork>();
        public virtual ICollection<CustomRequest> CustomRequests { get; set; } = new List<CustomRequest>();
    }
}
