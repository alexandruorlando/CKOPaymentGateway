namespace PaymentGateway.Contracts.DTOs.v1;

public sealed record PaymentDetails(
    string PaymentId,
    string? MaskedCardNumber,
    int CardExpiryMonth,
    int CardExpiryYear,
    decimal Amount,
    string Currency,
    string? Status
);