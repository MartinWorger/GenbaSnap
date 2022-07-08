using GenbaSnap.Domain.Models;
using GenbaSnap.Persistence.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace GenbaSnap.Persistence.Internal
{
    internal class SnapGameRepository : ISnapGameRepository
    {
        private readonly ILogger<SnapGameRepository> _logger;
        private readonly IMemoryCache _cache;

        internal static string SnapGameCacheKey => "GenbaSnap.Persistence.SnapGameRepository.SnapGame";

        public SnapGameRepository(
            ILogger<SnapGameRepository> logger,
            IMemoryCache cache
            )
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public SnapGame? GetSnapGame()
        {
            if (!_cache.TryGetValue(SnapGameCacheKey, out SnapGame snapGame))
            {
                _logger.LogWarning("Snap game could not be retrieved from repository");
                return null;
            }
            return snapGame;
        }

        public void SaveSnapGame(SnapGame game)
        {
            _cache.Set(SnapGameCacheKey, game);
        }
    }
}
