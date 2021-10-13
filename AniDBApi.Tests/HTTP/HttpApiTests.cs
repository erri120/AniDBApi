using System.Threading.Tasks;
using AniDBApi.HTTP;
using Microsoft.Extensions.Logging;
using Moq.Contrib.HttpClient;
using Xunit;
using Xunit.Abstractions;

namespace AniDBApi.Tests.HTTP;

public class HttpApiTests
{
    private readonly ILogger<HttpApi> _logger;

    public HttpApiTests(ITestOutputHelper testOutputHelper)
    {
        _logger = new XUnitLogger<HttpApi>(testOutputHelper);
    }

    private HttpApi SetupApi(string fileName)
    {
        return TestUtils.IsCI ? HttpTestUtils.SetupApiFakeClient(_logger, handler =>
        {
            handler.SetupAnyRequest().ReturnsResponse(TestUtils.GetResult("http", fileName));
        }) : HttpTestUtils.SetupApiRealClient(_logger);
    }

    [Fact]
    public async Task TestGetAnime()
    {
        var api = SetupApi("GetAnime-22.xml");
        var result = await api.GetAnime(22);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TestGetRandomRecommendation()
    {
        var api = SetupApi("GetRandomRecommendation.xml");
        var result = await api.GetRandomRecommendation();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TestGetRandomSimilar()
    {
        var api = SetupApi("GetRandomSimilar.xml");
        var result = await api.GetRandomSimilar();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TestGetHotAnime()
    {
        var api = SetupApi("GetHotAnime.xml");
        var result = await api.GetHotAnime();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task TestGetMain()
    {
        var api = SetupApi("GetMain.xml");
        var result = await api.GetMain();
        Assert.NotNull(result);
    }
}
