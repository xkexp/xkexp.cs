using Orleans;

namespace TestKitTest.Grains;

public sealed class DiGrain : Grain, IGrainWithGuidKey
{
    public DiGrain(IDiService service)
    {
        Service = service;
    }

    public IDiService Service { get; }

    public bool GetServiceValue() => Service.GetValue();
}

public interface IDiService
{
    bool GetValue();
}