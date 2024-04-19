using AutoFixture;
using PaymentGateway.Contracts.DTOs.v1;

namespace PaymentGateway.TestUtilities.Builders.Customisations;

public class ValidPaymentCommandCustomisation : ICustomization
{
    public void Customize(IFixture fixture)
    {
        fixture.Customize<PaymentRequest>(composer => composer
            .With(p => p.CardNumber, "4111111111111111")
            .With(p => p.CVV, () => fixture.Create<int>() % 900 + 100)
            .With(p => p.Amount, () => Math.Round(fixture.Create<decimal>() % 9999 + 0.01m, 2))
            .With(p => p.Currency, "USD")
            .With(p => p.CardExpiryYear, () => DateTime.UtcNow.Year + fixture.Create<int>() % 10 + 1)
            .With(p => p.CardExpiryMonth, () => DateTime.UtcNow.Month + fixture.Create<int>() % (12 - DateTime.UtcNow.Month + 1))
        );
    }
}