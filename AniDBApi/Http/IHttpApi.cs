using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi;

/// <summary>
/// https://wiki.anidb.net/HTTP_API_Definition
/// </summary>
[PublicAPI]
public interface IHttpApi
{
    /// <summary>
    /// Allows retrieval of non-file or episode related information for a specific anime by AID (AniDB anime id).
    /// </summary>
    /// <param name="animeId">AniDB anime id of the anime you want to retrieve data for.</param>
    /// <param name="cancellationToken"></param>
    /// <remarks>Use <see cref="DataDumps.DownloadAnimeTitles"/> if you need the Id but only have the Title.</remarks>
    /// <returns>
    /// Returns the immediate result of the API which should be XML.
    /// Use the <see cref="System.Xml.XmlReader"/> for parsing the result.
    /// </returns>
    Task<string> GetAnime(int animeId, CancellationToken cancellationToken = default);

    /// <summary>
    /// This command mirrors the type of data provided on the main web page. Use this instead of scraping the HTML. Please note, however, that the 'random recommendations' are, in fact, random. Please do not expect random results here to match random results there.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns the immediate result of the API which should be XML.
    /// Use the <see cref="System.Xml.XmlReader"/> for parsing the result.
    /// </returns>
    Task<string> GetRandomRecommendation(CancellationToken cancellationToken = default);

    /// <summary>
    /// This command mirrors the type of data provided on the main web page. Use this instead of scraping the HTML. Please note, however, that the 'random similar' are, in fact, random. Please do not expect random results here to match random results there.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns the immediate result of the API which should be XML.
    /// Use the <see cref="System.Xml.XmlReader"/> for parsing the result.
    /// </returns>
    Task<string> GetRandomSimilar(CancellationToken cancellationToken = default);

    /// <summary>
    /// This command mirrors the type of data provided on the main web page. Use this instead of scraping the HTML. Unlike the two random result commands, the results here will match the results as supplied by the main web page (with some possible variance of a few hours, depending on cache life.)
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns the immediate result of the API which should be XML.
    /// Use the <see cref="System.Xml.XmlReader"/> for parsing the result.
    /// </returns>
    Task<string> GetHotAnime(CancellationToken cancellationToken = default);

    /// <summary>
    /// A one-stop command returning the combined results of random recommendation, random similar, and hot anime. Use this command instead of scraping the HTML, and if you need more than one of the individual replies.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// Returns the immediate result of the API which should be XML.
    /// Use the <see cref="System.Xml.XmlReader"/> for parsing the result.
    /// </returns>
    Task<string> GetMain(CancellationToken cancellationToken = default);
}
