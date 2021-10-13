using System.Threading.Tasks;
using AniDBApi.UDP;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace AniDBApi.Tests.UDP;

public class UdpApiTests
{
    private readonly ILogger<UdpApi> _logger;

    public UdpApiTests(ITestOutputHelper testOutputHelper)
    {
        _logger = new XUnitLogger<UdpApi>(testOutputHelper);
    }

    private UdpApi SetupApi(string resultFile)
    {
        return TestUtils.IsCI
            ? UdpTestUtils.SetupApiFakeClient(_logger, TestUtils.GetResultPath("udp", resultFile))
            : UdpTestUtils.SetupApiRealClient(_logger);
    }

    [Fact]
    public void TestCreateResult()
    {
        var api = SetupApi("PING.dat");
        var result = api.CreateResult(new byte[] { 0x33, 0x30, 0x30, 0x20, 0x50, 0x4F, 0x4E, 0x47, 0xA });
        Assert.Equal(300, result.ReturnCode);
        Assert.Equal("PONG", result.ReturnString);
    }

    [Fact]
    public async Task TestPing()
    {
        var api = SetupApi("PING.dat");
        var result = await api.Ping();
        TestResult(result, 300, "PONG");
    }

    [Fact]
    public async Task TestVersion()
    {
        var api = SetupApi("VERSION.dat");
        var result = await api.Version();
        TestResult(result, 998, "VERSION", 1);
        Assert.Equal("0.03.851 (2020-08-15)", result.Lines[0][0]);
    }

    private static void TestResult(UdpApiResult result, int returnCode, string returnString, int lineCount = 0)
    {
        Assert.Equal(returnCode, result.ReturnCode);
        Assert.Equal(returnString, result.ReturnString);
        Assert.Equal(lineCount, result.Lines.Count);
    }
}
