namespace PaymentGateway.Infrastructure.Currency.Exceptions;

public class CurrencyProviderException(string message) : Exception(message);