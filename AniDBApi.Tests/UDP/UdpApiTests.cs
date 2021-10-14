using System;
using System.Collections.Generic;
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

    private UdpApi SetupApi(string commandName, string resultFile)
    {
        return SetupApi(new Dictionary<string, string>
        {
            { commandName, resultFile }
        });
    }

    private UdpApi SetupApi(Dictionary<string, string> results)
    {
        return TestUtils.IsCI
            ? UdpTestUtils.SetupApiFakeClient(_logger, results)
            : UdpTestUtils.SetupApiRealClient(_logger);
    }

    [Fact]
    public void TestCreateResult()
    {
        var api = SetupApi("PING", "PING.dat");
        var result = api.CreateResult(new byte[] { 0x33, 0x30, 0x30, 0x20, 0x50, 0x4F, 0x4E, 0x47, 0xA });
        Assert.Equal(300, result.ReturnCode);
        Assert.Equal("PONG", result.ReturnString);
    }

    [Fact]
    public async Task TestPing()
    {
        var api = SetupApi("PING", "PING.dat");
        var result = await api.Ping();
        TestResult(result, 300, "PONG");
    }

    [Fact]
    public async Task TestVersion()
    {
        var api = SetupApi("VERSION", "VERSION.dat");
        var result = await api.Version();
        TestResult(result, 998, "VERSION", 1);
        Assert.Equal("0.03.851 (2020-08-15)", result.Lines[0][0]);
    }

    [Fact]
    public async Task TestAuthCycle()
    {
        var api = SetupApi(new Dictionary<string, string>
        {
            {"AUTH", "AUTH.dat"},
            {"LOGOUT", "LOGOUT.dat"}
        });

        var (username, password) = GetUserCredentials();
        var authResult = await api.Auth(username, password);

        Assert.Equal(200, authResult.ReturnCode);
        Assert.True(api.IsAuthenticated);

        var logoutResult = await api.Logout();
        TestResult(logoutResult, 203, "LOGGED OUT");
        Assert.False(api.IsAuthenticated);
    }

    private static (string, string) GetUserCredentials()
    {
        if (TestUtils.IsCI) return ("moq", "moq");

        var username = Environment.GetEnvironmentVariable("USERNAME", EnvironmentVariableTarget.Process);
        var password = Environment.GetEnvironmentVariable("PASSWORD", EnvironmentVariableTarget.Process);

        Assert.NotNull(username);
        Assert.NotNull(password);

        return (username!, password!);
    }

    private static void TestResult(UdpApiResult result, int returnCode, string returnString, int lineCount = 0)
    {
        Assert.Equal(returnCode, result.ReturnCode);
        Assert.Equal(returnString, result.ReturnString);
        Assert.Equal(lineCount, result.Lines.Count);
    }
}
