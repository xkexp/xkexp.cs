using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using Xunit;

namespace xk.experiment.OfOrleans.Hello.Test
{
    public class HelloGrainTests
    {
        [Fact]
        public async Task SaysHelloCorrectly()
        {
            var builder = new TestClusterBuilder();
            var cluster = builder.Build();
            cluster.Deploy();

            var hello = cluster.GrainFactory.GetGrain<IHelloGrain>(Guid.NewGuid().ToString());
            var greeting = await hello.SayHello("Test");

            cluster.StopAllSilos();

            Assert.Equal("Hello, Test!", greeting);
        }
    }
}