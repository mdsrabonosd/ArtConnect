using ArtConnect.Entities;

namespace ArtConnect.Facade.Models
{
    public class CustomerDashboard
    {
        public int OpenRequests { get; set; }
        public int PendingProposals { get; set; }
        public int ActiveOrders { get; set; }
        public int WishlistCount { get; set; }
        public List<CustomRequest> RecentRequests { get; set; } = new List<CustomRequest>();
    }

    public class ArtistDashboard
    {
        public ArtistProfile Profile { get; set; } = null!;
        public int TotalArtworks { get; set; }
        public int SoldArtworks { get; set; }
        public int PendingProposals { get; set; }
        public int ActiveOrders { get; set; }
        public int ReviewCount { get; set; }
        public double AverageRating { get; set; }
        public List<Review> LatestReviews { get; set; } = new List<Review>();
    }
}
