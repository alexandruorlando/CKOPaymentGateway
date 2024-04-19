using PaymentGateway.Domain.Entities;

namespace PaymentGateway.Infrastructure.Abstractions.Data.Repositories;

public interface IPaymentRepository
{
    Task CreateAsync(Payment payment, CancellationToken cancellationToken);
    Task<Payment?> GetByIdAsync(Guid externalPaymentId, CancellationToken cancellationToken);
}