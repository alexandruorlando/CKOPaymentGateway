using Microsoft.Extensions.Caching.Distributed;
using PaymentGateway.Infrastructure.Abstractions.Cache;

namespace PaymentGateway.Caching.Cache;

public class CacheService(IDistributedCache cache) : ICacheService
{
    public Task<string?> GetStringAsync(string key, CancellationToken token)
    {
        return cache.GetStringAsync(key, token);
    }

    public Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token)
    {
        return cache.SetStringAsync(key, value, options, token);
    }
}