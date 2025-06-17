using System.Data;
using Wishlist.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Wishlist.Persistence.Util;

public interface ITransactionProvider : IAsyncDisposable, IDisposable
{
    public ValueTask BeginTransactionAsync();
    public ValueTask CommitAsync();
    public ValueTask RollbackAsync();
}

public interface IUnitOfWork
{
    public IWishlistRepository WishlistRepository { get; }
    public Task SaveChangesAsync();
}

internal sealed class UnitOfWork(DatabaseContext context)
    : IUnitOfWork, ITransactionProvider
{
    private IDbContextTransaction? _transaction;
    
    public IWishlistRepository WishlistRepository => new WishlistRepository(context.Wishlists, context.WishlistItems);

    public async ValueTask BeginTransactionAsync()
    {
        if (_transaction is not null)
        {
            throw new TransactionException("Transaction already started, unable to start another");
        }

        _transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Snapshot);
    }

    public async ValueTask CommitAsync()
    {
        if (_transaction is null)
        {
            throw new TransactionException("No transaction started, unable to commit");
        }

        await _transaction.CommitAsync();
        _transaction = null;
    }

    public async ValueTask RollbackAsync()
    {
        if (_transaction is null)
        {
            throw new TransactionException("No transaction started, unable to rollback");
        }

        await _transaction.RollbackAsync();
        _transaction = null;
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction is null)
        {
            return;
        }

        // Transaction was neither committed nor rolled back, rolling back now - silent, this is acceptable
        await _transaction.RollbackAsync();
        await _transaction.DisposeAsync();
    }

    public void Dispose()
    {
        _transaction?.Dispose();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    private sealed class TransactionException(string message) : Exception(message);
}
