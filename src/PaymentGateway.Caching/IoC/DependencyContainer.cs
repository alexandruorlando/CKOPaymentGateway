using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Caching.Cache;
using PaymentGateway.Caching.Idempotency;
using PaymentGateway.Caching.Locking;
using PaymentGateway.Infrastructure.Abstractions.Cache;
using PaymentGateway.Infrastructure.Abstractions.Idempotency;
using PaymentGateway.Infrastructure.Abstractions.Locking;

namespace PaymentGateway.Caching.IoC;

public static class DependencyContainer
{
    public static IServiceCollection RegisterCache(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddDistributedMemoryCache();
        serviceProvider.AddScoped<ICacheService, CacheService>();
        serviceProvider.AddScoped<IIdempotencyService, IdempotencyService>();
        serviceProvider.RegisterLocking();
        return serviceProvider;
    }

    private static void RegisterLocking(this IServiceCollection services)
    {
        services.AddScoped<ILockProvider, InMemoryLockProvider>();
    }
}