using ArtConnect.Entities;

namespace ArtConnect.Facade.Models
{
    public class UserListItem
    {
        public ApplicationUser User { get; set; } = null!;
        public string Role { get; set; } = string.Empty;
    }
}
