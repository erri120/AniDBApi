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

    private UdpApi SetupApi(string commandName, string resultFile, bool requiresLogin = true)
    {
        return SetupApi(new Dictionary<string, string>
        {
            { commandName, resultFile }
        }, requiresLogin);
    }

    private UdpApi SetupApi(Dictionary<string, string> results, bool requiresLogin = true)
    {
        if (requiresLogin)
        {
            results.Add("AUTH", "AUTH.dat");
            results.Add("LOGOUT", "LOGOUT.dat");
        }

        return TestUtils.IsCI
            ? UdpTestUtils.SetupApiFakeClient(_logger, results)
            : UdpTestUtils.SetupApiRealClient(_logger);
    }

    [Fact]
    public void TestCreateResult()
    {
        var api = SetupApi("PING", "PING.dat", false);
        var result = api.CreateResult(new byte[] { 0x33, 0x30, 0x30, 0x20, 0x50, 0x4F, 0x4E, 0x47, 0xA });
        Assert.Equal(300, result.ReturnCode);
        Assert.Equal("PONG", result.ReturnString);
    }

    [Fact]
    public async Task TestPing()
    {
        var api = SetupApi("PING", "PING.dat", false);
        var result = await api.Ping();
        TestResult(result, 300, "PONG");
    }

    [Fact]
    public async Task TestVersion()
    {
        var api = SetupApi("VERSION", "VERSION.dat", false);
        var result = await api.Version();
        TestResult(result, 998, "VERSION", 1);
        Assert.Equal("0.03.851 (2020-08-15)", result.Lines[0][0]);
    }

    [Fact]
    public async Task TestAuthCycle()
    {
        var api = SetupApi(new Dictionary<string, string>
        {
            {"UPTIME", "UPTIME.dat"}
        });

        var (username, password) = GetUserCredentials();

        var authResult = await api.Auth(username, password);
        Assert.Equal(200, authResult.ReturnCode);
        Assert.True(api.IsAuthenticated);

        var uptimeRes = await api.Uptime();
        TestResult(uptimeRes, 208, "UPTIME", 1);

        var logoutResult = await api.Logout();
        TestResult(logoutResult, 203, "LOGGED OUT");
        Assert.False(api.IsAuthenticated);
    }

    [Fact]
    public async Task TestUptime()
    {
        var api = SetupApi("UPTIME", "UPTIME.dat");
        await using var authenticatedSession = await CreateSession(api);

        var res = await api.Uptime();
        TestResult(res, 208, "UPTIME", 1);
    }

    [Fact]
    public async Task TestEncryptionCycle()
    {
        var api = SetupApi(new Dictionary<string, string>
        {
            {"ENCRYPT", "ENCRYPT.dat"},
            {"AUTH", "AUTH-encrypted.dat"},
            {"UPTIME", "UPTIME-encrypted.dat"},
            {"LOGOUT", "LOGOUT-encrypted.dat"}
        }, false);

        var username = TestUtils.IsCI ? "moq" : TestUtils.GetEnvironmentVariable("USERNAME");
        var apiKey = TestUtils.IsCI ? "my-sexy-api-key" : TestUtils.GetEnvironmentVariable("APIKEY");

        var encryptRes = await api.Encrypt(username, apiKey);
        Assert.Equal(209, encryptRes.ReturnCode);

        await using var authenticatedSession = await CreateSession(api);
        var uptimeRes = await api.Uptime();
        TestResult(uptimeRes, 208, "UPTIME", 1);
    }

    [Fact]
    public async Task TestUser()
    {
        var api = SetupApi("USER", "USER.dat");
        await using var session = await CreateSession(api);

        var res = await api.User("erri120");
        TestResult(res, 295, "USER", 1);
        Assert.Equal("943810", res.Lines[0][0]);
        Assert.Equal("erri120", res.Lines[0][1]);
    }

    [Fact]
    public async Task TestAnimeWithId()
    {
        var api = SetupApi("ANIME", "ANIME.dat");
        await using var session = await CreateSession(api);

        var res = await api.Anime(10816);
        TestResult(res, 230, "ANIME", 1);
        Assert.Equal("10816", res.Lines[0][0]);
    }

    [Fact]
    public async Task TestAnimeWithName()
    {
        var api = SetupApi("ANIME", "ANIME.dat");
        await using var session = await CreateSession(api);

        var res = await api.Anime("Overlord");
        TestResult(res, 230, "ANIME", 1);
        Assert.Equal("10816", res.Lines[0][0]);
    }

    /*[Fact]
    public async Task TestAnimeWithMask()
    {
        var mask = (AnimeMask)0xb2f0e0fc000000;

        var api = SetupApi("ANIME", "ANIME-mask.dat");
        await using var session = await CreateSession(api);

        var res = await api.Anime(10816, mask);
        TestResult(res, 230, "ANIME", 1);
    }*/

    [Fact]
    public async Task TestAnimeDesc()
    {
        var api = SetupApi("ANIMEDESC", "ANIMEDESC.dat");
        await using var session = await CreateSession(api);

        var res = await api.AnimeDesc(22, 0);
        TestResult(res, 233, "ANIMEDESC", 1);
        Assert.Equal("0", res.Lines[0][0]);
    }

    [Fact]
    public async Task TestCalendar()
    {
        var api = SetupApi("CALENDAR", "CALENDAR.dat");
        await using var session = await CreateSession(api);

        var res = await api.Calendar();
        TestResult(res, 297, "CALENDAR", 50);
    }

    private static async Task<AuthenticatedSession> CreateSession(UdpApi api)
    {
        var (username, password) = GetUserCredentials();
        var authenticatedSession = await AuthenticatedSession.CreateSession(api, username, password);
        TestSession(authenticatedSession);
        return authenticatedSession;
    }

    private static (string, string) GetUserCredentials()
    {
        return TestUtils.IsCI
            ? ("moq", "moq")
            : (TestUtils.GetEnvironmentVariable("USERNAME"), TestUtils.GetEnvironmentVariable("PASSWORD"));
    }

    private static void TestSession(AuthenticatedSession authenticatedSession)
    {
        Assert.True(authenticatedSession.IsActive);
    }

    private static void TestResult(UdpApiResult result, int returnCode, string returnString, int lineCount = 0)
    {
        Assert.Equal(returnCode, result.ReturnCode);
        Assert.Equal(returnString, result.ReturnString);
        Assert.Equal(lineCount, result.Lines.Count);
    }
}
