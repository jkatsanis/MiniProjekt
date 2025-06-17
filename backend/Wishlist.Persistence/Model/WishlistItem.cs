namespace Wishlist.Persistence.Model;

public class WishlistItem
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsDone { get; set; }

    public int? WishlistId { get; set; }
    public WishingList? Wishlist { get; set; }
} 