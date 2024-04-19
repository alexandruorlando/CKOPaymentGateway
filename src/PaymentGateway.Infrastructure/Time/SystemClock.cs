using PaymentGateway.Infrastructure.Abstractions.Time;

namespace PaymentGateway.Infrastructure.Time;

public class SystemClock : IClock
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public long GetCurrentUnixTimeMilliseconds()
    {
        return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}