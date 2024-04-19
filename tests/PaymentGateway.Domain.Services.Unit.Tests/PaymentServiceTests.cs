using AutoFixture;
using Moq;
using PaymentGateway.Domain.Entities;
using PaymentGateway.Domain.POCOs;
using PaymentGateway.Domain.Services;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator.Dtos;
using PaymentGateway.Infrastructure.Abstractions.Data.Repositories;
using PaymentGateway.Infrastructure.Abstractions.Sequencers;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider.Dtos;

namespace PaymentGateway.Core.Unit.Tests;

public class PaymentServiceTests
{
    private Mock<ICardTokenizationService> _cardTokenizationServiceMock = null!;
    private Mock<IBankSimulatorService> _bankSimulatorServiceMock = null!;
    private Mock<IPaymentRepository> _paymentRepositoryMock = null!;
    private Mock<IIdGenerator> _idGeneratorMock = null!;
    private PaymentService _paymentService = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        SetUpMocks();

        _paymentService = new PaymentService(
            _cardTokenizationServiceMock.Object,
            _bankSimulatorServiceMock.Object,
            _paymentRepositoryMock.Object,
            _idGeneratorMock.Object);
    }

    [Test]
    public void ProcessPaymentAsync_WhenBankReturnsUnsuccessfulResponse_ThrowsException()
    {
        // Arrange
        _bankSimulatorServiceMock.Setup(b =>
                b.ProcessPaymentAsync(It.IsAny<ProcessBankPaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessBankPaymentResponse
            {
                IsSuccess = false,
                Message = "Incorrect CVV"
            });

        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        // Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(() =>
            _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None));
        Assert.That(ex!.Message, Does.Contain("Error processing payment. Please try again later."));
        _cardTokenizationServiceMock.Verify(x => x.TokenizeCardAsync(It.IsAny<TokenizeCardRequest>()), Times.Never);
        _idGeneratorMock.Verify(x => x.GenerateId(), Times.Never);
        _paymentRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Payment>(), CancellationToken.None), Times.Never);
    }

    [Test]
    public async Task ProcessPaymentAsync_WhenProcessedSuccessfully_CallsBankSimulatorOnce()
    {
        // Arrange
        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        await _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None);

        // Assert
        _bankSimulatorServiceMock.Verify(
            x => x.ProcessPaymentAsync(It.IsAny<ProcessBankPaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Test]
    public async Task ProcessPaymentAsync_WhenProcessedSuccessfully_CallsIdGeneratorOnce()
    {
        // Arrange
        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        await _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None);

        // Assert
        _idGeneratorMock.Verify(x => x.GenerateId(), Times.Once);
    }

    [Test]
    public async Task ProcessPaymentAsync_WhenProcessedSuccessfully_CallsPaymentRepositoryOnce()
    {
        // Arrange
        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        await _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None);

        // Assert
        _paymentRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Payment>(), CancellationToken.None), Times.Once);
    }

    [Test]
    public async Task ProcessPaymentAsync_WhenProcessedSuccessfully_CallsCardTokenizerOnce()
    {
        // Arrange
        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        await _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None);

        // Assert
        _cardTokenizationServiceMock.Verify(x => x.TokenizeCardAsync(It.IsAny<TokenizeCardRequest>()), Times.Once);
    }

    [Test]
    public void ProcessPaymentAsync_WhenTokenizationServiceReturnsEmptyToken_ThrowsException()
    {
        // Arrange
        _cardTokenizationServiceMock.Setup(c =>
                c.TokenizeCardAsync(It.IsAny<TokenizeCardRequest>()))
            .ReturnsAsync(new TokenizeCardResponse
            {
                Token = string.Empty
            });

        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        // Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(() =>
            _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None));
        Assert.That(ex!.Message,
            Does.Contain($"{ErrorMessage.ApplicationFailure}. Details: {ErrorMessage.PaymentTokenizerFailure}"));
        _cardTokenizationServiceMock.Verify(x => x.TokenizeCardAsync(It.IsAny<TokenizeCardRequest>()), Times.Once);
        _idGeneratorMock.Verify(x => x.GenerateId(), Times.Never);
        _paymentRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Payment>(), CancellationToken.None), Times.Never);
    }
    
    [Test]
    public void ProcessPaymentAsync_WhenRepositoryThrowsException_ThrowsException()
    {
        // Arrange
        var errorMessage = "Transaction could not be saved.";
        _paymentRepositoryMock.Setup(r =>
                r.CreateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Transaction could not be saved."));

        var paymentProcessingData = _fixture.Create<PaymentProcessingData>();

        // Act
        // Assert
        var ex = Assert.ThrowsAsync<ApplicationException>(() =>
            _paymentService.ProcessPaymentAsync(paymentProcessingData, CancellationToken.None));
        Assert.That(ex!.Message,
            Does.Contain($"{ErrorMessage.ApplicationFailure}. Details: {errorMessage}"));
        _bankSimulatorServiceMock.Verify(
            x => x.ProcessPaymentAsync(It.IsAny<ProcessBankPaymentRequest>(), It.IsAny<CancellationToken>()),
            Times.Once);
        _cardTokenizationServiceMock.Verify(x => x.TokenizeCardAsync(It.IsAny<TokenizeCardRequest>()), Times.Once);
        _idGeneratorMock.Verify(x => x.GenerateId(), Times.Once);
        _paymentRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Payment>(), CancellationToken.None), Times.Once);
    }

    private void SetUpMocks()
    {
        _cardTokenizationServiceMock = new Mock<ICardTokenizationService>();
        _cardTokenizationServiceMock.Setup(b =>
                b.TokenizeCardAsync(It.IsAny<TokenizeCardRequest>()))
            .ReturnsAsync(new TokenizeCardResponse
            {
                Token = Guid.NewGuid().ToString()
            });
        _bankSimulatorServiceMock = new Mock<IBankSimulatorService>();
        _bankSimulatorServiceMock.Setup(b =>
                b.ProcessPaymentAsync(It.IsAny<ProcessBankPaymentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProcessBankPaymentResponse
            {
                IsSuccess = true,
                Message = "Payment processed successfully"
            });
        _paymentRepositoryMock = new Mock<IPaymentRepository>();
        _paymentRepositoryMock.Setup(b =>
                b.CreateAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _idGeneratorMock = new Mock<IIdGenerator>();
        _idGeneratorMock.Setup(b => b.GenerateId())
            .Returns(_fixture.Create<long>());
    }
}