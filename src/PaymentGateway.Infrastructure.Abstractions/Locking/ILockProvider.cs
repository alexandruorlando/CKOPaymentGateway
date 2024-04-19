namespace PaymentGateway.Infrastructure.Abstractions.Locking;

public interface ILockProvider
{
    Task<IDisposable> AcquireLockAsync(string key, CancellationToken cancellationToken);
}