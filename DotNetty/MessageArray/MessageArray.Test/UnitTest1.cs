using DotNetty.Buffers;
using DotNetty.Transport.Channels.Embedded;
using Google.Protobuf;
using MessageArray.Codec;
using System.Collections.Generic;
using xkexp.DotNetty.MessageArray;
using Xunit;

namespace MessageArray.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var channel = new EmbeddedChannel(
                new MessageArrayDecoder(),
                new MessageArrayEncoder()
            );

            var list = new List<IMessage>()
            {
                new Test1Message() { T1 = 999 },
                new Test2Message() { T2 = "test2" },
            };

            Assert.True(channel.WriteOutbound(list));
            var buffer = channel.ReadOutbound<IByteBuffer>();


            Assert.True(channel.WriteInbound(buffer));
            var ml = channel.ReadInbound<List<IMessage>>();
            Assert.NotNull(ml);
        }
    }
}