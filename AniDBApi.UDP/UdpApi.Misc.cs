using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        public Task<UdpApiResult> Ping(CancellationToken cancellationToken = default)
            => SendAndReceive("PING", "PING", cancellationToken);

        public Task<UdpApiResult> Version(CancellationToken cancellationToken = default)
            => SendAndReceive("VERSION", "VERSION", cancellationToken);

        public async Task<UdpApiResult> Uptime(CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return UdpApiResult.CreateMissingSessionError(_logger, "UPTIME");

            var commandString = CreateCommandString("UPTIME", true);
            return await SendAndReceive("UPTIME", commandString, cancellationToken);
        }

        public async Task<UdpApiResult> User(string user, CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return UdpApiResult.CreateMissingSessionError(_logger, "USER");

            var commandString = CreateCommandString("USER", true, $"user={user}");
            return await SendAndReceive("USER", commandString, cancellationToken);
        }
    }
}


