using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using TcpJson.Codecs;
using TcpJson.Handlers;

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
            var pipline = channel.Pipeline;
            pipline.AddLast(new JsonDecoder());
            pipline.AddLast(new EntryHandler());
        }));

    var bootChannel = await bootstrap.BindAsync(8087);

    Console.ReadLine();
    await bootChannel.CloseAsync();
}
finally
{
    await Task.WhenAll(
        bossGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)),
        workerGroup.ShutdownGracefullyAsync(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(1)));
}