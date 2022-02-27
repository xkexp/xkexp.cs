using DotNetty.Codecs;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using MessageArray.Codec;
using MessageArray.Server;

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
            pipeline.AddLast(new MessageArrayDecoder());
            pipeline.AddLast(new ServerHandler());
            pipeline.AddFirst(new MessageArrayEncoder());
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