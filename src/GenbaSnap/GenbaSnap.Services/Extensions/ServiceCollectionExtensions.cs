using GenbaSnap.Services.Interfaces;
using GenbaSnap.Services.Internal;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Dependency injection extensions.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds dependencies required by the GenbaSnap.Services assembly.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSnapServices(this IServiceCollection services)
        {
            services.AddSnapPersistenceServices();

            services.AddTransient<ICardDeckFactory, CardDeckFactory>();
            services.AddTransient<ISnapGameService, SnapGameService>();
            services.AddTransient<ISnapGameFactory, SnapGameFactory>();

            return services;
        }
    }
}
