using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        public Task<UdpApiResult> MyListStats(CancellationToken cancellationToken = default)
            => CreateCommand("MYLISTSTATS", cancellationToken);

        public Task<UdpApiResult> RandomAnime(RandomAnimeType type, CancellationToken cancellationToken = default)
            => CreateCommand("RANDOMANIME", cancellationToken, $"type={((byte)type).ToString()}");
    }
}
