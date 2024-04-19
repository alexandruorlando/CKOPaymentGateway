namespace PaymentGateway.Contracts.DTOs.v1;

public sealed record PaymentRequest(
    string CardNumber,
    int CardExpiryMonth,
    int CardExpiryYear,
    int CVV,
    decimal Amount,
    string Currency
);