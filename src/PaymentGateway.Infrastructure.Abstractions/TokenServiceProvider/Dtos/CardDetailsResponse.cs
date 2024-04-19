namespace PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

public class CardDetailsResponse
{
    public string? MaskedCardNumber { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
}