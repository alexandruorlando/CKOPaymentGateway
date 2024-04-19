using System.ComponentModel.DataAnnotations;
using MediatR;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Domain.Abstractions;
using PaymentGateway.Domain.POCOs;
using PaymentGateway.Infrastructure.Abstractions.Time;

namespace PaymentGateway.Application.Commands.Handlers;

public class ProcessPaymentHandler(IPaymentService paymentService, IClock clock)
    : IRequestHandler<ProcessPaymentCommand, PaymentResult>
{
    public async Task<PaymentResult> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(request);
        if (!Validator.TryValidateObject(request, context, validationResults, true))
        {
            throw new ValidationException("Payment request validation failed: " + string.Join("; ", validationResults));
        }

        if (IsCardExpired(request.CardExpiryYear, request.CardExpiryMonth))
        {
            throw new ValidationException("Card is expired.");
        }
        
        if (request.Amount is < PaymentConstants.MinAmount or > PaymentConstants.MaxAmount)
        {
            throw new ValidationException($"Amount {request.Amount} is not in the allowed range: [${PaymentConstants.MinAmount} and {PaymentConstants.MaxAmount}].");
        }

        try
        {
            var paymentProcessingData = new PaymentProcessingData(
                request.Amount,
                request.Currency,
                request.CardNumber,
                request.CardExpiryMonth,
                request.CardExpiryYear,
                request.Cvv
            );

            var paymentResult = await paymentService.ProcessPaymentAsync(paymentProcessingData, cancellationToken);
            return new PaymentResult(true, "Payment processed successfully.", paymentResult.Identifier.ToString());
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message, ex);
        }
    }

    private bool IsCardExpired(int expiryYear, int expiryMonth)
    {
        var currentYear = clock.UtcNow.Year;
        var currentMonth = clock.UtcNow.Month;
        return expiryYear < currentYear || (expiryYear == currentYear && expiryMonth < currentMonth);
    }
}