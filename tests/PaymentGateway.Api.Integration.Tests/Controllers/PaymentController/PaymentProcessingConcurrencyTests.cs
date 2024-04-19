using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Infrastructure.Database;
using PaymentGateway.TestUtilities.Builders;

namespace PaymentGateway.Api.Integration.Tests.Controllers.PaymentController;

public class PaymentProcessingConcurrencyTests
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
    public async Task ProcessPayment_WithConcurrentRequestsWithDifferentKeys_ReturnsDifferentPaymentIdentifiers()
    {
        // Arrange
        var paymentRequest1 = _paymentDetailsBuilder.BuildValid();
        var paymentRequest2 = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey1 = Guid.NewGuid().ToString();
        var idempotencyKey2 = Guid.NewGuid().ToString();

        // Act
        var sendPaymentTask1 = _httpClientHelper.SendPaymentRequest(paymentRequest1, idempotencyKey1);
        var sendPaymentTask2 = _httpClientHelper.SendPaymentRequest(paymentRequest2, idempotencyKey2);

        await Task.WhenAll(sendPaymentTask1, sendPaymentTask2);
        var response1 = sendPaymentTask1.Result;
        var response2 = sendPaymentTask2.Result;

        // Assert
        var result1 = await response1.Content.ReadFromJsonAsync<PaymentResult>();
        var result2 = await response2.Content.ReadFromJsonAsync<PaymentResult>();

        Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result2!.PaymentId, Is.Not.EqualTo(result1!.PaymentId));
    }
    
    [Test]
    public async Task ProcessPayment_WithConcurrentRequestsWithSameKey_EnsuresSingleProcessing()
    {
        // Arrange
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var sendPaymentTask1 = _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);
        var sendPaymentTask2 = _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);

        await Task.WhenAll(sendPaymentTask1, sendPaymentTask2);
        var response1 = sendPaymentTask1.Result;
        var response2 = sendPaymentTask2.Result;

        // Assert
        var result1 = await response1.Content.ReadFromJsonAsync<PaymentResult>();
        var result2 = await response2.Content.ReadFromJsonAsync<PaymentResult>();

        Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        Assert.That(result2!.PaymentId, Is.EqualTo(result1!.PaymentId));
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