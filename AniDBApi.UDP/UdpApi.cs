using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public class UdpApi
    {
        private readonly RateLimiter _rateLimiter = new(TimeSpan.FromSeconds(3));

        private const string DefaultServer = "api.anidb.net";
        private const int DefaultPort = 9000;

        private static readonly byte[] PingBytes = { 0x50, 0x49, 0x4E, 0x47 };
        private static readonly byte[] VersionBytes = { 0x56, 0x45, 0x52, 0x53, 0x49, 0x4F, 0x4E };

        private readonly ILogger<UdpApi> _logger;
        private readonly IUdpClient _client;

        public UdpApi(ILogger<UdpApi> logger, IUdpClient client, string server = DefaultServer, int port = DefaultPort)
        {
            _logger = logger;
            _client = client;
            _client.Connect(server, port);
        }

        public Task<UdpApiResult> Ping(CancellationToken cancellationToken = default)
            => SendAndReceive("PING", PingBytes, cancellationToken);

        public Task<UdpApiResult> Version(CancellationToken cancellationToken = default)
            => SendAndReceive("VERSION", VersionBytes, cancellationToken);

        private async Task<UdpApiResult> SendAndReceive(string commandName, byte[] bytes, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending Command {CommandName}", commandName);
            await _rateLimiter.Trigger(cancellationToken);

            // TODO: there is no overload for byte[] that accepts a CancellationToken so this uses SendAsync(ReadOnlyMemory<byte>, CancellationToken)
            var length = await _client.SendAsync(bytes, cancellationToken);
            if (length != bytes.Length)
                return UdpApiResult.CreateInternalError(_logger, $"Unable to send entire Command, only {length.ToString()} bytes out of {bytes.Length.ToString()} have been sent");

            var result = await _client.ReceiveAsync(cancellationToken);
            await File.WriteAllBytesAsync($"{commandName}.dat", result.Buffer, cancellationToken);
            return CreateResult(result.Buffer);
        }

        // TODO: internal
        public UdpApiResult CreateResult(byte[] resultBytes)
        {
            //"300 PONG\n"

            var bytes = new ReadOnlySpan<byte>(resultBytes, 0, resultBytes.Length);
            var charCount = Encoding.ASCII.GetCharCount(bytes);
            Span<char> chars = stackalloc char[charCount];

            var actualCount = Encoding.ASCII.GetChars(bytes, chars);
            if (actualCount != charCount)
                return UdpApiResult.CreateInternalError(_logger, $"Number of decoded bytes does not match expected amount: {actualCount.ToString()} != {charCount.ToString()}");

            ReadOnlySpan<char> span = chars;

            var returnCodeSlice = span[..3];
            if (!int.TryParse(returnCodeSlice, NumberStyles.None, null, out var returnCode))
                return UdpApiResult.CreateInternalError(_logger, $"Unable to parse \"{returnCodeSlice.ToString()}\" as int");

            var lineFeedIndex = span.IndexOf('\n');
            if (lineFeedIndex == -1)
                return UdpApiResult.CreateInternalError(_logger, "Return Packet does not contain a linefeed!");

            var returnStringSlice = span.Slice(4, lineFeedIndex - 4);
            var returnString = returnStringSlice.ToString();

            var result = new UdpApiResult(returnCode, returnString);

            // end of the packet
            if (lineFeedIndex == span.Length - 1) return result;

            var consumedChars = lineFeedIndex + 1;
            var nextLineSlice = span.Slice(lineFeedIndex + 1, span.Length - lineFeedIndex - 1);
            lineFeedIndex = nextLineSlice.IndexOf('\n');

            while (lineFeedIndex != -1)
            {
                consumedChars += lineFeedIndex + 1;

                var line = nextLineSlice.Slice(0, lineFeedIndex).ToString();
                var dataFields = line.Split('|', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                result.InternalLines.Add(dataFields.ToList());

                if (consumedChars == span.Length) break;

                nextLineSlice = span.Slice(lineFeedIndex + 1, span.Length - lineFeedIndex - 1);
                lineFeedIndex = nextLineSlice.IndexOf('\n');
            }

            return result;
        }
    }
}
