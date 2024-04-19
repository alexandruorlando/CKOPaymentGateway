using PaymentGateway.Infrastructure.Abstractions.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;

namespace PaymentGateway.TestUtilities.Mocked;

public class FaultyBankSimulatorService : IBankSimulatorService
{
    public Task<ProcessBankPaymentResponse> ProcessPaymentAsync(ProcessBankPaymentRequest payment, CancellationToken cancellationToken)
    {
        throw new InvalidOperationException("Payment cannot be processed now.");
    }
}