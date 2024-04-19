using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

namespace PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;

public interface ICardTokenizationService
{
    Task<TokenizeCardResponse> TokenizeCardAsync(TokenizeCardRequest tokenizeCardRequest);
    Task<CardDetailsResponse> GetCardDetailsAsync(string cardToken);
}