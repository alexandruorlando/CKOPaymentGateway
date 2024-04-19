namespace PaymentGateway.Infrastructure.Abstractions.Time;

public interface IClock
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    long GetCurrentUnixTimeMilliseconds();
}