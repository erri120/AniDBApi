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

        public Task<UdpApiResult> Uptime(CancellationToken cancellationToken = default)
            => CreateCommand("UPTIME", cancellationToken);

        public Task<UdpApiResult> User(string user, CancellationToken cancellationToken = default)
            => CreateCommand("USER", cancellationToken, $"user={user}");
    }
}


