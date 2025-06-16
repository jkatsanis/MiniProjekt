using Microsoft.EntityFrameworkCore;
using Wishlist.Persistence.Model;

namespace Wishlist.Persistence.Repositories;

public interface IWishlistRepository
{
    void Add(WishlistItem item);
    void Remove(WishlistItem item);
    ValueTask<WishlistItem?> GetByIdAsync(int id);
    ValueTask<IReadOnlyCollection<WishlistItem>> GetWishlistItemsAsync(int? itemsPerPage, int? page, string? filter);
}

internal sealed class WishlistRepository(DbSet<WishlistItem> items) : IWishlistRepository
{
    private IQueryable<WishlistItem> ItemsNoTracking => items.AsNoTracking();

    public void Add(WishlistItem item) => items.Add(item);
    public void Remove(WishlistItem item) => items.Remove(item);

    public async ValueTask<WishlistItem?> GetByIdAsync(int id) =>
        await ItemsNoTracking.FirstOrDefaultAsync(i => i.Id == id);

    public async ValueTask<IReadOnlyCollection<WishlistItem>> GetWishlistItemsAsync(int? itemsPerPage, int? page, string? filter)
    {
        IQueryable<WishlistItem> query = ItemsNoTracking;

        if (!string.IsNullOrWhiteSpace(filter))
            query = query.Where(i => i.Name.Contains(filter));

        if (itemsPerPage is not null && page is not null)
            query = query.Skip((page.Value - 1) * itemsPerPage.Value).Take(itemsPerPage.Value);

        return await query.ToListAsync();
    }
}