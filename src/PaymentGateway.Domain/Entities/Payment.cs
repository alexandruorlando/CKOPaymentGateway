using PaymentGateway.Domain.Enumerations;

namespace PaymentGateway.Domain.Entities;

public class Payment(
    long id,
    Guid identifier,
    string cardToken,
    decimal amount,
    string currency,
    PaymentStatus status,
    string? bankTransactionId,
    string? failureMessage = null)
{
    public long Id { get; init; } = id;
    public Guid Identifier { get; init; } = identifier;
    public string CardToken { get; init; } = cardToken;
    public decimal Amount { get; init; } = amount;
    public string Currency { get; init; } = currency;
    public PaymentStatus Status { get; init; } = status;
    public string? BankTransactionId { get; init; } = bankTransactionId;
    public string? FailureMessage { get; init; } = failureMessage;
}
