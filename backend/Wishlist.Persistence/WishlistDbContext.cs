using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Models;

namespace Wishlist.Persistence;

public class WishlistDbContext : DbContext
{
    public DbSet<Wishlist> Wishlists { get; set; } = null!;
    public DbSet<WishlistItem> WishlistItems { get; set; } = null!;

    public WishlistDbContext(DbContextOptions<WishlistDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wishlist>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasMany(e => e.Items)
                  .WithOne(e => e.Wishlist)
                  .HasForeignKey(e => e.WishlistId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<WishlistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
        });
    }
} 