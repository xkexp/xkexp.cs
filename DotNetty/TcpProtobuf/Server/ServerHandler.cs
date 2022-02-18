using DotNetty.Transport.Channels;
using TcpProtobuf.Messages;

namespace TcpProtobuf.Handlers;

internal class ServerHandler : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is TestMessage tm)
        {
            Console.WriteLine($"收到: {tm}");
        }
        base.ChannelRead(context, message);
    }

    public override void ChannelReadComplete(IChannelHandlerContext context)
    {
        context.Flush();
    }
    public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
    {
        Console.WriteLine($"Gateway Exception: {exception}");
        context.CloseAsync();
    }
}
