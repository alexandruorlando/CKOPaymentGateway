using PaymentGateway.Infrastructure.Abstractions.Time;

namespace PaymentGateway.TestUtilities.Mocked;

public class TestableClock : IClock
{
    private DateTime? _fixedTime;

    public DateTime Now => (_fixedTime ?? DateTime.Now).ToLocalTime();
    public DateTime UtcNow => (_fixedTime ?? DateTime.UtcNow);

    public long GetCurrentUnixTimeMilliseconds()
    {
        return (long) UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }

    public void SetFixedTime(DateTime dateTime)
    {
        _fixedTime = dateTime;
    }

    public void ResetTime()
    {
        _fixedTime = null;
    }
}