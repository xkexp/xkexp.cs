using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.Hosting;
using xkexp.OfOrleans.SimpleStream.Common;

namespace xkexp.OfOrleans.SimpleStream.Silo;

class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            var host = new HostBuilder()
                .UseOrleans(ConfigureSilo)
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await host.RunAsync();

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static void ConfigureSilo(ISiloBuilder siloBuilder)
    {
        siloBuilder
            .UseLocalhostClustering(serviceId: Constants.ServiceId, clusterId: Constants.ServiceId)
            .AddSimpleMessageStreamProvider(Constants.StreamProvider)
            .AddMemoryGrainStorage("PubSubStore");
    }
}
