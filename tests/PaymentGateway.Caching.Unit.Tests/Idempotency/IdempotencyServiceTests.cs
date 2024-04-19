using System.Text.Json;
using AutoFixture;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using PaymentGateway.Caching.Idempotency;
using PaymentGateway.Infrastructure.Abstractions.Cache;

namespace PaymentGateway.Caching.Unit.Tests.Idempotency;

public class IdempotencyServiceTests
{
    private Mock<ICacheService> _cacheMock = null!;
    private IdempotencyService _idempotencyService = null!;
    private Fixture _fixture = null!;

    [SetUp]
    public void Setup()
    {
        _fixture = new Fixture();
        _cacheMock = new Mock<ICacheService>();
        SetUpMocks();
        _idempotencyService = new IdempotencyService(_cacheMock.Object);
    }

    private void SetUpMocks()
    {
        // Default behavior: return null for any GetStringAsync call
        _cacheMock.Setup(x => x.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null!);

        // Default behavior: assume SetStringAsync succeeds
        _cacheMock.Setup(x => x.SetStringAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<DistributedCacheEntryOptions>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    }

    [Test]
    public async Task GetCachedResponseAsync_WhenKeyExists_ReturnsCachedResponse()
    {
        // Arrange
        var expectedResponse = _fixture.Create<string>();
        var key = _fixture.Create<string>();
        _cacheMock.Setup(x => x.GetStringAsync(key, CancellationToken.None))
            .ReturnsAsync(JsonSerializer.Serialize(expectedResponse));

        // Act
        var result = await _idempotencyService.GetCachedResponseAsync<string>(key, CancellationToken.None);

        // Assert
        Assert.That(result, Is.EqualTo(expectedResponse));
    }

    [Test]
    public async Task GetCachedResponseAsync_WhenKeyDoesNotExist_ReturnsNull()
    {
        // Arrange
        var key = _fixture.Create<string>();
        _cacheMock.Setup(x => x.GetStringAsync(key, CancellationToken.None))
            .ReturnsAsync((string)null!);

        // Act
        var result = await _idempotencyService.GetCachedResponseAsync<string>(key, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void GetCachedResponseAsync_WhenMaxAttemptsReached_ThrowsRedisConnectionException()
    {
        // Arrange
        var key = _fixture.Create<string>();
        _cacheMock.Setup(x => x.GetStringAsync(key, CancellationToken.None))
            .ThrowsAsync(new StackExchange.Redis.RedisConnectionException(
                StackExchange.Redis.ConnectionFailureType.UnableToResolvePhysicalConnection, "Connection failed"));

        // Act
        // Assert
        Assert.ThrowsAsync<StackExchange.Redis.RedisConnectionException>(() =>
            _idempotencyService.GetCachedResponseAsync<string>(key, CancellationToken.None));
    }

    [Test]
    public async Task CacheResponseAsync_WhenCalled_SetsStringGetsCalledOnce()
    {
        // Arrange
        var response = _fixture.Create<string>();
        var key = _fixture.Create<string>();
        var serializedResponse = JsonSerializer.Serialize(response);
        var options = new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) };

        // Act
        await _idempotencyService.CacheResponseAsync(key, response, CancellationToken.None);

        // Assert
        _cacheMock.Verify(x => x.SetStringAsync(
                It.Is<string>(k => k == key),
                It.Is<string>(s => s == serializedResponse),
                It.Is<DistributedCacheEntryOptions>(o =>
                    o.AbsoluteExpirationRelativeToNow == options.AbsoluteExpirationRelativeToNow),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}