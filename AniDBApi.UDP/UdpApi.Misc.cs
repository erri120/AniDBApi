using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        public ValueTask<UdpApiResult> MyListExportQueue(string templateName, CancellationToken cancellationToken = default)
            => CreateCommand("MYLISTEXPORT", cancellationToken, $"template={templateName}");

        public ValueTask<UdpApiResult> MyListExportCancel(CancellationToken cancellationToken = default)
            => CreateCommand("MYLISTEXPORT", cancellationToken, "cancel=1");

        public ValueTask<UdpApiResult> Ping(CancellationToken cancellationToken = default)
            => SendAndReceive("PING", "PING", cancellationToken);

        public ValueTask<UdpApiResult> Version(CancellationToken cancellationToken = default)
            => SendAndReceive("VERSION", "VERSION", cancellationToken);

        public ValueTask<UdpApiResult> Uptime(CancellationToken cancellationToken = default)
            => CreateCommand("UPTIME", cancellationToken);

        public async ValueTask<UdpApiResult> SendMsg(string username, string title, string body, CancellationToken cancellationToken = default)
        {
            if (title.Length >= 50)
                return UdpApiResult.CreateInternalError(_logger, "Title must not be longer than 50 chars!");
            if (body.Length >= 900)
                return UdpApiResult.CreateInternalError(_logger, "Body must not be longer than 900 chars!");

            return await CreateCommand("SENDMSG", cancellationToken, $"to={username}", $"title={title}", $"body={body}");
        }

        public ValueTask<UdpApiResult> User(string user, CancellationToken cancellationToken = default)
            => CreateCommand("USER", cancellationToken, $"user={user}");
    }
}


