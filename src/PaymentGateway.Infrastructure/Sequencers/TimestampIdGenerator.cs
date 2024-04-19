using PaymentGateway.Infrastructure.Abstractions.Sequencers;
using PaymentGateway.Infrastructure.Abstractions.Time;

namespace PaymentGateway.Infrastructure.Sequencers;

public class TimestampIdGenerator(IClock clock) : IIdGenerator
{
    private long _lastTimestamp = -1;
    private int _sequence = 0;
    private readonly object _lock = new();

    public long GenerateId()
    {
        lock (_lock)
        {
            var now = clock.UtcNow.Ticks;
            if (now == _lastTimestamp)
            {
                _sequence++;
            }
            else
            {
                _sequence = 0;
                _lastTimestamp = now;
            }

            // Shift the timestamp to the left to leave space for the sequence
            var id = now << 20; // Leaves more space for the sequence
            id |= (uint)(_sequence & 0xFFFFF); // Increase the sequence mask to allow more unique IDs per tick

            return id;
        }
    }
}