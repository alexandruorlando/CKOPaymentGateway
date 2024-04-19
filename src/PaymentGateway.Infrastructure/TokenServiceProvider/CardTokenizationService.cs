using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

namespace PaymentGateway.Infrastructure.TokenServiceProvider;

// This mock service provides two core features:
// 1. Generates a unique token for a given card, simulating the tokenization process.
// 2. Retrieves masked card details using the generated token, ensuring sensitive card information is not exposed.
public class CardTokenizationService : ICardTokenizationService
{
    public Task<TokenizeCardResponse> TokenizeCardAsync(TokenizeCardRequest tokenizeCardRequest)
    {
        return Task.FromResult(new TokenizeCardResponse { Token = Guid.NewGuid().ToString() });
    }
    
    public Task<CardDetailsResponse> GetCardDetailsAsync(string token)
    {
        return Task.FromResult(new CardDetailsResponse
        {
            MaskedCardNumber = "XXXX-XXXX-XXXX-1234",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
        });
    }
}