using System;
using System.Threading.Tasks;
using TestKitTest.Grains;
using Xunit;
using Orleans.TestKit;
using Moq;

namespace TestKitTest.Tests;

public class DiGrainTests : TestKitBase
{

    [Fact]
    public async Task CreateGrainWithService()
    {
        var grain = await Silo.CreateGrainAsync<DiGrain>(Guid.NewGuid());

        Assert.NotNull(grain.Service);
    }


    [Fact]
    public async Task SetupGrainService()
    {
        var mockSvc = new Mock<IDiService>();
        mockSvc.Setup(x => x.GetValue()).Returns(true);

        Silo.ServiceProvider.AddServiceProbe(mockSvc);
        var grain = await Silo.CreateGrainAsync<DiGrain>(Guid.NewGuid());

        Assert.True(grain.GetServiceValue());
        mockSvc.Verify(x => x.GetValue(), Times.Once);
    }
}
