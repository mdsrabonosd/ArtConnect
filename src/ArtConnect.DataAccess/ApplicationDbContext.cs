using ArtConnect.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ArtConnect.DataAccess
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Users, Roles and UserRoles come from IdentityDbContext
        public DbSet<ArtistProfile> ArtistProfiles { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Artwork> Artworks { get; set; } = null!;
        public DbSet<ArtworkImage> ArtworkImages { get; set; } = null!;
        public DbSet<Comment> Comments { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<CustomRequest> CustomRequests { get; set; } = null!;
        public DbSet<ArtistProposal> ArtistProposals { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<ProgressPhoto> ProgressPhotos { get; set; } = null!;
        public DbSet<Message> Messages { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Users & Artist profiles (one user has zero or one artist profile)
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasOne(u => u.ArtistProfile)
                      .WithOne(a => a.User)
                      .HasForeignKey<ArtistProfile>(a => a.UserId);
            });

            modelBuilder.Entity<ArtistProfile>(entity =>
            {
                entity.HasIndex(a => a.UserId)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");
            });

            // 2. Categories
            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasIndex(c => c.Name)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");
            });

            // 3. Social tables: one like / one wishlist entry per user per artwork
            modelBuilder.Entity<Like>(entity =>
            {
                entity.HasIndex(l => new { l.ArtworkId, l.UserId })
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");
            });

            modelBuilder.Entity<Wishlist>(entity =>
            {
                entity.HasIndex(w => new { w.UserId, w.ArtworkId })
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");
            });

            // 4. Custom requests and proposals
            modelBuilder.Entity<CustomRequest>(entity =>
            {
                entity.HasOne(r => r.Customer)
                      .WithMany()
                      .HasForeignKey(r => r.CustomerId);

                entity.HasOne(r => r.Category)
                      .WithMany(c => c.CustomRequests)
                      .HasForeignKey(r => r.CategoryId);
            });

            modelBuilder.Entity<ArtistProposal>(entity =>
            {
                entity.HasOne(p => p.CustomRequest)
                      .WithMany(r => r.Proposals)
                      .HasForeignKey(p => p.RequestId);

                entity.HasOne(p => p.ArtistProfile)
                      .WithMany(a => a.Proposals)
                      .HasForeignKey(p => p.ArtistProfileId);
            });

            // 5. Orders (a proposal creates zero or one order)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasOne(o => o.Customer)
                      .WithMany()
                      .HasForeignKey(o => o.CustomerId);

                entity.HasOne(o => o.ArtistProfile)
                      .WithMany()
                      .HasForeignKey(o => o.ArtistProfileId);

                entity.HasOne(o => o.Proposal)
                      .WithOne()
                      .HasForeignKey<Order>(o => o.ProposalId);
            });

            // 6. Messages (sender and receiver are both users)
            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasOne(m => m.Sender)
                      .WithMany()
                      .HasForeignKey(m => m.SenderId);

                entity.HasOne(m => m.Receiver)
                      .WithMany()
                      .HasForeignKey(m => m.ReceiverId);
            });

            // 7. One review per order
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasIndex(r => r.OrderId)
                      .IsUnique()
                      .HasFilter("[IsDeleted] = 0");
            });

            // 8. Soft delete: deleted rows are hidden from every normal query
            modelBuilder.Entity<ArtistProfile>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Category>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Artwork>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ArtworkImage>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Comment>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Like>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Wishlist>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<CustomRequest>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ArtistProposal>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Order>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<OrderItem>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<ProgressPhoto>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Message>().HasQueryFilter(e => !e.IsDeleted);
            modelBuilder.Entity<Review>().HasQueryFilter(e => !e.IsDeleted);

            // 9. Avoid multiple cascade paths in SQL Server (we use soft delete anyway)
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                         .Where(t => typeof(BaseEntity).IsAssignableFrom(t.ClrType))
                         .SelectMany(t => t.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>()
                         .Where(e => e.State == EntityState.Modified))
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }

            return base.SaveChangesAsync(cancellationToken);
        }
    }
}
