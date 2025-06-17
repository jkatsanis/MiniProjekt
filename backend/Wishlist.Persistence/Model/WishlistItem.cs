namespace Wishlist.Persistence.Model;

public sealed class WishlistItem
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public bool IsDone { get; set; }

    public int? WishlistId { get; set; }
    public WishingList? Wishlist { get; set; }
} 