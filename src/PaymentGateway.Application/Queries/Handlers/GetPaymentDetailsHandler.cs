using System.ComponentModel.DataAnnotations;
using MediatR;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Domain.Abstractions;

namespace PaymentGateway.Application.Queries.Handlers;

public class GetPaymentDetailsHandler(
    IPaymentService paymentService)
    : IRequestHandler<GetPaymentDetailsQuery, PaymentDetails>
{
    public async Task<PaymentDetails> Handle(GetPaymentDetailsQuery request, CancellationToken cancellationToken)
    {
        if (!Guid.TryParse(request.PaymentId, out var externalPaymentId))
        {
            throw new ValidationException("Payment identifier is invalid.");
        }

        try
        {
            var paymentResult = await paymentService.GetPaymentDetailsAsync(externalPaymentId, cancellationToken);
            return new PaymentDetails(
                paymentResult.PaymentId,
                paymentResult.MaskedCardNumber,
                paymentResult.CardExpiryMonth,
                paymentResult.CardExpiryYear,
                paymentResult.Amount,
                paymentResult.Currency,
                paymentResult.Status);
        }
        catch (KeyNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new ApplicationException(ex.Message, ex);
        }
    }
}