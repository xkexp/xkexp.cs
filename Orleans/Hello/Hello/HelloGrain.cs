namespace xk.experiment.OfOrleans.Hello;

internal class HelloGrain : Orleans.Grain, IHelloGrain
{
    public Task<string> SayHello(string greeting)
    {
        return Task.FromResult($"Hello, {greeting}!");
    }
}
