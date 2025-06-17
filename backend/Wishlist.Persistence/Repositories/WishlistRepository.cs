using Microsoft.EntityFrameworkCore;
using Wishlist.Persistence.Model;

namespace Wishlist.Persistence.Repositories;

public interface IWishlistRepository
{
    public WishingList AddWishlist(IReadOnlyCollection<(string Name, string? Description)> items);
    public ValueTask<IReadOnlyCollection<WishingList>> GetAllWishlistsAsync(bool tracking);
    public ValueTask<WishingList?> GetWishlistByIdAsync(int id, bool tracking);
    public void RemoveWishlist(WishingList wishlist);
    
    public WishlistItem AddItem(int wishlistId, string name, string? description);
    public ValueTask<WishlistItem?> GetItemAsync(int wishlistId, int itemId, bool tracking);
    public void RemoveItem(WishlistItem item);
}

internal sealed class WishlistRepository(DbSet<WishingList> wishlistSet, DbSet<WishlistItem> itemSet) : IWishlistRepository
{
    private IQueryable<WishingList> Wishlists => wishlistSet;
    private IQueryable<WishingList> WishlistsNoTracking => Wishlists.AsNoTracking();
    private IQueryable<WishlistItem> Items => itemSet;
    private IQueryable<WishlistItem> ItemsNoTracking => Items.AsNoTracking();

    public WishingList AddWishlist(IReadOnlyCollection<(string Name, string? Description)> items)
    {
        var wishlist = new WishingList
        {
            Items = items.Select(i => new WishlistItem
            {
                Name = i.Name,
                Description = i.Description
            }).ToList()
        };

        wishlistSet.Add(wishlist);
        return wishlist;
    }

    public async ValueTask<IReadOnlyCollection<WishingList>> GetAllWishlistsAsync(bool tracking)
    {
        IQueryable<WishingList> source = tracking ? Wishlists : WishlistsNoTracking;
        source = source.Include(w => w.Items);

        List<WishingList> wishlists = await source.ToListAsync();
        return wishlists;
    }

    public async ValueTask<WishingList?> GetWishlistByIdAsync(int id, bool tracking)
    {
        IQueryable<WishingList> source = tracking ? Wishlists : WishlistsNoTracking;
        source = source.Include(w => w.Items);

        var wishlist = await source.FirstOrDefaultAsync(w => w.Id == id);
        return wishlist;
    }

    public void RemoveWishlist(WishingList wishlist)
    {
        wishlistSet.Remove(wishlist);
    }

    public WishlistItem AddItem(int wishlistId, string name, string? description)
    {
        var item = new WishlistItem
        {
            Name = name,
            Description = description,
            WishlistId = wishlistId
        };

        itemSet.Add(item);
        return item;
    }

    public async ValueTask<WishlistItem?> GetItemAsync(int wishlistId, int itemId, bool tracking)
    {
        IQueryable<WishlistItem> source = tracking ? Items : ItemsNoTracking;

        var item = await source.FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId);
        return item;
    }

    public void RemoveItem(WishlistItem item)
    {
        itemSet.Remove(item);
    }
} 