using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.Facade.Implementation
{
    public class DashboardFacade : IDashboardFacade
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IArtistProfileRepository _profileRepository;
        private readonly IArtworkRepository _artworkRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICustomRequestRepository _requestRepository;
        private readonly IOrderRepository _orderRepository;

        public DashboardFacade(
            UserManager<ApplicationUser> userManager,
            IArtistProfileRepository profileRepository,
            IArtworkRepository artworkRepository,
            ICategoryRepository categoryRepository,
            ICustomRequestRepository requestRepository,
            IOrderRepository orderRepository)
        {
            _userManager = userManager;
            _profileRepository = profileRepository;
            _artworkRepository = artworkRepository;
            _categoryRepository = categoryRepository;
            _requestRepository = requestRepository;
            _orderRepository = orderRepository;
        }

        public async Task<DashboardSummary> GetSummaryAsync()
        {
            var customers = await _userManager.GetUsersInRoleAsync(AppRoles.Customer);
            var artists = await _userManager.GetUsersInRoleAsync(AppRoles.Artist);

            var summary = new DashboardSummary
            {
                TotalCustomers = customers.Count(u => !u.IsDeleted),
                TotalArtists = artists.Count(u => !u.IsDeleted),
                VerifiedArtists = await _profileRepository.CountAsync(p => p.IsVerified),
                TotalArtworks = await _artworkRepository.CountAsync(),
                TotalCategories = await _categoryRepository.CountAsync(),
                OpenRequests = await _requestRepository.CountAsync(r => r.Status == RequestStatus.Open),
                TotalOrders = await _orderRepository.CountAsync()
            };

            summary.RecentArtworks = (await _artworkRepository.GetAllWithDetailsAsync()).Take(5).ToList();

            summary.RecentUsers = await _userManager.Users
                .Where(u => !u.IsDeleted)
                .OrderByDescending(u => u.CreatedAt)
                .Take(5)
                .ToListAsync();

            return summary;
        }
    }
}
