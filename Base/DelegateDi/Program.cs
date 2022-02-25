
using DelegateDi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello DelegateDi.");

Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSingleton<Decoder.HeadChecker>(_ => (o) => true);
        services.AddSingleton<Decoder>();
        services.AddHostedService<HelloService>();
    })
    .Build()
    .Run();


class HelloService : IHostedService
{
    private readonly Decoder _decoder;
    public HelloService(Decoder decoder)
    {
        _decoder = decoder;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _decoder.Decode(0);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}