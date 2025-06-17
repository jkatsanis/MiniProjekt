using OneOf;
using OneOf.Types;
using Wishlist.Persistence.Model;

namespace Wishlist.Core.Services;

public interface IWishlistService
{
    ValueTask<WishingList> AddWishlistAsync(AddWishlistRequest request);
    ValueTask<OneOf<WishingList, NotFound>> GetWishlistByIdAsync(int id, bool tracking);
    ValueTask<IReadOnlyCollection<WishingList>> GetAllWishlistsAsync();
    ValueTask<OneOf<Success, NotFound>> DeleteWishlistAsync(int id);
    
    ValueTask<WishlistItem> AddItemAsync(int wishlistId, AddWishlistItemRequest request);
    ValueTask<OneOf<WishlistItem, NotFound>> GetItemAsync(int wishlistId, int itemId);
    ValueTask<OneOf<Success, NotFound>> UpdateItemAsync(int wishlistId, int itemId, AddWishlistItemRequest request);
    ValueTask<OneOf<Success, NotFound>> DeleteItemAsync(int wishlistId, int itemId);
} 