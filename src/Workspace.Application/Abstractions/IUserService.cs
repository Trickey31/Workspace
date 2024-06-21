using Workspace.Contract;
using Workspace.Domain;

namespace Workspace.Application
{
    public interface IUserService
    {
        Task<TResult<User>> GetCurrentUserAsync();
    }
}
