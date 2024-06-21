using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis;
using Workspace.Application;

namespace Workspace.Infrastructure
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IConnectionMultiplexer _connectionMultiplexer;

        public ResponseCacheService(IDistributedCache cache, IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _connectionMultiplexer = connectionMultiplexer;
        }

        public async Task<string> GetCacheResponseAsync(string key)
        {
            var response = await _cache.GetStringAsync(key);

            if(string.IsNullOrEmpty(response))
            {
                return null;
            }

            return response;
        }

        public async Task SetCacheResponseAsync(string key, string value, TimeSpan timeout)
        {
            if (string.IsNullOrEmpty(value))
                return;

            await _cache.SetStringAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = timeout
            });
        }
    }
}
