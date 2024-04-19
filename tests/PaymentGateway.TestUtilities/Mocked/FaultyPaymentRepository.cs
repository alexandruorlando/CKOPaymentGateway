using PaymentGateway.Domain.Entities;
using PaymentGateway.Infrastructure.Abstractions.Data.Repositories;

namespace PaymentGateway.TestUtilities.Mocked;

public class FaultyPaymentRepository : IPaymentRepository
{
    public Task CreateAsync(Payment payment, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Unable to create the payment.");
    }

    public Task<Payment?> GetByIdAsync(Guid externalPaymentId, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Unable to retrieve the payment.");
    }
}