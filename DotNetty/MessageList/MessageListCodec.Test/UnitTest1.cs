using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using Google.Protobuf;
using System.Collections.Generic;
using xkexp.DotNetty.MessageListCodec;
using Xunit;

namespace MessageListCodec.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var channel = new EmbeddedChannel(
                new MessageListDecoder(),
                new MessageListEncoder()
            );

            var list = new List<IMessage>()
            {
                new Test1Message() { T1 = 999 },
                new Test2Message() { T2 = "test2" },
            };

            Assert.True(channel.WriteOutbound(list));
            var buffer = channel.ReadOutbound<IByteBuffer>();

            var data = new byte[buffer.ReadableBytes];
            buffer.ReadBytes(data);

            IByteBuffer inputBuffer;
            inputBuffer = Unpooled.WrappedBuffer(data);


            Assert.True(channel.WriteInbound(inputBuffer));

            var ml = channel.ReadInbound<List<IMessage>>();
            Assert.NotNull(ml);
        }
    }
}