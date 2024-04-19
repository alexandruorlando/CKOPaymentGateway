using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using PaymentGateway.Api;
using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.Infrastructure.Abstractions.BankSimulator;
using PaymentGateway.Infrastructure.Abstractions.Data.Repositories;
using PaymentGateway.Infrastructure.Abstractions.TokenServiceProvider;
using PaymentGateway.Infrastructure.Database;
using PaymentGateway.TestUtilities.Builders;
using PaymentGateway.TestUtilities.Mocked;

namespace PaymentGateway.Api.Integration.Tests.Controllers.PaymentController;

public class PaymentProcessingServerErrorsTests
{
    private HttpClientHelper _httpClientHelper = null!;
    private CustomWebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private PaymentRequestBuilder _paymentDetailsBuilder = null!;

    [Test]
    public async Task ProcessPayment_WhenBankSimulatorThrowsExceptions_Returns500()
    {
        // Arrange
        _factory = new CustomWebApplicationFactory<Program>();
        _factory.AdditionalConfigureServices = services =>
        {
            services.AddScoped<IBankSimulatorService, FaultyBankSimulatorService>();
        };
        AdditionalSetUp();
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task ProcessPayment_WhenTokenizationServiceThrowsExceptions_Returns500()
    {
        // Arrange
        _factory = new CustomWebApplicationFactory<Program>();
        _factory.AdditionalConfigureServices = services =>
        {
            services.AddScoped<ICardTokenizationService, FaultyCardTokenizationService>();
        };
        AdditionalSetUp();
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    [Test]
    public async Task ProcessPayment_WhenPaymentRepositoryThrowsExceptions_Returns500()
    {
        // Arrange
        _factory = new CustomWebApplicationFactory<Program>();
        _factory.AdditionalConfigureServices = services =>
        {
            services.AddScoped<IPaymentRepository, FaultyPaymentRepository>();
        };
        AdditionalSetUp();
        var paymentRequest = _paymentDetailsBuilder.BuildValid();
        var idempotencyKey = Guid.NewGuid().ToString();

        // Act
        var response = await _httpClientHelper.SendPaymentRequest(paymentRequest, idempotencyKey);

        // Assert
        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.InternalServerError));
    }

    private void AdditionalSetUp()
    {
        _client = _factory.CreateClient();
        _httpClientHelper = new HttpClientHelper(_client);
        _paymentDetailsBuilder = new PaymentRequestBuilder();
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