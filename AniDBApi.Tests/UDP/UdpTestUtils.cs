using System;
using System.Collections.Generic;
using System.IO;
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

        var clientName = Environment.GetEnvironmentVariable("CLIENT_NAME_UDP", EnvironmentVariableTarget.Process);
        var sClientVer = Environment.GetEnvironmentVariable("CLIENT_VER_UDP", EnvironmentVariableTarget.Process);

        Assert.NotNull(clientName);
        Assert.NotNull(sClientVer);
        Assert.True(int.TryParse(sClientVer, out var clientVer));

        return new UdpApi(logger, new UdpClientWrapper(client), clientName!, clientVer);
    }

    public static UdpApi SetupApiFakeClient(ILogger<UdpApi> logger, Dictionary<string, string> results)
    {
        Assert.True(TestUtils.IsCI);

        var client = new Mock<IUdpClient>();
        client.Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()));
        client.Setup(x => x.SendAsync(It.IsAny<ReadOnlyMemory<byte>>(), It.IsAny<CancellationToken>()))
            .Returns<ReadOnlyMemory<byte>,CancellationToken>((datagram, _) => new ValueTask<int>(datagram.Length));
        client.Setup(x => x.ReceiveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns<string, CancellationToken>((commandName, _) =>
            {
                Assert.True(results.ContainsKey(commandName), $"Missing mock result: {commandName}");
                var file = TestUtils.GetResultPath("udp", results[commandName]);

                return new ValueTask<UdpReceiveResult>(new UdpReceiveResult(File.ReadAllBytes(file),
                        new IPEndPoint(0x2414188f, 1)));
            });

        return new UdpApi(logger, client.Object, "moq", 1, "testing.com", -1);
    }
}
