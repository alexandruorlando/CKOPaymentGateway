using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using PaymentGateway.Infrastructure.Abstractions.Cache;
using PaymentGateway.Infrastructure.Abstractions.Idempotency;

namespace PaymentGateway.Caching.Idempotency;

public class IdempotencyService(ICacheService cache) : IIdempotencyService
{
    private const int MaxAttempts = 3;
    private readonly JsonSerializerOptions _options = new() { PropertyNameCaseInsensitive = true };

    public async Task<T?> GetCachedResponseAsync<T>(string key, CancellationToken cancellationToken) where T : class
    {
        var attempt = 0;

        while (attempt < MaxAttempts)
        {
            try
            {
                var cachedResponse = await cache.GetStringAsync(key, cancellationToken);
                return cachedResponse == null ? null : JsonSerializer.Deserialize<T>(cachedResponse, _options);
            }
            catch (StackExchange.Redis.RedisConnectionException)
            {
                attempt++;
                if (attempt >= MaxAttempts)
                {
                    throw;
                }

                // Exponential back-off
                await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
            }
        }

        return null;
    }

    public Task CacheResponseAsync<T>(string key, T response, CancellationToken cancellationToken)
    {
        var serializedResponse = JsonSerializer.Serialize(response, _options);
        return cache.SetStringAsync(key, serializedResponse, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        }, cancellationToken);
    }
}