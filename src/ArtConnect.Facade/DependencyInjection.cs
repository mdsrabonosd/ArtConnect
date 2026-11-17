using ArtConnect.DataAccess;
using ArtConnect.DataAccess.Implementation;
using ArtConnect.DataAccess.Interface;
using ArtConnect.Facade.Implementation;
using ArtConnect.Facade.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArtConnect.Facade
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddArtConnectFacade(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            // Repositories and facades (one pair per module)
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<ICategoryFacade, CategoryFacade>();

            services.AddScoped<IArtistProfileRepository, ArtistProfileRepository>();
            services.AddScoped<IArtistProfileFacade, ArtistProfileFacade>();

            services.AddScoped<IArtworkRepository, ArtworkRepository>();
            services.AddScoped<IArtworkFacade, ArtworkFacade>();

            services.AddScoped<IArtworkImageRepository, ArtworkImageRepository>();
            services.AddScoped<IArtworkImageFacade, ArtworkImageFacade>();

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentFacade, CommentFacade>();

            services.AddScoped<ILikeRepository, LikeRepository>();
            services.AddScoped<ILikeFacade, LikeFacade>();

            services.AddScoped<IWishlistRepository, WishlistRepository>();
            services.AddScoped<IWishlistFacade, WishlistFacade>();

            services.AddScoped<ICustomRequestRepository, CustomRequestRepository>();
            services.AddScoped<ICustomRequestFacade, CustomRequestFacade>();

            services.AddScoped<IArtistProposalRepository, ArtistProposalRepository>();
            services.AddScoped<IArtistProposalFacade, ArtistProposalFacade>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderFacade, OrderFacade>();

            services.AddScoped<IOrderItemRepository, OrderItemRepository>();
            services.AddScoped<IOrderItemFacade, OrderItemFacade>();

            services.AddScoped<IProgressPhotoRepository, ProgressPhotoRepository>();
            services.AddScoped<IProgressPhotoFacade, ProgressPhotoFacade>();

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageFacade, MessageFacade>();

            services.AddScoped<IReviewRepository, ReviewRepository>();
            services.AddScoped<IReviewFacade, ReviewFacade>();

            // Facades that use ASP.NET Core Identity or combine several modules
            services.AddScoped<IUserFacade, UserFacade>();
            services.AddScoped<IDashboardFacade, DashboardFacade>();

            // Public site and user panels
            services.AddScoped<IGalleryFacade, GalleryFacade>();
            services.AddScoped<ICustomerFacade, CustomerFacade>();
            services.AddScoped<IArtistPanelFacade, ArtistPanelFacade>();

            return services;
        }
    }
}
