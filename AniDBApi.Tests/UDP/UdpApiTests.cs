using System.Threading.Tasks;
using AniDBApi.UDP;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace AniDBApi.Tests.UDP;

public class UdpApiTests
{
    // "300 PONG\n"
    private static readonly byte[] PingResult = { 0x33, 0x30, 0x30, 0x20, 0x50, 0x4F, 0x4E, 0x47, 0xA };

    private readonly ILogger<UdpApi> _logger;

    public UdpApiTests(ITestOutputHelper testOutputHelper)
    {
        _logger = new XUnitLogger<UdpApi>(testOutputHelper);
    }

    private UdpApi SetupApi(byte[] commandResult)
    {
        return TestUtils.IsCI
            ? UdpTestUtils.SetupApiFakeClient(_logger, commandResult)
            : UdpTestUtils.SetupApiRealClient(_logger);
    }

    [Fact]
    public void TestCreateResult()
    {
        var api = SetupApi(System.Array.Empty<byte>());
        var result = api.CreateResult(PingResult);
        Assert.Equal(300, result.ReturnCode);
        Assert.Equal("PONG", result.ReturnString);
    }

    [Fact]
    public async Task TestPing()
    {
        var api = SetupApi(PingResult);
        var result = await api.Ping();
        TestResult(300, "PONG", result);
    }

    private static void TestResult(int returnCode, string returnString, UdpApiResult result)
    {
        Assert.Equal(returnCode, result.ReturnCode);
        Assert.Equal(returnString, result.ReturnString);
    }
}
