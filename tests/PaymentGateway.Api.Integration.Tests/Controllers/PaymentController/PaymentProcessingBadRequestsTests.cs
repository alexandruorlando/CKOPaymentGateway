using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Infrastructure.Database;
using PaymentGateway.TestUtilities.Builders;

namespace PaymentGateway.Api.Integration.Tests.Controllers.PaymentController;

public class PaymentProcessingBadRequestsTests
{
    private HttpClientHelper _httpClientHelper = null!;
    private CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void Setup()
    {
        _factory = new CustomWebApplicationFactory<Program>();
        _client = _factory.CreateClient();
        _httpClientHelper = new HttpClientHelper(_client);
    }
    
    [TestCase("4231111111111111", 11, 2032, 107, 20.01, "GBP",true, Description = "Invalid card number", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase(null, 11, 2032, 107, 20.01, "GBP",true, Description = "Missing card number", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase("", 11, 2032, 107, 20.01, "GBP",true, Description = "Empty card number", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase("4111111111111111", 63, 2030, 123, 150.00, "EUR",true, Description = "Invalid expiry month", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase("4111111111111111", 11, 2020, 157, 150.00, "EUR",true, Description = "Expired card", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase("4111111111111111", 11, 2032, 193, 150.00, "FGFGFD",true, Description = "Invalid currency", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase("4111111111111111", 11, 2032, 121, 0.00, "USD", true, Description = "Invalid amount", ExpectedResult = HttpStatusCode.BadRequest)]
    [TestCase("4111111111111111", 11, 2032, 771, 150.00, "USD", false, Description = "Missing idempotency key", ExpectedResult = HttpStatusCode.BadRequest)]
    public async Task<HttpStatusCode> ProcessPayment_WithVariousInvalidPayments_Returns400(
        string cardNumber, int month, int year, int cvv, decimal amount, string currency, bool withIdempotencyKey)
    {
        var paymentRequest = new PaymentRequestBuilder()
            .WithCardNumber(cardNumber)
            .WithCardExpiryMonth(month)
            .WithCardExpiryYear(year)
            .WithCVV(cvv)
            .WithAmount(amount)
            .WithCurrency(currency)
            .Build();

        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, withIdempotencyKey ? Guid.NewGuid().ToString() : "");
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