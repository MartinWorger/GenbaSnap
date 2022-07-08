using GenbaSnap.Persistence.Interfaces;
using GenbaSnap.Persistence.Internal;
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
        /// Adds dependencies required by the GenbaSnap.Persistence assembly.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSnapPersistenceServices(this IServiceCollection services)
        {
            services.AddSingleton<ISnapGameRepository, SnapGameRepository>();

            return services;
        }
    }
}
