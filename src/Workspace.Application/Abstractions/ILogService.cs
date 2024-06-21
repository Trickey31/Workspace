using Workspace.Contract;

namespace Workspace.Application
{
    public interface ILogService
    {
        Task<Result> CreateLog(string functionType, string functionName, string application, string? beforeValue, string? afterValue, Guid objId);
    }
}
