using System.Text;
using System.Text.Json;
using LibraryManagementSystem.Service;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace Tests.Services;

public class CacheServiceTest
{
    [Fact]
    public async Task GetAsync_ReturnsCachedValue_WhenKeyExists()
    {
        // Arrange
        var mockCache = new Mock<IDistributedCache>();
        var cacheService = new CacheService(mockCache.Object);

        const string key = "test_key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var serializeValue = JsonSerializer.Serialize(value);
        var encodedValue = Encoding.UTF8.GetBytes(serializeValue);

        mockCache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync(encodedValue);

        // Act
        var result = await cacheService.GetAsync<TestObject>(key);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(value.Id, result.Id);
        Assert.Equal(value.Name, result.Name);
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_WhenKeyDoesNotExist()
    {
        // Arrange
        var mockCache = new Mock<IDistributedCache>();
        var cacheService = new CacheService(mockCache.Object);

        const string key = "non_existent_key";

        mockCache.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
            .ReturnsAsync((byte[]?)null);

        // Act
        var result = await cacheService.GetAsync<TestObject>(key);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task SetAsync_SetsValueInCache()
    {
        // Arrange
        var mockCache = new Mock<IDistributedCache>();
        var cacheService = new CacheService(mockCache.Object);

        const string key = "test_key";
        var value = new TestObject { Id = 1, Name = "Test" };
        var serializedValue = JsonSerializer.Serialize(value);
        var encodedValue = Encoding.UTF8.GetBytes(serializedValue);

        mockCache
            .Setup(c => c.SetAsync(
                key,
                It.Is<byte[]>(b => b.SequenceEqual(encodedValue)),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await cacheService.SetAsync(key, value);

        // Assert
        mockCache.Verify(
            c => c.SetAsync(
                key,
                It.Is<byte[]>(b => b.SequenceEqual(encodedValue)),
                It.IsAny<DistributedCacheEntryOptions>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_RemovesValueFromCache()
    {
        // Arrange
        var mockCache = new Mock<IDistributedCache>();
        var cacheService = new CacheService(mockCache.Object);

        const string key = "test_key";

        mockCache.Setup(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await cacheService.RemoveAsync(key);

        // Assert
        mockCache.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
    }

    private class TestObject
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}