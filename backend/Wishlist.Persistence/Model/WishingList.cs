namespace Wishlist.Persistence.Model;

public sealed class WishingList
{
    public int Id { get; set; }
    public List<WishlistItem> Items { get; set; } = new();
} 