using ArtConnect.Entities;
using ArtConnect.Facade.Models;

namespace ArtConnect.Web.UI.Models
{
    public class HomeViewModel
    {
        public List<Artwork> Latest { get; set; } = new List<Artwork>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }

    public class GalleryIndexViewModel
    {
        public PagedResult<Artwork> Result { get; set; } = new PagedResult<Artwork>();
        public ArtworkFilter Filter { get; set; } = new ArtworkFilter();
        public List<Category> Categories { get; set; } = new List<Category>();
        public List<string> Mediums { get; set; } = new List<string>();
        public string? Error { get; set; }
    }

    public class RequestDetailsViewModel
    {
        public CustomRequest Request { get; set; } = null!;
        public List<ArtistProposal> Proposals { get; set; } = new List<ArtistProposal>();
    }

    public class OpenRequestViewModel
    {
        public CustomRequest Request { get; set; } = null!;
        public ArtistProposal? MyProposal { get; set; }
        public ArtistProposal Form { get; set; } = new ArtistProposal { DeliveryDays = 7, RevisionsIncluded = 1 };
    }
}
