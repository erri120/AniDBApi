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

    public static string GetResultPath(string type, string name)
    {
        var path = Path.Combine("files", type, name);
        Assert.True(File.Exists(path), $"Test file {path} does not exist!");
        return path;
    }
}
