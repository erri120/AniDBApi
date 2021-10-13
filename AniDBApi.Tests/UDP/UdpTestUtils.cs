using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using AniDBApi.UDP;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace AniDBApi.Tests.UDP;

public static class UdpTestUtils
{
    public static UdpApi SetupApiRealClient(ILogger<UdpApi> logger)
    {
        Assert.False(TestUtils.IsCI);

        var client = new UdpClient();
        return new UdpApi(logger, new UdpClientWrapper(client));
    }

    public static UdpApi SetupApiFakeClient(ILogger<UdpApi> logger, byte[] commandResult)
    {
        Assert.True(TestUtils.IsCI);

        var client = new Mock<IUdpClient>();
        client.Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()));
        client.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
            .Returns<ReadOnlyMemory<byte>,CancellationToken>((datagram, _) => new ValueTask<int>(datagram.Length));
        client.Setup(x => x.ReceiveAsync(It.IsAny<CancellationToken>()))
            .Returns<CancellationToken>(_ => new ValueTask<UdpReceiveResult>(new UdpReceiveResult(commandResult, new IPEndPoint(0x2414188f, 1))));

        return new UdpApi(logger, client.Object, "testing.com", -1);
    }
}
