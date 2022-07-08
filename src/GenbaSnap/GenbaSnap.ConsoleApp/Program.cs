using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GenbaSnap.ConsoleApp
{
    internal static class Program
    {
        static void Main()
        {
            var hostBuilder = new HostBuilder()
                .ConfigureAppConfiguration(configuration =>
                {

                })
                .ConfigureServices((context, services) =>
                {
                    services.AddMemoryCache();
                    services.AddSnapServices();

                    services.AddSingleton<IGameEngine, GameEngine>();
                    services.AddSingleton<ICommandService, ConsoleCommandService>();

                    services.AddLogging(logging =>
                    {
                        logging.AddConsole();
                    });
                });

            using var host = hostBuilder.Build();
            var engine = host.Services.GetRequiredService<IGameEngine>();

            engine.PlayGame();
        }
    }
}