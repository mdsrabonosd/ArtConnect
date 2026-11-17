using ArtConnect.Entities;

namespace ArtConnect.Facade.Models
{
    public class ArtworkDetailsModel
    {
        public Artwork Artwork { get; set; } = null!;
        public int LikeCount { get; set; }
        public bool IsLiked { get; set; }
        public bool IsWishlisted { get; set; }
        public List<Comment> Comments { get; set; } = new List<Comment>();
    }

    public class ArtistPortfolioModel
    {
        public ArtistProfile Profile { get; set; } = null!;
        public List<Artwork> Artworks { get; set; } = new List<Artwork>();
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
    }
}
