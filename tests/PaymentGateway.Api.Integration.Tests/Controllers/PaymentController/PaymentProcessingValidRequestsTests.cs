using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Infrastructure.Database;
using PaymentGateway.TestUtilities.Builders;

namespace PaymentGateway.Api.Integration.Tests.Controllers.PaymentController;

public class PaymentProcessingValidRequestsTests
{
    private HttpClientHelper _httpClientHelper = null!;
    private PaymentRequestBuilder _paymentDetailsBuilder = null!;
    private CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _httpClientHelper = new HttpClientHelper(_client);
        _paymentDetailsBuilder = new PaymentRequestBuilder();
    }
    
    [Test]
    public async Task ProcessPayment_WithValidPaymentRequest_Returns200()
    {
        // Arrange
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);
        
        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [Test]
    public async Task ProcessPayment_WithValidPaymentRequest_ReturnsSuccessTrue()
    {
        // Arrange
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);
        
        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentResult>();
        Assert.IsTrue(result is { Success: true });
    }
    
    [Test]
    public async Task ProcessPayment_WithValidPaymentRequest_ReturnsExpectedMessage()
    {
        // Arrange
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);
        
        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentResult>();
        Assert.IsTrue(result is { Message: "Payment processed successfully." });
    }
    
    [Test]
    public async Task ProcessPayment_WithValidPaymentRequest_ReturnsAValidPaymentIdentifier()
    {
        // Arrange
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);
        
        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentResult>();
        var isValidGuid = Guid.TryParse(result!.PaymentId, out var parsedGuid);
        Assert.IsTrue(isValidGuid && parsedGuid != Guid.Empty);
    }
    
    [TestCase("4111111111111111", 11, 2032, 107, 20.01, "GBP", ExpectedResult = HttpStatusCode.OK)]
    [TestCase("4111111111111111", 12, 2030, 123, 150.00, "EUR", ExpectedResult = HttpStatusCode.OK)]
    public async Task<HttpStatusCode> ProcessPayment_WithVariousValidPayments_Returns200(
        string cardNumber, int month, int year, int cvv, decimal amount, string currency)
    {
        var paymentRequest = new PaymentRequestBuilder()
            .WithCardNumber(cardNumber)
            .WithCardExpiryMonth(month)
            .WithCardExpiryYear(year)
            .WithCVV(cvv)
            .WithAmount(amount)
            .WithCurrency(currency)
            .Build();

        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, Guid.NewGuid().ToString());
        return response.StatusCode;
    }
    
    [OneTimeTearDown]
    public void TearDown()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        dbContext.Database.EnsureDeleted();
        _client.Dispose();
        _factory.Dispose();
    }
}