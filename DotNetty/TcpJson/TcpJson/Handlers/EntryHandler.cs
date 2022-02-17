using DotNetty.Transport.Channels;

namespace TcpJson.Handlers;

internal class EntryHandler : ChannelHandlerAdapter
{
    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        Console.WriteLine(message is Context);
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
