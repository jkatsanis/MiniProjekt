namespace Wishlist.Persistence.Model;

public class WishingList
{
    public int Id { get; set; }
    public List<WishlistItem> Items { get; set; } = new();
} 