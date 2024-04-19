using Moq;
using AutoFixture;
using PaymentGateway.Domain.Abstractions;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.Enumerations;
using PaymentGateway.Application.Commands.Handlers;
using PaymentGateway.Application.Commands;
using System.ComponentModel.DataAnnotations;
using PaymentGateway.Domain.POCOs;
using PaymentGateway.TestUtilities.Mocked;

namespace PaymentGateway.Application.Unit.Tests.Commands.Handlers;

[TestFixture]
public class ProcessPaymentHandlerTests
{
    private Mock<IPaymentService> _paymentServiceMock = null!;
    private ProcessPaymentHandler _handler = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new ProcessPaymentHandler(_paymentServiceMock.Object, new TestableClock());
        _fixture = new Fixture();

        // Customize the fixture setup for ProcessPaymentCommand
        _fixture.Customize<ProcessPaymentCommand>(composer => composer
            .With(p => p.Amount, _fixture.Create<decimal>() % 10000 + 0.01m)  // Generates amounts between 0.01 and 10000.00
            .With(p => p.CardNumber, "4111111111111111") // Mock typical card number
            .With(p => p.Currency, "USD")
            .With(p => p.CardExpiryYear, DateTime.UtcNow.Year + 1) // Ensure the card is not expired
            .With(p => p.CardExpiryMonth, DateTime.UtcNow.Month)
            .With(p => p.Cvv, 123)); // Mock typical CVV
    }
    
    [Test]
    public void Handle_WhenValidationFails_ThrowsValidationException()
    {
        // Arrange
        var command = _fixture.Build<ProcessPaymentCommand>()
            .With(x => x.CardNumber, "") // Invalid value to trigger validation failure
            .Create();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex.Message, Does.Contain("Payment request validation failed:"));
    }

    [Test]
    public void Handle_WhenCardIsExpired_ThrowsValidationException()
    {
        // Arrange
        var command = _fixture.Build<ProcessPaymentCommand>()
            .With(p => p.Amount,
                _fixture.Create<decimal>() % 10000 + 0.01m) // Generates amounts between 0.01 and 10000.00
            .With(p => p.CardNumber, "4111111111111111") // Mock typical card number
            .With(p => p.Currency, "USD")
            .With(p => p.CardExpiryYear, DateTime.UtcNow.Year) // Ensure the card is not expired
            .With(p => p.CardExpiryMonth, DateTime.UtcNow.Month-1)
            .With(p => p.Cvv, 123)
            .Create();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex.Message, Is.EqualTo("Card is expired."));
    }
    
    [TestCase(0)]
    [TestCase(1_000_001)]
    public void Handle_WhenAmountIsInvalid_ThrowsValidationException(decimal amount)
    {
        // Arrange
        var command = _fixture.Build<ProcessPaymentCommand>()
            .With(p => p.Amount,
                amount) // Generates amounts between 0.01 and 10000.00
            .With(p => p.CardNumber, "4111111111111111") // Mock typical card number
            .With(p => p.Currency, "USD")
            .With(p => p.CardExpiryYear, DateTime.UtcNow.Year) // Ensure the card is not expired
            .With(p => p.CardExpiryMonth, DateTime.UtcNow.Month)
            .With(p => p.Cvv, 123)
            .Create();

        // Act & Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex.Message, Is.EqualTo($"Amount {amount} is not in the allowed range: [${PaymentConstants.MinAmount} and {PaymentConstants.MaxAmount}]."));
    }

    [Test]
    public async Task Handle_WhenPaymentSucceeds_ReturnsSuccessfulPaymentResult()
    {
        // Arrange
        var command = _fixture.Create<ProcessPaymentCommand>();
        var paymentResult = new Payment(
            id: _fixture.Create<long>(),
            identifier: Guid.NewGuid(),
            cardToken: _fixture.Create<string>(),
            amount: command.Amount,
            currency: command.Currency,
            status: PaymentStatus.Processed,
            bankTransactionId: _fixture.Create<string>());

        _paymentServiceMock.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentProcessingData>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.That(result.Message, Is.EqualTo("Payment processed successfully."));
        Assert.That(result.PaymentId, Is.EqualTo(paymentResult.Identifier.ToString()));
    }

    [Test]
    public void Handle_WhenPaymentServiceThrowsException_ThrowsApplicationException()
    {
        // Arrange
        var command = _fixture.Create<ProcessPaymentCommand>();
        var expectedException = new Exception("Unexpected error");

        _paymentServiceMock.Setup(x => x.ProcessPaymentAsync(It.IsAny<PaymentProcessingData>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act & Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.That(ex.InnerException?.Message, Is.EqualTo(expectedException.Message));
    }
}
