using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Application.Commands;

namespace PaymentGateway.Application.IoC;

public static class DependencyContainer
{
    public static IServiceCollection RegisterApplication(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblyContaining<ProcessPaymentCommand>();
            cfg.Lifetime = ServiceLifetime.Scoped;
        });
        return serviceProvider;
    }
}