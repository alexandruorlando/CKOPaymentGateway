using System.ComponentModel.DataAnnotations;
using MediatR;
using PaymentGateway.Contracts.DTOs.v1;

namespace PaymentGateway.Application.Commands
{
    public class ProcessPaymentCommand(
        string cardNumber,
        int cardExpiryMonth,
        int cardExpiryYear,
        int cvv,
        decimal amount,
        string currency)
        : IRequest<PaymentResult>
    {
        [Required(ErrorMessage = "{0} is required.")]
        [MinLength(1, ErrorMessage = "{0} is required.")]
        [CreditCard(ErrorMessage = "Invalid credit card number")]
        public string CardNumber { get; init; } = cardNumber;
        
        [Required(ErrorMessage = "{0} is required.")]
        [Range(1, 12, ErrorMessage = "CardExpiryMonth must be between {1} and {2}.")]
        public int CardExpiryMonth { get; init; } = cardExpiryMonth;
        
        [Required(ErrorMessage = "{0} is required.")]
        [Range(2024, 2040, ErrorMessage = "CardExpiryYear must be in the future.")]
        public int CardExpiryYear { get; init; } = cardExpiryYear;
        
        [Required(ErrorMessage = "{0} is required.")]
        [Range(100, 999, ErrorMessage = "CVV must be between {1} and {2}.")]
        public int Cvv { get; init; } = cvv;
        [Required(ErrorMessage = "{0} is required.")]
        public decimal Amount { get; init; } = amount;
        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(3, ErrorMessage = "{0} does not have a valid currency format.")]
        public string Currency { get; init; } = currency;
        
    }
}