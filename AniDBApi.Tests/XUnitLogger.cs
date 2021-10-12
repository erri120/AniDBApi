using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace AniDBApi.Tests;

public class XUnitLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _testOutputHelper;

    public XUnitLogger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        _testOutputHelper.WriteLine($"{DateTime.UtcNow:O}|{logLevel}: {formatter(state, exception)}");
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state)
    {
        throw new NotImplementedException();
    }
}
