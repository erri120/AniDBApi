using System;
using System.Collections.Generic;
using System.Linq;
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
    public Task TestUptime()
    {
        return TestSimpleCommand(
            "UPTIME",
            null,
            api => api.Uptime(),
            208,
            null,
            1);
    }

    [Fact]
    public Task TestUser()
    {
        return TestSimpleCommand(
            "USER",
            null,
            api => api.User("erri120"),
            295,
            null,
            1,
            res =>
            {
                Assert.Equal("943810", res.Lines[0][0]);
                Assert.Equal("erri120", res.Lines[0][1]);
            });
    }

    [Fact]
    public Task TestAnimeWithId()
    {
        return TestSimpleCommand(
            "ANIME",
            null,
            api => api.Anime(10816),
            230,
            null,
            1,
            res => Assert.Equal("10816", res.Lines[0][0]));
    }

    [Fact]
    public Task TestAnimeWithName()
    {
        return TestSimpleCommand(
            "ANIME",
            null,
            api => api.Anime("Overlord"),
            230,
            null,
            1,
            res => Assert.Equal("10816", res.Lines[0][0]));
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
    public Task TestAnimeDesc()
    {
        return TestSimpleCommand(
            "ANIMEDESC",
            null,
            api => api.AnimeDesc(22, 0),
            233,
            null,
            1,
            res => Assert.Equal("0", res.Lines[0][0]));
    }

    [Fact]
    public Task TestCalendar()
    {
        return TestSimpleCommand(
            "CALENDAR",
            null,
            api => api.Calendar(),
            297,
            null,
            50);
    }

    [Fact]
    public Task TestCharacter()
    {
        return TestSimpleCommand(
            "CHARACTER",
            null,
            api => api.Character(1905),
            235,
            null,
            1,
            res => Assert.Equal("1905", res.Lines[0][0]));
    }

    [Fact]
    public Task TestCreator()
    {
        return TestSimpleCommand(
            "CREATOR",
            null,
            api => api.Creator(666),
            245,
            null,
            1,
            res => Assert.Equal("666", res.Lines[0][0]));
    }

    [Fact]
    public Task TestEpisodeWithId()
    {
        return TestSimpleCommand(
            "EPISODE",
            null,
            api => api.Episode(235753),
            240,
            null,
            1,
            res => Assert.Equal("235753", res.Lines[0][0]));
    }

    [Fact]
    public Task TestEpisodeWithAnimeId()
    {
        return TestSimpleCommand(
            "EPISODE",
            null,
            api => api.Episode(14977, 5),
            240,
            null,
            1,
            res => Assert.Equal("235753", res.Lines[0][0]));
    }

    [Fact]
    public Task TestEpisodeWithAnimeName()
    {
        return TestSimpleCommand(
            "EPISODE",
            null,
            api => api.Episode("Attack on Titan The Final Season", 5),
            240,
            null,
            1,
            res => Assert.Equal("235753", res.Lines[0][0]));
    }

    [Fact]
    public Task TestFileWithId()
    {
        Assert.True(TestUtils.IsCI, "You can get banned very quickly, don't use this in a non-CI context.");

        return TestSimpleCommand(
            "FILE",
            "FILE-id",
            api => api.File(2603934, FileMask.Ed2k, FileAnimeMask.EnglishName),
            220,
            null,
            1,
            res => Assert.Equal("2603934", res.Lines[0][0]));
    }

    [Fact]
    public Task TestFileWithSizeAndHash()
    {
        Assert.True(TestUtils.IsCI, "You can get banned very quickly, don't use this in a non-CI context.");

        return TestSimpleCommand(
            "FILE",
            "FILE-size&hash",
            api => api.File(7262144789, "b036c4c4de9522e96e235a5a797d8c15", FileMask.Ed2k, FileAnimeMask.EnglishName),
            220,
            null,
            1,
            res => Assert.Equal("2849075", res.Lines[0][0]));
    }

    [Fact]
    public Task TestFileWithANameGNameEpNo()
    {
        Assert.True(TestUtils.IsCI, "You can get banned very quickly, don't use this in a non-CI context.");

        return TestSimpleCommand(
            "FILE",
            "FILE-aname&gname&epno",
            api => api.File("Violet Evergarden the Movie", "EMBER", 1, FileMask.Ed2k, FileAnimeMask.EnglishName),
            220,
            null,
            1,
            res => Assert.Equal("2869654", res.Lines[0][0]));
    }

    [Fact]
    public Task TestFileWithANameGIdEpNo()
    {
        Assert.True(TestUtils.IsCI, "You can get banned very quickly, don't use this in a non-CI context.");

        return TestSimpleCommand(
            "FILE",
            "FILE-aname&gid&epno",
            api => api.File("Cowboy Bebop", 16576, 1, FileMask.Ed2k, FileAnimeMask.EnglishName),
            220,
            null,
            1,
            res => Assert.Equal("2755871", res.Lines[0][0]));
    }

    [Fact]
    public Task TestFileWithAIdGNameEpNo()
    {
        Assert.True(TestUtils.IsCI, "You can get banned very quickly, don't use this in a non-CI context.");

        return TestSimpleCommand(
            "FILE",
            "FILE-aid&gname&epno",
            api => api.File(6529, "hydes", 1, FileMask.Ed2k, FileAnimeMask.EnglishName),
            220,
            null,
            1,
            res => Assert.Equal("2755844", res.Lines[0][0]));
    }

    [Fact]
    public Task TestFileWithAIdGIdEpNo()
    {
        Assert.True(TestUtils.IsCI, "You can get banned very quickly, don't use this in a non-CI context.");

        return TestSimpleCommand(
            "FILE",
            "FILE-aid&gid&epno",
            api => api.File(28, 16576, 1, FileMask.Ed2k, FileAnimeMask.EnglishName),
            220,
            null,
            1,
            res => Assert.Equal("2714876", res.Lines[0][0]));
    }

    [Fact]
    public Task TestGroupWithId()
    {
        return TestSimpleCommand(
            "GROUP",
            "GROUP",
            api => api.Group(16576),
            250,
            null,
            1,
            res => Assert.Equal("16576", res.Lines[0][0]));
    }

    [Fact]
    public Task TestGroupWithName()
    {
        return TestSimpleCommand(
            "GROUP",
            "GROUP",
            api => api.Group("hydes"),
            250,
            null,
            1,
            res => Assert.Equal("16576", res.Lines[0][0]));
    }

    [Fact]
    public Task TestGroupStatus()
    {
        return TestSimpleCommand(
            "GROUPSTATUS",
            "GROUPSTATUS",
            api => api.GroupStatus(7729),
            225,
            null,
            42);
    }

    [Fact]
    public Task TestGroupStatusWithStatus()
    {
        return TestSimpleCommand(
            "GROUPSTATUS",
            "GROUPSTATUS-status",
            api => api.GroupStatus(7729, GroupStatusCompletionState.Complete),
            225,
            null,
            42);
    }

    [Fact]
    public Task TestUpdated()
    {
        return TestSimpleCommand(
            "UPDATED",
            "UPDATED",
            api => api.Updated(),
            243,
            null,
            1,
            res => Assert.Equal("1", res.Lines[0][0]));
    }

    [Fact]
    public Task TestSendMsg()
    {
        var (username, _) = GetUserCredentials();

        return TestSimpleCommand(
            "SENDMSG",
            null,
            api => api.SendMsg(username, "Message to self", "Hello myself!"),
            294,
            "SENDMSG SUCCESSFUL");
    }

    [Fact]
    public Task TestNotificationAdd()
    {
        return TestSimpleCommand(
            "NOTIFICATIONADD",
            null,
            api => api.NotificationAddGroup(16576, NotificationType.All, NotificationPriority.High),
            246,
            "NOTIFICATION ITEM ADDED",
            1);
    }

    [Fact]
    public Task TestNotificationDel()
    {
        return TestSimpleCommand(
            "NOTIFICATIONDEL",
            null,
            api => api.NotificationDelGroup(16576),
            247,
            "NOTIFICATION ITEM DELETED",
            1);
    }

    [Fact]
    public Task TestMyListExport()
    {
        return TestSimpleCommand(
            "MYLISTEXPORT",
            null,
            api => api.MyListExportQueue("csv-adborg"),
            217,
            "MYLIST EXPORT QUEUED");
    }

    [Fact]
    public Task TestMyListExportCancel()
    {
        return TestSimpleCommand(
            "MYLISTEXPORT",
            "MYLISTEXPORT-cancel",
            api => api.MyListExportCancel(),
            218,
            "EXPORT CANCELLED");
    }

    [Fact]
    public Task TestMyListStats()
    {
        return TestSimpleCommand(
            "MYLISTSTATS",
            null,
            api => api.MyListStats(),
            222,
            "MYLIST STATS",
            1);
    }

    [Fact]
    public Task TestRandomAnime()
    {
        return TestSimpleCommand(
            "RANDOMANIME",
            null,
            api => api.RandomAnime(RandomAnimeType.FromDb),
            230,
            "ANIME",
            1);
    }

    private async Task TestSimpleCommand(string commandName, string? resultFile, Func<UdpApi, ValueTask<UdpApiResult>> func,
        int returnCode, string? returnString, int lineCount = 0, Action<UdpApiResult>? action = null)
    {
        var api = SetupApi(commandName, resultFile == null ? $"{commandName}.dat" : $"{resultFile}.dat");
        await using var session = await CreateSession(api);

        var res = await func(api);
        TestResult(res, returnCode, returnString ?? commandName, lineCount);
        action?.Invoke(res);
    }

    private static async Task<AuthenticatedSession> CreateSession(IUdpApi api)
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
        Assert.True(authenticatedSession.IsActive,
            "Authentication failed: " +
            $"{authenticatedSession.AuthResult?.ReturnCode.ToString()} " +
            $"{authenticatedSession.AuthResult?.ReturnString}");
    }

    private static void TestResult(UdpApiResult result, int returnCode, string returnString, int lineCount = 0)
    {
        var lines = result.Lines.SelectMany(x => x).ToList();
        Assert.False(result.ReturnCode == 555, $"{result.ReturnString}: {(lines.Any() ? lines.Aggregate((a,b) => $"{a} {b}") : "")}");

        Assert.Equal(returnCode, result.ReturnCode);
        Assert.Equal(returnString, result.ReturnString);
        Assert.Equal(lineCount, result.Lines.Count);
    }
}
