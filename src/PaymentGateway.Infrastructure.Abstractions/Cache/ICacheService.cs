using Microsoft.Extensions.Caching.Distributed;

namespace PaymentGateway.Infrastructure.Abstractions.Cache;

public interface ICacheService
{
    Task<string?> GetStringAsync(string key, CancellationToken token);
    Task SetStringAsync(string key, string value, DistributedCacheEntryOptions options, CancellationToken token);
}