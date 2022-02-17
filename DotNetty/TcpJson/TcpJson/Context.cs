using TcpJson.Messages;

namespace TcpJson;

internal class Context
{
    public Head Head { get; set; }
    
    public TestMessage Body { get; set; }
}
