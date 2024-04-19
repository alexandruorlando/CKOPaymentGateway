using PaymentGateway.Contracts.DTOs.v1;
using PaymentGateway.TestUtilities.Builders;

namespace PaymentGateway.Api.Integration.Tests.Controllers.PaymentController;

public class HttpClientHelper(HttpClient client)
{
    public Task<HttpResponseMessage> SendPaymentRequest(PaymentRequest paymentRequest, string idempotencyKey)
    {
        var httpRequestBuilder = new HttpRequestBuilder(client.BaseAddress!.ToString())
            .WithMethod(HttpMethod.Post)
            .WithPath("api/v1/Payment")
            .WithJsonContent(paymentRequest)
            .WithHeader("Idempotency-Key", idempotencyKey);

        var requestMessage = httpRequestBuilder.Build();
        return client.SendAsync(requestMessage);
    }
    
    public Task<HttpResponseMessage> GetPaymentDetails(string paymentId)
    {
        var httpRequestBuilder = new HttpRequestBuilder(client.BaseAddress!.ToString())
            .WithMethod(HttpMethod.Get)
            .WithPath($"api/v1/Payment/{paymentId}");

        var requestMessage = httpRequestBuilder.Build();
        return client.SendAsync(requestMessage);
    }
}