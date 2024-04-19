namespace PaymentGateway.Infrastructure.Abstractions.Idempotency;

public interface IIdempotencyService
{
    Task<T?> GetCachedResponseAsync<T>(string key, CancellationToken cancellationToken) where T : class;
    Task CacheResponseAsync<T>(string key, T response, CancellationToken cancellationToken);
}