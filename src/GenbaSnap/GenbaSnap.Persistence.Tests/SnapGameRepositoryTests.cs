using GenbaSnap.Domain.Models;
using GenbaSnap.Persistence.Internal;
using GenbaSnap.Persistence.Tests.Fixtures;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace GenbaSnap.Persistence.Tests
{
    [TestClass]
    public class SnapGameRepositoryTests
    {
        private ILogger<SnapGameRepository>? logger;
        private IMemoryCache? cache;

        private SnapGameRepository NewSnapGameRepository =>
            new SnapGameRepository(logger!, cache!);

        [TestInitialize]
        public void Initialise()
        {
            logger = new NullLogger<SnapGameRepository>();
            cache = CacheTestFixture.NewMemoryCache();
        }

        [TestMethod]
        public void Ctor_WithParamNull_Logger_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameRepository(null, cache!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_WithParamNull_Cache_Throws()
        {
            // Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
            {
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
                _ = new SnapGameRepository(logger!, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
            });
        }

        [TestMethod]
        public void Ctor_CorrectParams_ReturnsService()
        {
            // Act
            var service = NewSnapGameRepository;

            // Assert
            Assert.IsNotNull(service);
        }

        [TestMethod]
        public void GetSnapGame_WhenNoGameCached_ReturnsNull()
        {
            // Arrange
            var service = NewSnapGameRepository;

            // Act
            var result = service.GetSnapGame();

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetSnapGame_WhenGameCached_ReturnsGame()
        {
            // Arrange
            var service = NewSnapGameRepository;
            var cachedGame = new SnapGame();
            cache.Set(SnapGameRepository.SnapGameCacheKey, cachedGame);

            // Act
            var result = service.GetSnapGame();

            // Assert
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void SaveSnapGame_CachesGame()
        {
            // Arrange
            var service = NewSnapGameRepository;
            var testGame = new SnapGame();

            // Act
            service.SaveSnapGame(testGame);

            // Assert
            Assert.IsTrue(cache!.TryGetValue(SnapGameRepository.SnapGameCacheKey, out var _));
        }

    }
}