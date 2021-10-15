using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public partial class UdpApi
    {
        private readonly RateLimiter _rateLimiter = new(TimeSpan.FromSeconds(4));

        private const string DefaultServer = "api.anidb.net";
        private const int DefaultPort = 9000;
        private const int ProtoVer = 3;

        private readonly ILogger<UdpApi> _logger;
        private readonly IUdpClient _client;
        private readonly string _clientName;
        private readonly int _clientVer;

        public Encoding DataEncoding { get; private set; } = Encoding.ASCII;
        public bool IsAuthenticated { get; private set; }
        public bool IsEncrypted { get; private set; }

        private string? _sessionKey;
        private byte[]? _encryptionKey;

        public UdpApi(ILogger<UdpApi> logger, IUdpClient client, string clientName, int clientVer,
            string server = DefaultServer, int port = DefaultPort)
        {
            _logger = logger;
            _client = client;
            // TODO: name and version validation
            _clientName = clientName;
            _clientVer = clientVer;

            _client.Connect(server, port);
        }

        private async Task<UdpApiResult> SendAndReceive(string commandName, string commandString, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending Command {CommandName}", commandName);
            await _rateLimiter.Trigger(cancellationToken);

            var bytes = DataEncoding.GetBytes(commandString);
            if (_encryptionKey != null && IsEncrypted)
            {
                using var aes = Aes.Create();
                aes.BlockSize = 128;
                aes.Key = _encryptionKey;
                aes.Mode = CipherMode.ECB;
                bytes = aes.EncryptEcb(bytes, PaddingMode.PKCS7);
            }

            // TODO: there is no overload for byte[] that accepts a CancellationToken so this uses SendAsync(ReadOnlyMemory<byte>, CancellationToken)
            var length = await _client.SendAsync(bytes, cancellationToken);
            if (length != bytes.Length)
                return UdpApiResult.CreateInternalError(_logger, $"Unable to send entire Command, only {length.ToString()} bytes out of {bytes.Length.ToString()} have been sent");

            var result = await _client.ReceiveAsync(commandName, cancellationToken);

            var resultBytes = result.Buffer;
            if (_encryptionKey != null && IsEncrypted)
            {
                using var aes = Aes.Create();
                aes.BlockSize = 128;
                aes.Key = _encryptionKey;
                aes.Mode = CipherMode.ECB;
                resultBytes = aes.DecryptEcb(resultBytes, PaddingMode.PKCS7);
            }

            // TODO: find a better solution
            await File.WriteAllBytesAsync($"{commandName}.dat", resultBytes, cancellationToken);
            return CreateResult(resultBytes);
        }

        private byte[] CreateEncryptionKey(string apiKey, string salt)
        {
            var concat = apiKey + salt;
            var bytes = DataEncoding.GetBytes(concat);

            var hash = new byte[16];
            var count = MD5.HashData(new ReadOnlySpan<byte>(bytes, 0, bytes.Length), new Span<byte>(hash, 0, hash.Length));
            if (count != hash.Length)
                throw new NotImplementedException();

            return hash;
        }

        private static string GetStringAfterReturnCode(UdpApiResult result)
        {
            var returnString = result.ReturnString;
            return returnString[..returnString.IndexOf(' ')];
        }

        private string CreateCommandString(string commandName, bool requiresSessionKey, IEnumerable<string?> parameters)
        {
            var sb = new StringBuilder(commandName);
            sb.Append(' ');

            foreach (var parameter in parameters)
            {
                if (parameter == null) continue;
                sb.Append($"&{parameter}");
            }

            if (requiresSessionKey)
            {
                if (!IsAuthenticated || _sessionKey == null)
                    throw new NotImplementedException();
                sb.Append($"&s={_sessionKey}");
            }

            return sb.ToString();
        }

        private string CreateCommandString(string commandName, bool requiresSessionKey, params string?[] parameters)
        {
            return CreateCommandString(commandName, requiresSessionKey, (IEnumerable<string?>) parameters);
        }

        // TODO: internal
        public UdpApiResult CreateResult(byte[] resultBytes)
        {
            //"300 PONG\n"

            var bytes = new ReadOnlySpan<byte>(resultBytes, 0, resultBytes.Length);
            var charCount = DataEncoding.GetCharCount(bytes);
            Span<char> chars = stackalloc char[charCount];

            var actualCount = DataEncoding.GetChars(bytes, chars);
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
