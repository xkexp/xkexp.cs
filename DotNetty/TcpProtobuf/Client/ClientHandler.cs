using DotNetty.Transport.Channels;
using TcpProtobuf.Messages;

namespace TcpProtobuf.Handlers;

internal class ClientHandler : ChannelHandlerAdapter
{
    public override void ChannelActive(IChannelHandlerContext context)
    {
        var message = new TestMessage()
        {
            Foo = 100,
            Bar = "hello from ClientHandler!"
        };
        Console.WriteLine($"发送: {message}");
        context.WriteAndFlushAsync(message);
    }

    public override void ChannelRead(IChannelHandlerContext context, object message)
    {
        if (message is TestMessage tm)
        {
            Console.WriteLine(tm.ToString());
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
