
using DotNetty.Codecs;
using DotNetty.Codecs.Protobuf;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using System.Net;
using TcpProtobuf.Handlers;
using TcpProtobuf.Messages;

var group = new MultithreadEventLoopGroup();


try
{
    var bootstrap = new Bootstrap();
    bootstrap
        .Group(group)
        .Channel<TcpSocketChannel>()
        .Option(ChannelOption.TcpNodelay, true)
        .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
        {
            IChannelPipeline pipeline = channel.Pipeline;
            pipeline.AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));
            pipeline.AddLast(new ProtobufDecoder(TestMessage.Parser));
            pipeline.AddLast(new LengthFieldPrepender(4));
            pipeline.AddLast(new ProtobufEncoder());

            pipeline.AddLast(new ClientHandler());
        }));

    var clientChannel = await bootstrap.ConnectAsync(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8087));

    var message = new TestMessage()
    {
        Foo = 999,
        Bar = "hello from Program.",
    };
    Console.WriteLine($"消息: {message}");
    await clientChannel.WriteAndFlushAsync(message);

    Console.ReadLine();

    await clientChannel.CloseAsync();
}
finally
{
    await group.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1));
}