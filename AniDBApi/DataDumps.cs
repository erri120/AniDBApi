using System;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi;

/// <summary>
/// Provides functions for downloading the daily AniDB data dumps.
/// </summary>
[PublicAPI]
public static class DataDumps
{
    private static RateLimiter _rateLimiter = new(TimeSpan.FromDays(1));
    private const string AnimeTitlesUrl = "http://anidb.net/api/anime-titles.xml.gz";

    /// <summary>
    /// Downloads the daily data dump of all anime titles. This has a Rate Limit of 1 Day.
    /// </summary>
    /// <remarks>The data format is XML.</remarks>
    /// <param name="path">Path to the output file.</param>
    /// <param name="cancellationToken"></param>
    public static async Task DownloadAnimeTitles(string path, CancellationToken cancellationToken = default)
    {
        await _rateLimiter.Trigger(cancellationToken);

        using var client = new HttpClient();
        await using var stream = await client.GetStreamAsync(AnimeTitlesUrl, cancellationToken);
        await using var gzipStream = new GZipStream(stream, CompressionMode.Decompress);
        await using var fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read);
        await gzipStream.CopyToAsync(fs, cancellationToken);
    }
}
