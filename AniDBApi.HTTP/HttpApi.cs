using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AniDBApi.HTTP
{
    /// <inheritdoc cref="IHttpApi"/>
    [PublicAPI]
    public sealed class HttpApi : IHttpApi
    {
        private const string ApiBaseUrl = "http://api.anidb.net:9001/httpapi";

        private readonly RateLimiter _rateLimiter = new(TimeSpan.FromSeconds(2));

        private readonly ILogger<HttpApi> _logger;
        private readonly HttpClient _client;
        private readonly string _baseUrl;

        private static readonly Regex ClientNameRegex = new("^[a-z]*$", RegexOptions.Compiled);

        /// <summary></summary>
        /// <param name="logger">Logger for logging all Api calls.</param>
        /// <param name="client">HttpClient for accessing the Api.</param>
        /// <param name="clientName">Name of the Client. You need to register your Client with AniDB first. The name has a max length of 16 characters and must only contain chars from a-z.</param>
        /// <param name="clientVer">Version of the Client. You need to register your Client with AniDB first. The version must be between 0 and 9.</param>
        /// <param name="baseUrl">Alternative Base Url for the Api.</param>
        /// <exception cref="ArgumentException">The name or version of the Client is invalid.</exception>
        public HttpApi(ILogger<HttpApi> logger, HttpClient client, string clientName, int clientVer,
            string baseUrl = ApiBaseUrl)
        {
            if (clientVer is < 0 or > 9)
                throw new ArgumentException($"Invalid client version: {clientVer} (must be between 0 and 9)", nameof(clientVer));
            if (clientName.Length > 16)
                throw new ArgumentException($"Invalid client name: \"{clientName}\" (name is longer than 16 characters)", nameof(clientName));
            if (!ClientNameRegex.IsMatch(clientName))
                throw new ArgumentException($"Invalid client name: \"{clientName}\" (name does not match regex \"^[a-z]*$\")", nameof(clientName));

            _logger = logger;
            _client = client;
            _baseUrl = $"{baseUrl}?client={clientName}&clientver={clientVer.ToString()}&protover=1";
        }

        /// <inheritdoc />
        public Task<string> GetAnime(int animeId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting Anime with Id {id}", animeId.ToString());
            var url = CreateUrl("anime", $"aid={animeId.ToString()}");
            return GetFromApi(url, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string> GetRandomRecommendation(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting Random Recommendations");
            var url = CreateUrl("randomrecommendation");
            return GetFromApi(url, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string> GetRandomSimilar(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting Random Similar");
            var url = CreateUrl("randomsimilar");
            return GetFromApi(url, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string> GetHotAnime(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting Hot Anime");
            var url = CreateUrl("hotanime");
            return GetFromApi(url, cancellationToken);
        }

        /// <inheritdoc />
        public Task<string> GetMain(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Getting Main");
            var url = CreateUrl("main");
            return GetFromApi(url, cancellationToken);
        }

        private async Task<string> GetFromApi(string url, CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Accessing {url}", url);

            // TODO: error checking
            //<error code="302">client version missing or invalid</error>

            await _rateLimiter.Trigger(cancellationToken).ConfigureAwait(false);
            var result = await _client.GetStringAsync(url, cancellationToken).ConfigureAwait(false);
            return result;
        }

        private string CreateUrl(string request, params string[] otherParams)
        {
            var sb = new StringBuilder(_baseUrl);

            sb.Append($"&request={request}");
            foreach (var param in otherParams)
            {
                sb.Append('&');
                sb.Append(param);
            }

            return sb.ToString();
        }
    }
}
