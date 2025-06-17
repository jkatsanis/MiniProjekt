using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;
using Wishlist.Persistence.Model;
using Wishlist.Core.Services;
using Wishlist.Persistence.Util;
using Wishlist.Util;

namespace Wishlist.Controllers;

[Route("api/wishlists")]
public sealed class WishlistController(
    ITransactionProvider transaction,
    IWishlistService wishlistService,
    ILogger<WishlistController> logger) : BaseController
{
    [HttpGet]
    [Route("{id:int}")]
    [ProducesResponseType<WishlistDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<WishlistDto>> GetWishlistById([FromRoute] int id)
    {
        if (id < 0)
        {
            return BadRequest();
        }

        OneOf<WishingList, NotFound> wishlistResult = await wishlistService.GetWishlistByIdAsync(id, false);

        return wishlistResult.Match<ActionResult<WishlistDto>>(
            wishlist => Ok(WishlistDto.FromWishlist(wishlist)),
            notFound => NotFound());
    }

    [HttpGet]
    [Route("")]
    [ProducesResponseType<AllWishlistsResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<AllWishlistsResponse>> GetWishlists()
    {
        var wishlists = await wishlistService.GetAllWishlistsAsync();

        List<WishlistDto> dtos = wishlists.Select(WishlistDto.FromWishlist).ToList();

        return Ok(new AllWishlistsResponse(dtos));
    }

    [HttpPost]
    [ProducesResponseType<WishlistDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WishlistDto>> AddWishlist([FromBody] AddWishlistRequest request)
    {
        if (!ValidateRequest<AddWishlistRequest.Validator, AddWishlistRequest>(request))
        {
            return BadRequest();
        }

        try
        {
            await transaction.BeginTransactionAsync();

            var items = request.Items.Select(i => (i.Name, i.Description)).ToList();
            var wishlist = await wishlistService.AddWishlistAsync(items);
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetWishlistById), 
                new { id = wishlist.Id }, 
                WishlistDto.FromWishlist(wishlist));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Error adding wishlist");
            return Problem();
        }
    }

    [HttpPost]
    [Route("{wishlistId:int}/items")]
    [ProducesResponseType<WishlistItemDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<WishlistItemDto>> AddItem(
        [FromRoute] int wishlistId,
        [FromBody] AddWishlistItemRequest request)
    {
        if (wishlistId < 0 || !ValidateRequest<AddWishlistItemRequest.Validator, AddWishlistItemRequest>(request))
        {
            return BadRequest();
        }

        try
        {
            await transaction.BeginTransactionAsync();

            var item = await wishlistService.AddItemAsync(wishlistId, request.Name, request.Description);
            await transaction.CommitAsync();

            return CreatedAtAction(nameof(GetItem),
                new { wishlistId, itemId = item.Id },
                WishlistItemDto.FromItem(item));
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Error adding item to wishlist {WishlistId}", wishlistId);
            return Problem();
        }
    }

    [HttpGet]
    [Route("{wishlistId:int}/items/{itemId:int}")]
    [ProducesResponseType<WishlistItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<WishlistItemDto>> GetItem(
        [FromRoute] int wishlistId,
        [FromRoute] int itemId)
    {
        if (wishlistId < 0 || itemId < 0)
        {
            return BadRequest();
        }

        OneOf<WishlistItem, NotFound> itemResult = await wishlistService.GetItemAsync(wishlistId, itemId);

        return itemResult.Match<ActionResult<WishlistItemDto>>(
            item => Ok(WishlistItemDto.FromItem(item)),
            notFound => NotFound());
    }

    [HttpPut]
    [Route("{wishlistId:int}/items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> UpdateItem(
        [FromRoute] int wishlistId,
        [FromRoute] int itemId,
        [FromBody] AddWishlistItemRequest request)
    {
        if (wishlistId < 0 || itemId < 0 || !ValidateRequest<AddWishlistItemRequest.Validator, AddWishlistItemRequest>(request))
        {
            return BadRequest();
        }

        try
        {
            await transaction.BeginTransactionAsync();

            var result = await wishlistService.UpdateItemAsync(wishlistId, itemId, request.Name, request.Description);
            if (result.IsT0)
            {
                await transaction.CommitAsync();
            }

            return result.Match<IActionResult>(
                success => NoContent(),
                notFound => NotFound());
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Error updating item {ItemId} in wishlist {WishlistId}", itemId, wishlistId);
            return Problem();
        }
    }

    [HttpDelete]
    [Route("{wishlistId:int}/items/{itemId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteItem(
        [FromRoute] int wishlistId,
        [FromRoute] int itemId)
    {
        if (wishlistId < 0 || itemId < 0)
        {
            return BadRequest();
        }

        try
        {
            var result = await wishlistService.DeleteItemAsync(wishlistId, itemId);

            return result.Match<IActionResult>(
                success => NoContent(),
                notFound => NotFound());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting item {ItemId} from wishlist {WishlistId}", itemId, wishlistId);
            return Problem();
        }
    }

    [HttpDelete]
    [Route("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteWishlist([FromRoute] int id)
    {
        if (id < 0)
        {
            return BadRequest();
        }

        try
        {
            var result = await wishlistService.DeleteWishlistAsync(id);

            return result.Match<IActionResult>(
                success => NoContent(),
                notFound => NotFound());
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting wishlist {WishlistId}", id);
            return Problem();
        }
    }
}


public sealed record WishlistDto(
    int Id,
    IReadOnlyCollection<WishlistItemDto> Items)
{
    public static WishlistDto FromWishlist(WishingList wishlist)
    {
        var items = wishlist.Items.Select(WishlistItemDto.FromItem).ToList();
        return new WishlistDto(wishlist.Id, items);
    }
}

public sealed record WishlistItemDto(
    int Id,
    string Name,
    string? Description,
    bool IsDone,
    int? WishlistId)
{
    public static WishlistItemDto FromItem(WishlistItem item)
    {
        return new WishlistItemDto(
            item.Id,
            item.Name,
            item.Description,
            item.IsDone,
            item.WishlistId);
    }
}

public sealed class AddWishlistRequest
{
    public required IReadOnlyCollection<AddWishlistItemRequest> Items { get; set; }

    public sealed class Validator : AbstractValidator<AddWishlistRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Items).NotEmpty().WithMessage("Wishlist must have at least one item.");
            RuleForEach(x => x.Items).SetValidator(new AddWishlistItemRequest.Validator());
        }
    }
}

public sealed class AddWishlistItemRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }

    public sealed class Validator : AbstractValidator<AddWishlistItemRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }
}

public sealed record AllWishlistsResponse(IReadOnlyCollection<WishlistDto> Wishlists); 