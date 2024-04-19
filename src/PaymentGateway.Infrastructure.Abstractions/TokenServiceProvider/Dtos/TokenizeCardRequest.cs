namespace PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

public record TokenizeCardRequest(
    string CardNumber,
    int CardExpiryMonth,
    int CardExpiryYear,
    int CVV
);