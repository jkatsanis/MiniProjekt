using Wishlist.Persistence.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace Wishlist.Persistence.Util;

public sealed class DatabaseContext(DbContextOptions<DatabaseContext> options) : DbContext(options)
{
    public const string SchemaName = "Wishlist";
    
    public DbSet<WishingList> Wishlists { get; set; }
    public DbSet<WishlistItem> WishlistItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaName);

        ConfigureWishlist(modelBuilder);
        ConfigureWishlistItem(modelBuilder);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);

        configurationBuilder.Conventions.Remove<TableNameFromDbSetConvention>();
    }

    private static void ConfigureWishlist(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<WishingList> wishlist = modelBuilder.Entity<WishingList>();
        wishlist.HasKey(w => w.Id);
        wishlist.Property(w => w.Id).ValueGeneratedOnAdd();
        wishlist.HasMany(w => w.Items)
                .WithOne(i => i.Wishlist)
                .HasForeignKey(i => i.WishlistId)
                .OnDelete(DeleteBehavior.Cascade);
    }

    private static void ConfigureWishlistItem(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<WishlistItem> item = modelBuilder.Entity<WishlistItem>();
        item.HasKey(i => i.Id);
        item.Property(i => i.Id).ValueGeneratedOnAdd();
        item.Property(i => i.Name).IsRequired();
    }
}
