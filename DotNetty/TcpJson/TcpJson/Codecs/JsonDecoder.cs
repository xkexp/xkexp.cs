using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System.Text;
using System.Text.Json;
using TcpJson.Messages;

namespace TcpJson.Codecs;

internal class JsonDecoder : ByteToMessageDecoder
{
    protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
    {
        if (input.ReadableBytes < sizeof(int))
        {
            Console.WriteLine($"W1: 可读长度不够: ReadableBytes: {input.ReadableBytes}");
            return;
        }

        var headLength = (int) input.ReadUnsignedIntLE();
        if (input.ReadableBytes < headLength)
        {
            Console.WriteLine($"W2: 可读长度不够: ReadableBytes: {input.ReadableBytes}");
            input.ResetReaderIndex();
            return;
        }

        var headString = input.ReadString(headLength, Encoding.UTF8);
        var head = JsonSerializer.Deserialize<RequestHead>(headString);

        if (input.ReadableBytes < head?.body_length)
        {
            Console.WriteLine($"W3: 可读长度不够: ReadableBytes: {input.ReadableBytes}");
            input.ResetReaderIndex();
            return;
        }

        var bodyString = input.ReadString(head.body_length, Encoding.UTF8);
        var body = JsonSerializer.Deserialize<TestMessage>(bodyString);

        output.Add(new Context()
        {
            Head = head,
            Body = body,
        });
    }
}
