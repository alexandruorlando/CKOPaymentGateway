using Microsoft.Extensions.Logging;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;

namespace PaymentGateway.Infrastructure.BankSimulator;

// Mocked service
public class BankSimulatorService(ILogger<BankSimulatorService> logger) : IBankSimulatorService
{
    public async Task<ProcessBankPaymentResponse> ProcessPaymentAsync(ProcessBankPaymentRequest processBankPaymentRequest, CancellationToken cancellationToken)
    {
        try
        {
            var response = new ProcessBankPaymentResponse
            {
                IsSuccess = true,
                TransactionId = Guid.NewGuid().ToString(),
                Message =  "Transaction was successful"
            };

            return await Task.FromResult(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing payment in bank simulator.");
            throw;
        }
    }
}