using Microsoft.EntityFrameworkCore;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Infrastructure.Abstractions.Data.Repositories;
using PaymentGateway.Infrastructure.Database;

namespace PaymentGateway.Infrastructure.Data.Repositories;

public class PaymentRepository(PaymentDbContext context) : IPaymentRepository
{
    public async Task CreateAsync(Payment payment, CancellationToken cancellationToken)
    {
        await context.Payments.AddAsync(payment, cancellationToken);
        var result = await context.SaveChangesAsync(cancellationToken);
        if (result == 0)
        {
            throw new InvalidOperationException("Failed to save the payment to the database.");
        }
    }

    public Task<Payment?> GetByIdAsync(Guid externalPaymentId, CancellationToken cancellationToken)
    {
        return context.Payments.AsNoTracking().SingleOrDefaultAsync(p => p.Identifier == externalPaymentId, cancellationToken);
    }
}