using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Database;
using PaymentGateway.TestUtilities.Builders;
using PaymentGateway.TestUtilities.Mocked;

namespace PaymentGateway.Api.Integration.Tests.Controllers.PaymentController;

public class GetPaymentDetailsTests
{
    private HttpClientHelper _httpClientHelper = null!;
    private CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private PaymentRequest _paymentRequest = null!;

    [SetUp]
    public void Setup()
    {
        _paymentRequest = new PaymentRequestBuilder().BuildValid();
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _httpClientHelper = new HttpClientHelper(_client);
    }

    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_Returns200()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
    
    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectPaymentIdentifier()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();

        // Assert
        Assert.That(result!.PaymentId, Is.EqualTo(paymentIdentifier));
    }

    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectPaymentId()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();
        // Card tokenizer is mocked with this value, a real system will return the correct stored card information
        Assert.IsTrue(result is { MaskedCardNumber: "XXXX-XXXX-XXXX-1234" });
    }
    
    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectCurrency()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();
        Assert.That(result!.Currency, Is.EqualTo(_paymentRequest.Currency));
    }

    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectCardExpiryYear()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();
        // Card tokenizer is mocked with this value, a real system will return the correct stored card information
        Assert.That(result!.CardExpiryYear, Is.EqualTo(2025));
    }

    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectCardExpiryMonth()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();
        // Card tokenizer is mocked with this value, a real system will return the correct stored card information
        Assert.That(result!.CardExpiryMonth, Is.EqualTo(12));
    }

    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectAmount()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();

        // Assert
        Assert.That(result!.Amount, Is.EqualTo(_paymentRequest.Amount));
    }
    
    [Test]
    public async Task GetPaymentDetails_WithExistingPaymentIdentifier_ReturnsCorrectStatus()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        var result = await response.Content.ReadFromJsonAsync<PaymentDetails>();
        Assert.That(result!.Status, Is.EqualTo("Processed"));
    }
    
    [Test]
    public async Task GetPaymentDetails_WithNonExistingPaymentIdentifier_Returns404()
    {
        // Arrange
        _ = await CreatePaymentAsync();

        // Act
        var response = await _httpClientHelper.GetPaymentDetails(Guid.NewGuid().ToString());

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }
    
    [Test]
    public async Task GetPaymentDetails_WhenTokenizerServiceFails_Returns500()
    {
        // Arrange
        var paymentIdentifier = await CreatePaymentAsync();
        var factory = new CustomWebApplicationFactory<Program>();
        factory.AdditionalConfigureServices = services =>
        {
            services.AddScoped<ICardTokenizationService, FaultyCardTokenizationService>();
        };
        
        var client = factory.CreateClient();
        var httpClientHelper = new HttpClientHelper(client);
        
        // Act
        var response = await httpClientHelper.GetPaymentDetails(paymentIdentifier);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    private async Task<string> CreatePaymentAsync()
    {
        var idempotencyKey = Guid.NewGuid().ToString();
        var paymentCreationResponse = await _httpClientHelper.SendPaymentRequest(_paymentRequest, idempotencyKey);
        var paymentCreationResult = await paymentCreationResponse.Content.ReadFromJsonAsync<PaymentResult>();
        return paymentCreationResult!.PaymentId!;
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PaymentDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();
        _client.Dispose();
        _factory.Dispose();
    }
}