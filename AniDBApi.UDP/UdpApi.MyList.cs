using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        public ValueTask<UdpApiResult> MyList(int fileId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyList(long size, string hash, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyList(string animeName, string groupName, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyList(string animeName, int groupId, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyList(int animeId, string groupName, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyList(int animeId, int groupId, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListDel(int fileId, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListDel(long size, string hash, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListDel(string animeName, string groupName, int episodeNumber,
            CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListDel(string animeName, int groupId, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListDel(int animeId, string groupName, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListDel(int animeId, int groupId, int episodeNumber, CancellationToken cancellationToken = default)
        {
            throw new System.NotImplementedException();
        }

        public ValueTask<UdpApiResult> MyListStats(CancellationToken cancellationToken = default)
            => CreateCommand("MYLISTSTATS", cancellationToken);

        public ValueTask<UdpApiResult> RandomAnime(RandomAnimeType type, CancellationToken cancellationToken = default)
            => CreateCommand("RANDOMANIME", cancellationToken, $"type={((byte)type).ToString()}");
    }
}
