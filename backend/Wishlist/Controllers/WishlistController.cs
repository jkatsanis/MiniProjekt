using FluentValidation;
using Wishlist.Core.Logic;
using Wishlist.Core.Services;
using Wishlist.Persistence.Model;
using Wishlist.Persistence.Repositories;
using Wishlist.Persistence.Util;
using Wishlist.Shared;
using Wishlist.Util;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using OneOf.Types;

namespace Wishlist.Controllers;

[Route("api/wishlist")]
public class WishlistController(IWishlistService wishlistService,
                                 ILogger<WishlistController> logger,
                                 IClock clock) : BaseController
{
    [HttpPatch("{id:int}/done")]
    [ProducesResponseType<WishlistItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<WishlistItemDto>> MarkItemDone([FromRoute] int id,
                                                                    [FromBody] MarkItemDoneRequest request,
                                                                    [FromServices] ITransactionProvider transaction)
    {
        try
        {
            await transaction.BeginTransactionAsync();
            var result = await wishlistService.MarkItemDoneAsync(id, request.Done);
            return await result.MatchAsync<ActionResult<WishlistItemDto>>(
                async item => {
                    await transaction.CommitAsync();
                    return Ok(WishlistItemDto.FromWishlistItem(item));
                },
                async _ => {
                    await transaction.RollbackAsync();
                    return NotFound();
                });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Failed to mark wishlist item {id} as done", id);
            return Problem();
        }
    }


    [HttpGet]
    [Route("")]
    [ProducesResponseType<WishlistResponse>(StatusCodes.Status200OK)]
    public async ValueTask<ActionResult<WishlistResponse>> GetWishlist([FromQuery] int? itemsPerPage,
                                                                       [FromQuery] int? page,
                                                                       [FromQuery] string? filter)
    {
        var items = await wishlistService.GetWishlistItemsAsync(itemsPerPage, page, filter);
        return Ok(new WishlistResponse(items.Select(WishlistItemDto.FromWishlistItem).ToList()));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<WishlistItemDto>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<ActionResult<WishlistItemDto>> GetWishlistItem([FromRoute] int id)
    {
        OneOf<WishlistItem, NotFound> result = await wishlistService.GetWishlistItemByIdAsync(id);
        return result.Match<ActionResult<WishlistItemDto>>(
            item => Ok(WishlistItemDto.FromWishlistItem(item)),
            _ => NotFound());
    }

    [HttpPost]
    [Route("")]
    [ProducesResponseType<WishlistItemDto>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async ValueTask<ActionResult<WishlistItemDto>> AddWishlistItem([FromBody] AddWishlistItemRequest request,
                                                                          [FromServices] ITransactionProvider transaction)
    {
        var validator = new AddWishlistItemRequest.Validator();
        if (!ValidateRequest(request, validator, out string[]? errors))
        {
            logger.LogInformation("Invalid wishlist item request");
            return BadRequest(errors);
        }

        try
        {
            await transaction.BeginTransactionAsync();

            WishlistItem item = await wishlistService.AddWishlistItemAsync(request.Name, request.Description);

            await transaction.CommitAsync();
            return CreatedAtAction(nameof(GetWishlistItem), new { id = item.Id }, WishlistItemDto.FromWishlistItem(item));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            logger.LogError(ex, "Error while adding wishlist item");
            return Problem();
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async ValueTask<IActionResult> DeleteWishlistItem([FromRoute] int id,
                                                             [FromServices] ITransactionProvider transaction)
    {
        try
        {
            await transaction.BeginTransactionAsync();
            bool deleted = await wishlistService.DeleteWishlistItemByIdAsync(id);
            if (!deleted)
            {
                await transaction.RollbackAsync();
                return NotFound();
            }
            await transaction.CommitAsync();
            return NoContent();
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError(e, "Error deleting wishlist item with ID {id}", id);
            return Problem();
        }
    }
}

public sealed record WishlistResponse(List<WishlistItemDto> Items);

public sealed record WishlistItemDto(int Id, string Name, string? Description, bool IsDone)
{
    public static WishlistItemDto FromWishlistItem(WishlistItem item) =>
        new(item.Id, item.Name, item.Description, item.IsDone);
}


public sealed record AddWishlistItemRequest(string Name, string? Description)
{
    public class Validator : AbstractValidator<AddWishlistItemRequest>
    {
        public Validator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
            RuleFor(x => x.Description).MaximumLength(500);
        }
    }
}
