namespace Wishlist.Core.Services;

public interface IWishlistService
{
    ValueTask<IReadOnlyCollection<WishlistItem>> GetWishlistItemsAsync(int? itemsPerPage, int? page, string? filter);
    ValueTask<WishlistItem> AddWishlistItemAsync(string name, string? description);
    ValueTask<OneOf<WishlistItem, NotFound>> GetWishlistItemByIdAsync(int id);
    ValueTask<bool> DeleteWishlistItemByIdAsync(int id);
}

internal sealed class WishlistService(IUnitOfWork uow,
                                      ILogger<WishlistService> logger) : IWishlistService
{
    public async ValueTask<IReadOnlyCollection<WishlistItem>> GetWishlistItemsAsync(int? itemsPerPage, int? page, string? filter) =>
        await uow.WishlistRepository.GetWishlistItemsAsync(itemsPerPage, page, filter);

    public async ValueTask<WishlistItem> AddWishlistItemAsync(string name, string? description)
    {
        var item = new WishlistItem { Name = name, Description = description };
        uow.WishlistRepository.Add(item);
        await uow.SaveChangesAsync();
        logger.LogInformation("Added wishlist item {name}", name);
        return item;
    }

    public async ValueTask<OneOf<WishlistItem, NotFound>> GetWishlistItemByIdAsync(int id)
    {
        WishlistItem? item = await uow.WishlistRepository.GetByIdAsync(id);
        return item is not null ? item : new NotFound();
    }

    public async ValueTask<bool> DeleteWishlistItemByIdAsync(int id)
    {
        WishlistItem? item = await uow.WishlistRepository.GetByIdAsync(id);
        if (item is null)
            return false;
        uow.WishlistRepository.Remove(item);
        await uow.SaveChangesAsync();
        logger.LogInformation("Deleted wishlist item with id {id}", id);
        return true;
    }
}
