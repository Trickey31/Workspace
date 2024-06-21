using Microsoft.EntityFrameworkCore;

namespace Workspace.Domain
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        /// <summary>
        /// Call save change from db context
        /// </summary>
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
    {
    }
}
