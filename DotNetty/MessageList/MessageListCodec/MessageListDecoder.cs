using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using xkexp.DotNetty.MessageListCodec;

namespace MessageListCodec
{
    public class MessageListDecoder : ByteToMessageDecoder
    {
        private readonly Dictionary<int, MessageParser> _parsers = new Dictionary<int, MessageParser>()
    {
        { 10, VariantMessage.Parser },
        { 11, MessageList.Parser },
        { 12, Test1Message.Parser },
        { 13, Test2Message.Parser },
    };

        protected override void Decode(IChannelHandlerContext context, IByteBuffer buffer, List<object> output)
        {
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

            var messageList = Read(buffer, length, MessageList.Parser);
            if (messageList == null)
            {
                Console.WriteLine($"{step++}: 读取头部失败");
                return;
            }

            var list = new List<IMessage>(messageList.Messages.Count);
            foreach (var message in messageList.Messages)
            {
                if (!_parsers.TryGetValue(message.MessageId, out var parser))
                {
                    Console.WriteLine($"type of id ({message.MessageId}) is not defined.");
                    return;
                }
            }

            foreach (var message in messageList.Messages)
            {
                _parsers.TryGetValue(message.MessageId, out var parser);
                var m = parser.ParseFrom(message.BodyContent);
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
}