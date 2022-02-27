using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System.Diagnostics.Contracts;
using xkexp.DotNetty.MessageArray;

namespace MessageArray.Codec;

public class MessageArrayDecoder : ByteToMessageDecoder
{
    private readonly Dictionary<int, MessageParser> _parsers = new Dictionary<int, MessageParser>()
    {
        { 10, MessageHead.Parser },
        { 11, MessageHeads.Parser },
        { 12, Test1Message.Parser },
        { 13, Test2Message.Parser },
    };

    protected override void Decode(IChannelHandlerContext context, IByteBuffer buffer, List<object> output)
    {
        Contract.Requires(context != null);
        Contract.Requires(buffer != null);
        Contract.Requires(output != null);

        var step = 0;
        if (buffer.ReadableBytes < sizeof(int))
        {
            Console.WriteLine($"{step++}: 可读长度不够: ReadableBytes: {buffer.ReadableBytes}");
            return;
        }

        // 读取头部长度
        var length = (int)buffer.ReadUnsignedInt();
        if (buffer.ReadableBytes < length)
        {
            Console.WriteLine($"{step++}: 可读长度不够: ReadableBytes: {buffer.ReadableBytes}");
            return;
        }

        // 读取头部数组
        var heads = Read(buffer, length, MessageHeads.Parser);
        if (heads == null)
        {
            Console.WriteLine($"{step++}: 读取头部失败");
            return;
        }
        if (heads.Heads.Count == 0)
        {
            Console.WriteLine($"{step++}: 头部为空");
            return;
        }

        // 检查body类型
        var body_length_need = 0;
        foreach (var head in heads.Heads)
        {
            if (!_parsers.ContainsKey(head.BodyTypeId))
            {
                Console.WriteLine($"{step++}: type of id ({head.BodyTypeId}) is not defined.");
                return;
            }
            body_length_need += head.BodyLength;
        }
        if (buffer.ReadableBytes < body_length_need)
        {
            Console.WriteLine($"{step++}: 可读长度不够: ReadableBytes: {buffer.ReadableBytes}");
            return;
        }

        // 读取消息列表
        var list = new List<IMessage>(heads.Heads.Count);
        foreach (var head in heads.Heads)
        {
            _parsers.TryGetValue(head.BodyTypeId, out var parser);
            var m = ReadMessage(buffer, head.BodyLength, parser);
            list.Add(m);
        }

        output.Add(list);
    }

    private IMessage ReadMessage(IByteBuffer buffer, int length, MessageParser parser)
    {
        Stream inputStream = null;
        try
        {
            CodedInputStream codedInputStream;
            if (buffer.IoBufferCount > 0)
            {
                ArraySegment<byte> bytes = buffer.GetIoBuffer(buffer.ReaderIndex, length);
                codedInputStream = new CodedInputStream(bytes.Array, bytes.Offset, length);
            }
            else
            {
                inputStream = new ReadOnlyByteBufferStream(buffer, false);
                codedInputStream = new CodedInputStream(inputStream);
            }

            var message = parser.ParseFrom(codedInputStream);
            buffer.SkipBytes(length);
            return message;
        }
        catch (Exception exception)
        {
            throw new CodecException(exception);
        }
        finally
        {
            inputStream?.Dispose();
        }
    }

    private T Read<T>(IByteBuffer buffer, int length, MessageParser<T> parser)
        where T : IMessage<T>
    {
        return (T)ReadMessage(buffer, length, parser);
    }
}
