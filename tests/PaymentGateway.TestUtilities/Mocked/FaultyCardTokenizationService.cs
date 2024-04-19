using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

namespace PaymentGateway.TestUtilities.Mocked;

public class FaultyCardTokenizationService : ICardTokenizationService
{
    public Task<TokenizeCardResponse> TokenizeCardAsync(TokenizeCardRequest tokenizeCardRequest)
    {
        throw new InvalidOperationException("Card cannot be tokenized.");
    }

    public Task<CardDetailsResponse> GetCardDetailsAsync(string cardToken)
    {
        throw new InvalidOperationException("Unable to get card details.");
    }
}