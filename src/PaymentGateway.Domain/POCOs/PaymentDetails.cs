namespace PaymentGateway.Domain.POCOs;

public record PaymentDetails(
    string PaymentId,
    string? MaskedCardNumber,
    int CardExpiryMonth,
    int CardExpiryYear,
    decimal Amount,
    string Currency,
    string? Status
);