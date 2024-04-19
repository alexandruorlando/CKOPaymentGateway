namespace PaymentGateway.Infrastructure.Abstractions.Currency;

public interface ICurrencyProvider
{
    bool IsCurrencyAllowed(string currency);
}