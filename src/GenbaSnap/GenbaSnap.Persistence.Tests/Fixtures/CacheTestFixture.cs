using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace GenbaSnap.Persistence.Tests.Fixtures
{
    [ExcludeFromCodeCoverage]
    internal static class CacheTestFixture
    {
        public static IMemoryCache NewMemoryCache()
        {
            var services = new ServiceCollection();
            services.AddMemoryCache();
            var provider = services.BuildServiceProvider();
            return provider.GetRequiredService<IMemoryCache>();
        }
    }
}
