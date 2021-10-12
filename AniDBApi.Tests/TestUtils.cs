using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace AniDBApi.Tests;

public static class TestUtils
{
    public static readonly bool IsCI;

    static TestUtils()
    {
        var ciEnv = Environment.GetEnvironmentVariable("CI", EnvironmentVariableTarget.Process);
        if (ciEnv == null) return;
        if (!bool.TryParse(ciEnv, out var isCI))
            IsCI = false;
        IsCI = isCI;
    }

    private static readonly Dictionary<string, string> ResultCache = new(StringComparer.OrdinalIgnoreCase);
    public static string GetResult(string type, string name)
    {
        var path = Path.Combine("files", type, name);
        if (ResultCache.ContainsKey(path))
            return ResultCache[path];

        Assert.True(File.Exists(path), $"Test file {path} does not exist!");
        var result = File.ReadAllText(path);
        ResultCache.Add(path, result);

        return result;
    }
}
