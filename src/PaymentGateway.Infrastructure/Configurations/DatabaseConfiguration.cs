namespace PaymentGateway.Infrastructure.Configurations;

public class DatabaseConfiguration
{
    public string? PaymentsDbName { get; set; }
    // For simplicity we'll use a config, but we can use a secure secrets management tool
    public string? PaymentsConnectionString { get; set; }
}