namespace PaymentGateway.Domain.Services;

internal static class ExternalIdGenerator
{
    internal static Guid GenerateExternalId()
    {
        return Guid.NewGuid();
    }
}