using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Abstractions;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Domain.POCOs;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;
using PaymentGateway.Infrastructure.Abstractions.Data.Repositories;
using PaymentGateway.Infrastructure.Abstractions.Sequencers;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

namespace PaymentGateway.Domain.Services;

public class PaymentService(
    ICardTokenizationService cardTokenizationService,
    IBankSimulatorService bankSimulatorService,
    IPaymentRepository paymentRepository,
    IIdGenerator idGenerator)
    : IPaymentService
{
    public async Task<Payment> ProcessPaymentAsync(PaymentProcessingData paymentData, CancellationToken cancellationToken)
    {
        try
        {
            var bankResponse = await bankSimulatorService.ProcessPaymentAsync(
                new ProcessBankPaymentRequest(paymentData.CardNumber, paymentData.CardExpiryMonth,
                    paymentData.CardExpiryYear, paymentData.CVV, paymentData.Amount, paymentData.Currency), cancellationToken);
            
            if (!bankResponse.IsSuccess)
            {
                throw new Exception($"Bank processing failed: {bankResponse.Message}");
            }

            var tokenizationResponse = await cardTokenizationService.TokenizeCardAsync(new TokenizeCardRequest(
                paymentData.CardNumber,
                paymentData.CardExpiryMonth,
                paymentData.CardExpiryYear,
                paymentData.CVV
            ));

            if (string.IsNullOrEmpty(tokenizationResponse.Token))
            {
                throw new Exception(ErrorMessage.PaymentTokenizerFailure);
            }

            // Perhaps use a service
            var paymentId = idGenerator.GenerateId();
            var paymentExternalIdentifier = ExternalIdGenerator.GenerateExternalId();

            var payment = new Payment(paymentId, paymentExternalIdentifier, tokenizationResponse.Token,
                paymentData.Amount, paymentData.Currency, bankResponse.IsSuccess ? PaymentStatus.Processed : PaymentStatus.Failed,
                bankResponse.TransactionId, bankResponse.IsSuccess ? null : ErrorMessage.PaymentBankFailure);

            await paymentRepository.CreateAsync(payment, cancellationToken);
            return payment;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"{ErrorMessage.ApplicationFailure}. Details: {ex.Message}", ex);
        }
    }

    public async Task<PaymentDetails> GetPaymentDetailsAsync(Guid externalPaymentId, CancellationToken cancellationToken)
    {
        var payment = await paymentRepository.GetByIdAsync(externalPaymentId, cancellationToken);
        if (payment == null)
        {
            throw new KeyNotFoundException($"Payment with ID {externalPaymentId} not found.");
        }
        
        var cardDetails = await cardTokenizationService.GetCardDetailsAsync(payment.CardToken);
        
        return new PaymentDetails(payment.Identifier.ToString(), cardDetails.MaskedCardNumber, cardDetails.ExpiryMonth,
            cardDetails.ExpiryYear, payment.Amount, payment.Currency, payment.Status.ToString());
    }
}