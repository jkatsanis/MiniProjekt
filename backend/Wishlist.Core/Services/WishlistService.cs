using OneOf;
using OneOf.Types;
using Wishlist.Persistence.Model;
using Wishlist.Persistence.Util;

namespace Wishlist.Core.Services;

public interface IWishlistService
{
    ValueTask<WishingList> AddWishlistAsync(IReadOnlyCollection<(string Name, string? Description)> items);
    ValueTask<OneOf<WishingList, NotFound>> GetWishlistByIdAsync(int id, bool tracking);
    ValueTask<IReadOnlyCollection<WishingList>> GetAllWishlistsAsync();
    ValueTask<OneOf<Success, NotFound>> DeleteWishlistAsync(int id);

    ValueTask<WishlistItem> AddItemAsync(int wishlistId, string name, string? description);
    ValueTask<OneOf<WishlistItem, NotFound>> GetItemAsync(int wishlistId, int itemId);
    ValueTask<OneOf<Success, NotFound>> UpdateItemAsync(int wishlistId, int itemId, string name, string? description);
    ValueTask<OneOf<Success, NotFound>> DeleteItemAsync(int wishlistId, int itemId);
}

internal sealed class WishlistService(IUnitOfWork uow) : IWishlistService
{
    public async ValueTask<WishingList> AddWishlistAsync(IReadOnlyCollection<(string Name, string? Description)> items)
    {
        var wishlist = uow.WishlistRepository.AddWishlist(items);

        await uow.SaveChangesAsync();

        return wishlist;
    }

    public async ValueTask<OneOf<WishingList, NotFound>> GetWishlistByIdAsync(int id, bool tracking)
    {
        var wishlist = await uow.WishlistRepository.GetWishlistByIdAsync(id, tracking);

        return wishlist is null ? new NotFound() : wishlist;
    }

    public async ValueTask<IReadOnlyCollection<WishingList>> GetAllWishlistsAsync()
    {
        IReadOnlyCollection<WishingList> wishlists = await uow.WishlistRepository.GetAllWishlistsAsync(false);

        return wishlists;
    }

    public async ValueTask<OneOf<Success, NotFound>> DeleteWishlistAsync(int id)
    {
        var wishlist = await uow.WishlistRepository.GetWishlistByIdAsync(id, true);

        if (wishlist is null)
        {
            return new NotFound();
        }

        uow.WishlistRepository.RemoveWishlist(wishlist);

        await uow.SaveChangesAsync();

        return new Success();
    }

    public async ValueTask<WishlistItem> AddItemAsync(int wishlistId, string name, string? description)
    {
        var item = uow.WishlistRepository.AddItem(wishlistId, name, description);

        await uow.SaveChangesAsync();

        return item;
    }

    public async ValueTask<OneOf<WishlistItem, NotFound>> GetItemAsync(int wishlistId, int itemId)
    {
        var item = await uow.WishlistRepository.GetItemAsync(wishlistId, itemId, false);

        return item is null ? new NotFound() : item;
    }

    public async ValueTask<OneOf<Success, NotFound>> UpdateItemAsync(int wishlistId, int itemId, string name, string? description)
    {
        var item = await uow.WishlistRepository.GetItemAsync(wishlistId, itemId, true);

        if (item is null)
        {
            return new NotFound();
        }

        item.Name = name;
        item.Description = description;

        await uow.SaveChangesAsync();

        return new Success();
    }

    public async ValueTask<OneOf<Success, NotFound>> DeleteItemAsync(int wishlistId, int itemId)
    {
        var item = await uow.WishlistRepository.GetItemAsync(wishlistId, itemId, true);

        if (item is null)
        {
            return new NotFound();
        }

        uow.WishlistRepository.RemoveItem(item);

        await uow.SaveChangesAsync();

        return new Success();
    }
} 