namespace xk.experiment.OfOrleans.Hello
{
    public interface IHelloGrain : Orleans.IGrainWithStringKey
    {
        Task<string> SayHello(string greeting);
    }
}
