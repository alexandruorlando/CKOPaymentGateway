namespace PaymentGateway.Domain.POCOs;

public record PaymentProcessingData(
    decimal Amount,
    string Currency,
    string CardNumber,
    int CardExpiryMonth,
    int CardExpiryYear,
    int CVV
);