using Asp.Versioning;
using PaymentGateway.Api.Exceptions.Filters;
using PaymentGateway.Api.HealthChecks;

namespace PaymentGateway.Api.IoC;

public static class DependencyContainer
{
    public static IServiceCollection RegisterApi(this IServiceCollection serviceProvider)
    {
        serviceProvider.AddControllers(options =>
        {
            options.Filters.Add<ApiExceptionHandler>();
        });
        
        serviceProvider.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        });

        serviceProvider.AddHealthChecks()
            .AddCheck<ConfigurationHealthCheck>("Configuration");
        return serviceProvider;
    }
}