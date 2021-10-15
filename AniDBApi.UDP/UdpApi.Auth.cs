using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
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
                $"clientver={_clientVer.ToString()}",
                "enc=UTF8");

            var result = await SendAndReceive("AUTH", commandString, cancellationToken);
            if (result.ReturnCode is 200 or 201)
            {
                _logger.LogInformation("User successfully authenticated");
                IsAuthenticated = true;
                _sessionKey = GetStringAfterReturnCode(result);
                DataEncoding = new UTF8Encoding(false);
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
                return UdpApiResult.CreateMissingSessionError(_logger, "LOGOUT");

            var commandString = CreateCommandString("LOGOUT", true);
            var result = await SendAndReceive("LOGOUT", commandString, cancellationToken);
            if (result.ReturnCode == 203)
            {
                _logger.LogInformation("User successfully logged out");
                IsAuthenticated = false;
                _sessionKey = null;
                DataEncoding = Encoding.ASCII;

                IsEncrypted = false;
                _encryptionKey = null;
            }
            else
            {
                _logger.LogError("Failed to logout with code {ErrorCode}: {Message}", result.ReturnCode.ToString(), result.ReturnString);
            }

            return result;
        }
    }
}


