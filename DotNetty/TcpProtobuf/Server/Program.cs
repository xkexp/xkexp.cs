using DotNetty.Codecs;
using DotNetty.Codecs.Protobuf;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using TcpProtobuf.Handlers;
using TcpProtobuf.Messages;

var bossGroup = new MultithreadEventLoopGroup(1);
var workerGroup = new MultithreadEventLoopGroup();

try
{
    var bootstrap = new ServerBootstrap()
        .Group(bossGroup, workerGroup)
        .Channel<TcpServerSocketChannel>()
        .Option(ChannelOption.SoBacklog, 100)
        .ChildHandler(new ActionChannelInitializer<IChannel>(channel =>
        {
            Console.WriteLine("连入新的连接");

            var pipeline = channel.Pipeline;
            pipeline.AddLast(new LengthFieldBasedFrameDecoder(ushort.MaxValue, 0, 4, 0, 4));
            pipeline.AddLast(new ProtobufDecoder(TestMessage.Parser));
            pipeline.AddLast(new ServerHandler());
            pipeline.AddLast(new LengthFieldPrepender(4));
            pipeline.AddLast(new ProtobufEncoder());
        }));

    var bootChannel = await bootstrap.BindAsync(8087);

    Console.WriteLine("开始监听端口: 8087");
    Console.ReadLine();
    await bootChannel.CloseAsync();
}
finally
{
    await Task.WhenAll(
        bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
        workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
}