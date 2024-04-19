using PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;

namespace PaymentGateway.Infrastructure.Abstractions.BankSimulator;

public interface IBankSimulatorService
{
    Task<ProcessBankPaymentResponse> ProcessPaymentAsync(ProcessBankPaymentRequest payment, CancellationToken cancellationToken);
}