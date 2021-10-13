using AniDBApi.HTTP;
using Xunit;

namespace AniDBApi.Tests.HTTP;

public class HttpApiUtilsTests
{
    [Fact]
    public void TestIsError()
    {
        const string input = @"<error code=""302"">client version missing or invalid</error>";

        Assert.True(HttpApiUtils.IsError(input, out var error));
        Assert.NotNull(error);
        Assert.Equal("client version missing or invalid", error);
    }
}
