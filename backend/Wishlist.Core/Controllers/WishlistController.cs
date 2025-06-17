using Microsoft.AspNetCore.Mvc;
using Wishlist.Core.Models;

namespace Wishlist.Core.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Wishlist>>> GetAllWishlists()
    {
        var wishlists = await _wishlistService.GetAllWishlistsAsync();
        return Ok(wishlists);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Wishlist>> GetWishlist(int id)
    {
        var wishlist = await _wishlistService.GetWishlistByIdAsync(id);
        if (wishlist == null)
        {
            return NotFound();
        }
        return Ok(wishlist);
    }

    [HttpPost]
    public async Task<ActionResult<Wishlist>> CreateWishlist(Wishlist wishlist)
    {
        var createdWishlist = await _wishlistService.CreateWishlistAsync(wishlist);
        return CreatedAtAction(nameof(GetWishlist), new { id = createdWishlist.Id }, createdWishlist);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateWishlist(int id, Wishlist wishlist)
    {
        if (id != wishlist.Id)
        {
            return BadRequest();
        }

        var success = await _wishlistService.UpdateWishlistAsync(wishlist);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWishlist(int id)
    {
        var success = await _wishlistService.DeleteWishlistAsync(id);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpPost("{wishlistId}/items")]
    public async Task<ActionResult<WishlistItem>> AddItem(int wishlistId, WishlistItem item)
    {
        item.WishlistId = wishlistId;
        var createdItem = await _wishlistService.AddItemAsync(item);
        return CreatedAtAction(nameof(GetItem), new { wishlistId, itemId = createdItem.Id }, createdItem);
    }

    [HttpGet("{wishlistId}/items/{itemId}")]
    public async Task<ActionResult<WishlistItem>> GetItem(int wishlistId, int itemId)
    {
        var item = await _wishlistService.GetItemAsync(wishlistId, itemId);
        if (item == null)
        {
            return NotFound();
        }
        return Ok(item);
    }

    [HttpPut("{wishlistId}/items/{itemId}")]
    public async Task<IActionResult> UpdateItem(int wishlistId, int itemId, WishlistItem item)
    {
        if (itemId != item.Id || wishlistId != item.WishlistId)
        {
            return BadRequest();
        }

        var success = await _wishlistService.UpdateItemAsync(item);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{wishlistId}/items/{itemId}")]
    public async Task<IActionResult> DeleteItem(int wishlistId, int itemId)
    {
        var success = await _wishlistService.DeleteItemAsync(wishlistId, itemId);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
} 