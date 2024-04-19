using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace PaymentGateway.Api.HealthChecks;

public class ConfigurationHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly HashSet<string> _mandatoryConfigs = ["Database:PaymentsDbName", "Database:PaymentsConnectionString"];

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        foreach (var configKey in _mandatoryConfigs)
        {
            var configValue = configuration[configKey];
            if (string.IsNullOrEmpty(configValue))
            {
                return Task.FromResult(HealthCheckResult.Unhealthy($"Missing configuration for {configKey}"));
            }
        }

        return Task.FromResult(HealthCheckResult.Healthy());
    }
}
