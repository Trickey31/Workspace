namespace Workspace.Application
{
    public interface IResponseCacheService
    {
        Task SetCacheResponseAsync(string key, string value, TimeSpan timeout);
        Task<string> GetCacheResponseAsync(string key);
    }
}
