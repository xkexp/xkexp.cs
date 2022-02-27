using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Google.Protobuf;
using System.Diagnostics.Contracts;
using xkexp.DotNetty.MessageArray;

namespace MessageArray.Codec;

public class MessageArrayEncoder : MessageToByteEncoder<List<IMessage>>
{
    private readonly Dictionary<Type, int> _messageIds = new Dictionary<Type, int>()
    {
        { typeof(MessageHead), 10 },
        { typeof(MessageHeads), 11 },
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
            if (!_messageIds.ContainsKey(message.GetType()))
            {
                Console.WriteLine($"type of message ({message.GetType().FullName}) not defined.");
                return;
            }
        }

        var heads = new MessageHeads();
        foreach (var message in messages)
        {
            _messageIds.TryGetValue(message.GetType(), out var messageId);
            var head = new MessageHead()
            {
                BodyTypeId = messageId,
                BodyLength = message.CalculateSize(),
            };

            heads.Heads.Add(head);
        }
        output.WriteInt(heads.CalculateSize());
        output.WriteBytes(heads.ToByteArray());

        foreach (var message in messages)
        {
            output.WriteBytes(message.ToByteArray()); 
        }
    }
}
