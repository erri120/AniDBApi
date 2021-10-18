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

        public async Task<UdpApiResult> SendMsg(string username, string title, string body, CancellationToken cancellationToken = default)
        {
            if (title.Length >= 50)
                return UdpApiResult.CreateInternalError(_logger, "Title must not be longer than 50 chars!");
            if (body.Length >= 900)
                return UdpApiResult.CreateInternalError(_logger, "Body must not be longer than 900 chars!");

            return await CreateCommand("SENDMSG", cancellationToken, $"to={username}", $"title={title}", $"body={body}");
        }

        public Task<UdpApiResult> User(string user, CancellationToken cancellationToken = default)
            => CreateCommand("USER", cancellationToken, $"user={user}");
    }
}


