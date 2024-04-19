using System.Net.Http.Json;

namespace PaymentGateway.TestUtilities.Builders;

public class HttpRequestBuilder(string baseUri)
{
    private HttpRequestMessage _request = new();

    public HttpRequestBuilder WithMethod(HttpMethod method)
    {
        _request.Method = method;
        return this;
    }

    public HttpRequestBuilder WithPath(string path)
    {
        _request.RequestUri = new Uri($"{baseUri}{path}");
        return this;
    }

    public HttpRequestBuilder WithJsonContent<T>(T content)
    {
        _request.Content = JsonContent.Create(content);
        return this;
    }

    public HttpRequestBuilder WithHeader(string name, string value)
    {
        _request.Headers.TryAddWithoutValidation(name, value);
        return this;
    }

    public HttpRequestMessage Build()
    {
        return _request;
    }

    public HttpRequestBuilder Reset()
    {
        _request = new HttpRequestMessage();
        return this;
    }
}
