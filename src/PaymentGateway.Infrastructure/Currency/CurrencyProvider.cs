
using Newtonsoft.Json;
using PaymentGateway.Infrastructure.Abstractions.Currency;
using PaymentGateway.Infrastructure.Currency.Exceptions;

namespace PaymentGateway.Infrastructure.Currency;

public class CurrencyProvider : ICurrencyProvider
{
    private readonly HashSet<string> _allowedCurrencies;

    public CurrencyProvider(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var currencies = JsonConvert.DeserializeObject<List<string>>(json);

            if (currencies == null || currencies.Count == 0)
            {
                throw new CurrencyProviderException("Error fetching allowed currencies: List is empty or not available.");
            }

            _allowedCurrencies = new HashSet<string>(currencies, StringComparer.OrdinalIgnoreCase);
        }
        catch (JsonException jsonEx)
        {
            throw new CurrencyProviderException($"Error fetching allowed currencies: {jsonEx.Message}");
        }
        catch (IOException ioEx)
        {
            throw new CurrencyProviderException($"Error reading allowed currencies file: {ioEx.Message}");
        }
    }

    public bool IsCurrencyAllowed(string currency)
    {
        return _allowedCurrencies.Contains(currency);
    }
}