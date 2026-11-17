using ArtConnect.DataAccess.Interface;
using ArtConnect.Entities;
using ArtConnect.Facade.Interface;
using ArtConnect.Facade.Models;
using ArtConnect.Framework;

namespace ArtConnect.Facade.Implementation
{
    public class GalleryFacade : IGalleryFacade
    {
        private readonly IArtworkRepository _artworkRepository;
        private readonly IArtistProfileRepository _profileRepository;
        private readonly ILikeRepository _likeRepository;
        private readonly IWishlistRepository _wishlistRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly ICommentFacade _commentFacade;

        public GalleryFacade(
            IArtworkRepository artworkRepository,
            IArtistProfileRepository profileRepository,
            ILikeRepository likeRepository,
            IWishlistRepository wishlistRepository,
            ICommentRepository commentRepository,
            IReviewRepository reviewRepository,
            ICommentFacade commentFacade)
        {
            _artworkRepository = artworkRepository;
            _profileRepository = profileRepository;
            _likeRepository = likeRepository;
            _wishlistRepository = wishlistRepository;
            _commentRepository = commentRepository;
            _reviewRepository = reviewRepository;
            _commentFacade = commentFacade;
        }

        public async Task<PagedResult<Artwork>> SearchAsync(ArtworkFilter filter)
        {
            var page = filter.Page < 1 ? 1 : filter.Page;
            var pageSize = filter.PageSize < 1 ? 12 : Math.Min(filter.PageSize, 48);

            var (items, total) = await _artworkRepository.SearchAsync(
                filter.Search, filter.CategoryId, filter.MinPrice, filter.MaxPrice, filter.Medium, page, pageSize);

            return new PagedResult<Artwork>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = total
            };
        }

        public async Task<List<Artwork>> GetLatestAsync(int count)
        {
            return await _artworkRepository.GetLatestAsync(count);
        }

        public async Task<List<string>> GetMediumsAsync()
        {
            return await _artworkRepository.GetMediumsAsync();
        }

        public async Task<ArtworkDetailsModel?> GetDetailsAsync(int artworkId, int? userId)
        {
            var artwork = await _artworkRepository.GetByIdWithDetailsAsync(artworkId);

            // Artworks of blocked or deleted artists are hidden from the public.
            if (artwork == null || artwork.ArtistProfile.User.IsBlocked || artwork.ArtistProfile.User.IsDeleted)
                return null;

            var model = new ArtworkDetailsModel
            {
                Artwork = artwork,
                LikeCount = await _likeRepository.CountAsync(l => l.ArtworkId == artworkId),
                Comments = await _commentRepository.GetByArtworkAsync(artworkId)
            };

            if (userId.HasValue)
            {
                model.IsLiked = await _likeRepository.AnyAsync(l => l.ArtworkId == artworkId && l.UserId == userId.Value);
                model.IsWishlisted = await _wishlistRepository.AnyAsync(w => w.ArtworkId == artworkId && w.UserId == userId.Value);
            }

            return model;
        }

        public async Task<List<ArtistProfile>> GetArtistsAsync()
        {
            var profiles = await _profileRepository.GetAllWithDetailsAsync();

            return profiles
                .Where(p => !p.User.IsBlocked && !p.User.IsDeleted)
                .OrderByDescending(p => p.IsVerified)
                .ThenBy(p => p.User.FullName)
                .ToList();
        }

        public async Task<ArtistPortfolioModel?> GetArtistPortfolioAsync(int artistProfileId)
        {
            var profile = await _profileRepository.GetByIdWithDetailsAsync(artistProfileId);
            if (profile == null || profile.User.IsBlocked || profile.User.IsDeleted)
                return null;

            var reviews = await _reviewRepository.GetByArtistAsync(artistProfileId);

            return new ArtistPortfolioModel
            {
                Profile = profile,
                Artworks = await _artworkRepository.GetByArtistAsync(artistProfileId),
                ReviewCount = reviews.Count,
                AverageRating = reviews.Count == 0 ? 0 : reviews.Average(r => r.Rating)
            };
        }

        public async Task<OperationResult> ToggleLikeAsync(int artworkId, int userId)
        {
            if (!await _artworkRepository.AnyAsync(a => a.Id == artworkId))
                return OperationResult.Fail("Artwork not found.");

            var existing = (await _likeRepository.FindAsync(l => l.ArtworkId == artworkId && l.UserId == userId)).FirstOrDefault();
            if (existing != null)
            {
                await _likeRepository.DeleteAsync(existing.Id);
                return OperationResult.Ok("Like removed.");
            }

            await _likeRepository.AddAsync(new Like { ArtworkId = artworkId, UserId = userId });
            return OperationResult.Ok("You liked this artwork.");
        }

        public async Task<OperationResult> ToggleWishlistAsync(int artworkId, int userId)
        {
            if (!await _artworkRepository.AnyAsync(a => a.Id == artworkId))
                return OperationResult.Fail("Artwork not found.");

            var existing = (await _wishlistRepository.FindAsync(w => w.ArtworkId == artworkId && w.UserId == userId)).FirstOrDefault();
            if (existing != null)
            {
                await _wishlistRepository.DeleteAsync(existing.Id);
                return OperationResult.Ok("Removed from your wishlist.");
            }

            await _wishlistRepository.AddAsync(new Wishlist { ArtworkId = artworkId, UserId = userId });
            return OperationResult.Ok("Saved to your wishlist.");
        }

        public async Task<OperationResult> AddCommentAsync(int artworkId, int userId, string text)
        {
            var result = await _commentFacade.AddCommentAsync(new Comment
            {
                ArtworkId = artworkId,
                UserId = userId,
                Text = text
            });

            return result.Success ? OperationResult.Ok("Your comment was added.") : result;
        }

        public async Task<OperationResult> DeleteCommentAsync(int commentId, int userId)
        {
            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return OperationResult.Fail("Comment not found.");

            if (comment.UserId != userId)
                return OperationResult.Fail("You can delete only your own comments.");

            await _commentRepository.DeleteAsync(commentId);
            return OperationResult.Ok("Your comment was deleted.");
        }
    }
}
