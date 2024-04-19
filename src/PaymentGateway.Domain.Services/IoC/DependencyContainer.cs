using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Domain.Abstractions;

namespace PaymentGateway.Domain.Services.IoC;

public static class DependencyContainer
{
    public static IServiceCollection RegisterDomainServices(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddScoped<IPaymentService, PaymentService>();
        return serviceProvider;
    }
}