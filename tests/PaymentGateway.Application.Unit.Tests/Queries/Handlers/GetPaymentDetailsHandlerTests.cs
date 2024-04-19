using Moq;
using AutoFixture;
using System.ComponentModel.DataAnnotations;
using PaymentGateway.Application.Queries.Handlers;
using PaymentGateway.Domain.Abstractions;
using PaymentGateway.Domain.POCOs;
using PaymentGateway.Application.Queries;

namespace PaymentGateway.Application.Unit.Tests.Queries.Handlers;

[TestFixture]
public class GetPaymentDetailsHandlerTests
{
    private Mock<IPaymentService> _paymentServiceMock = null!;
    private GetPaymentDetailsHandler _handler = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void SetUp()
    {
        _paymentServiceMock = new Mock<IPaymentService>();
        _handler = new GetPaymentDetailsHandler(_paymentServiceMock.Object);
        _fixture = new Fixture();
    }

    [Test]
    public void Handle_WhenPaymentIdIsInvalid_ThrowsValidationException()
    {
        // Arrange
        var request = new GetPaymentDetailsQuery("not-a-valid-guid");

        // Act
        // Assert
        var ex = Assert.ThrowsAsync<ValidationException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.That(ex.Message, Is.EqualTo("Payment identifier is invalid."));
    }

    [Test]
    public async Task Handle_WhenPaymentIdIsValid_ReturnsPaymentDetails()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var request = new GetPaymentDetailsQuery(validGuid);
        var paymentResult = _fixture.Create<PaymentDetails>();

        _paymentServiceMock.Setup(x => x.GetPaymentDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(paymentResult);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        Assert.That(result.PaymentId, Is.EqualTo(paymentResult.PaymentId));
        Assert.That(result.MaskedCardNumber, Is.EqualTo(paymentResult.MaskedCardNumber));
        Assert.That(result.CardExpiryMonth, Is.EqualTo(paymentResult.CardExpiryMonth));
        Assert.That(result.CardExpiryYear, Is.EqualTo(paymentResult.CardExpiryYear));
        Assert.That(result.Amount, Is.EqualTo(paymentResult.Amount));
        Assert.That(result.Currency, Is.EqualTo(paymentResult.Currency));
        Assert.That(result.Status, Is.EqualTo(paymentResult.Status));
    }

    [Test]
    public void Handle_WhenPaymentDetailsNotFound_ThrowsKeyNotFoundException()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var request = new GetPaymentDetailsQuery(validGuid);

        _paymentServiceMock.Setup(x => x.GetPaymentDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        // Assert
        Assert.ThrowsAsync<KeyNotFoundException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Test]
    public void Handle_WhenUnhandledExceptionOccurs_ThrowsApplicationException()
    {
        // Arrange
        var validGuid = Guid.NewGuid().ToString();
        var request = new GetPaymentDetailsQuery(validGuid);
        var exceptionMessage = "Some unhandled exception";

        _paymentServiceMock.Setup(x => x.GetPaymentDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        // Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(request, CancellationToken.None));
        Assert.That(ex.InnerException?.Message, Is.EqualTo(exceptionMessage));
    }
}
