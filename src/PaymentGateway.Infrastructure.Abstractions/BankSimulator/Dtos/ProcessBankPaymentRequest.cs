namespace PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;

public record ProcessBankPaymentRequest(
    string CardNumber,
    int CardExpiryMonth,
    int CardExpiryYear,
    int CVV,
    decimal Amount,
    string Currency
);