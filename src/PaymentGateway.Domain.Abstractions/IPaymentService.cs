using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.POCOs;

namespace PaymentGateway.Domain.Abstractions;

public interface IPaymentService
{
    Task<Payment> ProcessPaymentAsync(PaymentProcessingData paymentProcessingData, CancellationToken cancellationToken);
    Task<PaymentDetails> GetPaymentDetailsAsync(Guid externalPaymentId, CancellationToken cancellationToken);
}