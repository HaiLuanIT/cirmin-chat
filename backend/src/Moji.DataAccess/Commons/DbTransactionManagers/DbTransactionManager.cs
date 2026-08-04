using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Moji.DataAccess.Commons.Exceptions;
using Moji.DataAccess.Configurations;

namespace Moji.DataAccess.Commons.DbTransactionManagers;

public class DbTransactionManager : IDbTransactionManager
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction _transaction;

    public DbTransactionManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException exception)
        {
            throw new DataConcurrencyException("The database has been updated by another user. Please try again.",
                exception);
        }
    }

    public async Task<IAsyncDisposable> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_context.Database.CurrentTransaction != null) return _context.Database.CurrentTransaction;

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null) await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null) await _transaction.RollbackAsync(cancellationToken);
    }
}