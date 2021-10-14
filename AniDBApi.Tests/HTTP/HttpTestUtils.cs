using System;
using System.Net;
using System.Net.Http;
using AniDBApi.HTTP;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using Xunit;

namespace AniDBApi.Tests.HTTP;

public static class HttpTestUtils
{
    public static HttpApi SetupApiRealClient(ILogger<HttpApi> logger)
    {
        Assert.False(TestUtils.IsCI);

        var client = new HttpClient(new SocketsHttpHandler
        {
            ConnectTimeout = TimeSpan.FromSeconds(1),
            AutomaticDecompression = DecompressionMethods.GZip
        });

        var clientName = TestUtils.GetEnvironmentVariable("CLIENT_NAME_HTTP");
        var sClientVer = TestUtils.GetEnvironmentVariable("CLIENT_VER_HTTP");

        Assert.True(int.TryParse(sClientVer, out var clientVer));
        return new HttpApi(logger, client, clientName!, clientVer);
    }

    public static HttpApi SetupApiFakeClient(ILogger<HttpApi> logger, Action<Mock<HttpMessageHandler>> setupHandler)
    {
        Assert.True(TestUtils.IsCI);

        var handler = new Mock<HttpMessageHandler>();
        setupHandler(handler);

        var client = handler.CreateClient();
        return new HttpApi(logger, client, "moq", 1, "https://testing.com/httpapi");
    }
}
