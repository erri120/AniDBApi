using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        public ValueTask<UdpApiResult> BuddyAdd(int userId, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYADD", cancellationToken, $"uid={userId.ToString()}");

        public ValueTask<UdpApiResult> BuddyAdd(string username, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYADD", cancellationToken, $"uname={username}");

        public ValueTask<UdpApiResult> BuddyDel(int userId, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYDEL", cancellationToken, $"uid={userId.ToString()}");

        public ValueTask<UdpApiResult> BuddyAccept(int userId, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYACCEPT", cancellationToken, $"uid={userId.ToString()}");

        public ValueTask<UdpApiResult> BuddyDeny(int userId, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYDENY", cancellationToken, $"uid={userId.ToString()}");

        public ValueTask<UdpApiResult> BuddyList(int startAt, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYLIST", cancellationToken, $"startat={startAt.ToString()}");

        public ValueTask<UdpApiResult> BuddyState(int startAt, CancellationToken cancellationToken = default)
            => CreateCommand("BUDDYSTATE", cancellationToken, $"startat={startAt.ToString()}");
    }
}
