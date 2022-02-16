using Orleans.TestingHost;
using System;
using System.Threading.Tasks;
using xk.experiment.OfOrleans.Hello;
using Xunit;

namespace Hello.Test;


[Collection(ClusterCollection.Name)]
public class HelloGrainTests2
{
    private readonly TestCluster _cluster;

    public HelloGrainTests2(ClusterFixture fixture)
    {
        _cluster = fixture.Cluster;
    }

    [Fact]
    public async Task SaysHelloCorrectly()
    {
        var hello = _cluster.GrainFactory.GetGrain<IHelloGrain>(Guid.NewGuid().ToString());
        var greeting = await hello.SayHello("Test");

        Assert.Equal("Hello, Test!", greeting);
    }
}