using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System.Diagnostics.Contracts;
using xkexp.DotNetty.MessageListCodec;

namespace MessageListCodec;

public class MessageListEncoder : MessageToByteEncoder<List<IMessage>>
{
    // TODO: 可用外部配置
    private readonly Dictionary<Type, int> _messageIds = new Dictionary<Type, int>()
    {
        { typeof(VariantMessage), 10 },
        { typeof(MessageList), 11 },
        { typeof(Test1Message), 12 },
        { typeof(Test2Message), 13 },
    };

    protected override void Encode(IChannelHandlerContext context, List<IMessage> messages, IByteBuffer output)
    {
        Contract.Requires(context != null);
        Contract.Requires(messages != null);
        Contract.Requires(output != null);

        if (messages == null || messages.Count == 0)
        {
            Console.WriteLine("messages is null or empty.");
            return;
        }

        foreach (var message in messages)
        {
            if (!_messageIds.TryGetValue(message.GetType(), out var id))
            {
                Console.WriteLine($"type of message ({message.GetType().FullName}) not defined.");
                return;
            }
        }

        var messageList = new MessageList();
        foreach (var message in messages)
        {
            _messageIds.TryGetValue(message.GetType(), out var typeid);
            var size = message!.CalculateSize();
            var v = new VariantMessage()
            {
                MessageId = typeid,
                BodyLength = size,
                BodyContent = message.ToByteString()
            };
            messageList.Messages.Add(v);
        }

        output.WriteInt(messageList.CalculateSize());
        output.WriteBytes(messageList.ToByteArray());
    }
}
