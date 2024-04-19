using PaymentGateway.Infrastructure.Abstractions.Locking;

namespace PaymentGateway.Caching.Locking;

public class InMemoryLockProvider : ILockProvider
{
    private static readonly Dictionary<string, bool> Locks = new();

    public async Task<IDisposable> AcquireLockAsync(string key, CancellationToken cancellationToken)
    {
        while (true)
        {
            lock (Locks)
            {
                if (!Locks.ContainsKey(key) || !Locks[key])
                {
                    Locks[key] = true;
                    return new LockReleaseHandle(key, ReleaseLock);
                }
            }

            cancellationToken.ThrowIfCancellationRequested();
            
            // Wait before retrying to acquire the lock
            await Task.Delay(100, cancellationToken);
        }
    }

    private void ReleaseLock(string key)
    {
        lock (Locks)
        {
            if (Locks.ContainsKey(key))
            {
                Locks[key] = false;
            }
        }
    }

    private class LockReleaseHandle(string key, Action<string> releaseLock) : IDisposable
    {
        public void Dispose()
        {
            releaseLock(key);
        }
    }
}