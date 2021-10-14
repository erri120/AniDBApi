using System;
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
    public class UdpApi
    {
        private readonly RateLimiter _rateLimiter = new(TimeSpan.FromSeconds(3));

        private const string DefaultServer = "api.anidb.net";
        private const int DefaultPort = 9000;
        private const int ProtoVer = 3;

        private readonly ILogger<UdpApi> _logger;
        private readonly IUdpClient _client;
        private readonly string _clientName;
        private readonly int _clientVer;

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

        public Task<UdpApiResult> Ping(CancellationToken cancellationToken = default)
            => SendAndReceive("PING", "PING", cancellationToken);

        public Task<UdpApiResult> Version(CancellationToken cancellationToken = default)
            => SendAndReceive("VERSION", "VERSION", cancellationToken);

        public async Task<UdpApiResult> Encrypt(string username, string apiKey, CancellationToken cancellationToken = default)
        {
            if (IsEncrypted)
                return UdpApiResult.CreateInternalError(_logger, "Session is already encrypted!");

            var commandString = CreateCommandString("ENCRYPT", false,
                $"user={username}", "type=1");

            var result = await SendAndReceive("ENCRYPT", commandString, cancellationToken);
            if (result.ReturnCode is 209)
            {
                _logger.LogInformation("Encryption enabled");
                IsEncrypted = true;
                var salt = GetStringAfterReturnCode(result);
                _encryptionKey = CreateEncryptionKey(apiKey, salt);
            }
            else
            {
                _logger.LogError("Encryption failed with code {ErrorCode}: {Message}", result.ReturnCode.ToString(), result.ReturnString);
            }

            return result;
        }

        public async Task<UdpApiResult> Auth(string username, string password, CancellationToken cancellationToken = default)
        {
            if (IsAuthenticated)
                return UdpApiResult.CreateInternalError(_logger, "User is already authenticated!");

            //AUTH user={str username}&pass={str password}&protover={int4 apiversion}&client={str clientname}&clientver={int4 clientversion}[&nat=1&comp=1&enc={str encoding}&mtu={int4 mtu value}&imgserver=1]
            var commandString = CreateCommandString("AUTH", false,
                $"user={username}",
                $"pass={password}",
                $"protover={ProtoVer.ToString()}",
                $"client={_clientName}",
                $"clientver={_clientVer.ToString()}");

            var result = await SendAndReceive("AUTH", commandString, cancellationToken);
            if (result.ReturnCode is 200 or 201)
            {
                _logger.LogInformation("User successfully authenticated");
                IsAuthenticated = true;
                _sessionKey = GetStringAfterReturnCode(result);
            }
            else
            {
                _logger.LogError("Authentication failed with code {ErrorCode}: {Message}", result.ReturnCode.ToString(), result.ReturnString);
            }

            return result;
        }

        public async Task<UdpApiResult> Logout(CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return UdpApiResult.CreateInternalError(_logger, "User is not authenticated!");

            var commandString = CreateCommandString("LOGOUT", true);
            var result = await SendAndReceive("LOGOUT", commandString, cancellationToken);
            if (result.ReturnCode == 203)
            {
                _logger.LogInformation("User successfully logged out");
                IsAuthenticated = false;
                _sessionKey = null;

                IsEncrypted = false;
                _encryptionKey = null;
            }
            else
            {
                _logger.LogError("Failed to logout with code {ErrorCode}: {Message}", result.ReturnCode.ToString(), result.ReturnString);
            }

            return result;
        }

        public async Task<UdpApiResult> Uptime(CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return UdpApiResult.CreateInternalError(_logger, "Command UPTIME requires a session!");

            var commandString = CreateCommandString("UPTIME", true);
            return await SendAndReceive("UPTIME", commandString, cancellationToken);
        }

        private async Task<UdpApiResult> SendAndReceive(string commandName, string commandString, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending Command {CommandName}", commandName);
            await _rateLimiter.Trigger(cancellationToken);

            var bytes = Encoding.ASCII.GetBytes(commandString);
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

        private static byte[] CreateEncryptionKey(string apiKey, string salt)
        {
            var concat = apiKey + salt;
            var bytes = Encoding.ASCII.GetBytes(concat);

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

        private string CreateCommandString(string commandName, bool requiresSessionKey, params string[] parameters)
        {
            var sb = new StringBuilder(commandName);
            sb.Append(' ');

            foreach (var parameter in parameters)
            {
                sb.Append($"&{parameter}");
            }

            if (requiresSessionKey)
            {
                if (!IsAuthenticated || _sessionKey == null)
                    throw new NotImplementedException();
                sb.Append(parameters.Any() ? $"&s={_sessionKey}" : $"s={_sessionKey}");
            }

            return sb.ToString();
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
