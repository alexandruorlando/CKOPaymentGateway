namespace PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;

public class ProcessBankPaymentResponse
{
    public bool IsSuccess { get; set; }
    public string TransactionId { get; set; } = null!;
    public string Message { get; set; } = null!;
}