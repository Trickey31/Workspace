using Workspace.Domain;
using Task = System.Threading.Tasks.Task;

namespace Workspace.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
            => _context = context;

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
            => await _context.SaveChangesAsync();

        async ValueTask IAsyncDisposable.DisposeAsync()
            => await _context.DisposeAsync();
    }
}
