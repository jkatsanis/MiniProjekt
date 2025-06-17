using Microsoft.EntityFrameworkCore;
using Wishlist.Core.Models;
using Wishlist.Persistence;

namespace Wishlist.Core.Services;

public class WishlistService : IWishlistService
{
    private readonly WishlistDbContext _context;

    public WishlistService(WishlistDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Wishlist>> GetAllWishlistsAsync()
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .ToListAsync();
    }

    public async Task<Wishlist?> GetWishlistByIdAsync(int id)
    {
        return await _context.Wishlists
            .Include(w => w.Items)
            .FirstOrDefaultAsync(w => w.Id == id);
    }

    public async Task<Wishlist> CreateWishlistAsync(Wishlist wishlist)
    {
        _context.Wishlists.Add(wishlist);
        await _context.SaveChangesAsync();
        return wishlist;
    }

    public async Task<bool> UpdateWishlistAsync(Wishlist wishlist)
    {
        var existingWishlist = await _context.Wishlists.FindAsync(wishlist.Id);
        if (existingWishlist == null)
        {
            return false;
        }

        _context.Entry(existingWishlist).CurrentValues.SetValues(wishlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteWishlistAsync(int id)
    {
        var wishlist = await _context.Wishlists.FindAsync(id);
        if (wishlist == null)
        {
            return false;
        }

        _context.Wishlists.Remove(wishlist);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<WishlistItem> AddItemAsync(WishlistItem item)
    {
        _context.WishlistItems.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task<WishlistItem?> GetItemAsync(int wishlistId, int itemId)
    {
        return await _context.WishlistItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId);
    }

    public async Task<bool> UpdateItemAsync(WishlistItem item)
    {
        var existingItem = await _context.WishlistItems.FindAsync(item.Id);
        if (existingItem == null || existingItem.WishlistId != item.WishlistId)
        {
            return false;
        }

        _context.Entry(existingItem).CurrentValues.SetValues(item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteItemAsync(int wishlistId, int itemId)
    {
        var item = await _context.WishlistItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.WishlistId == wishlistId);
        
        if (item == null)
        {
            return false;
        }

        _context.WishlistItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }
} 