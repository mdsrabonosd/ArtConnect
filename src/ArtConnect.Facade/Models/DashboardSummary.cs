using ArtConnect.Entities;

namespace ArtConnect.Facade.Models
{
    public class DashboardSummary
    {
        public int TotalCustomers { get; set; }
        public int TotalArtists { get; set; }
        public int VerifiedArtists { get; set; }
        public int TotalArtworks { get; set; }
        public int TotalCategories { get; set; }
        public int OpenRequests { get; set; }
        public int TotalOrders { get; set; }

        public List<Artwork> RecentArtworks { get; set; } = new List<Artwork>();
        public List<ApplicationUser> RecentUsers { get; set; } = new List<ApplicationUser>();
    }
}
