using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace AniDBApi.Tests;

public class RateLimiterTests
{
    [Theory]
    [InlineData(1, 10)]
    [InlineData(5, 3)]
    public async Task TestRateLimiter(int seconds, int taskCount)
    {
        var interval = TimeSpan.FromSeconds(seconds);
        var rateLimiter = new RateLimiter(interval);
        var list = new List<DateTime>();

        async Task AddTime()
        {
            await rateLimiter.Trigger();
            list.Add(DateTime.UtcNow);
        }

        var tasks = Enumerable
            .Range(0, taskCount)
            .Select(_ => Task.Run(AddTime));

        await Task.WhenAll(tasks);

        Assert.Equal(taskCount, list.Count);

        var lastTime = DateTime.UnixEpoch;
        foreach (var curTime in list)
        {
            var timeDiff = curTime - lastTime;
            Assert.True(
                timeDiff >= interval,
                "Difference between current and last task is less than the expected Interval: " +
                $"{curTime:s} - {lastTime:s} < {interval:G}");
            lastTime = curTime;
        }
    }
}
