using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        public ValueTask<UdpApiResult> NotificationAddAnime(int animeId, NotificationType type, NotificationPriority priority, CancellationToken cancellationToken = default)
            => CreateCommand("NOTIFICATIONADD", cancellationToken, $"aid={animeId.ToString()}", $"type={((byte)type).ToString()}", $"priority={((byte)priority).ToString()}");

        public ValueTask<UdpApiResult> NotificationAddGroup(int groupId, NotificationType type, NotificationPriority priority, CancellationToken cancellationToken = default)
            => CreateCommand("NOTIFICATIONADD", cancellationToken, $"gid={groupId.ToString()}", $"type={((byte)type).ToString()}", $"priority={((byte)priority).ToString()}");

        public ValueTask<UdpApiResult> NotificationDelAnime(int animeId, CancellationToken cancellationToken = default)
            => CreateCommand("NOTIFICATIONDEL", cancellationToken, $"aid={animeId.ToString()}");

        public ValueTask<UdpApiResult> NotificationDelGroup(int groupId, CancellationToken cancellationToken = default)
            => CreateCommand("NOTIFICATIONDEL", cancellationToken, $"gid={groupId.ToString()}");
    }
}
