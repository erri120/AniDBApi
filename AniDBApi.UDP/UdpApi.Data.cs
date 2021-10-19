using System.Threading;
using System.Threading.Tasks;

namespace AniDBApi.UDP
{
    public partial class UdpApi
    {
        //public const AnimeMask DefaultMask = AnimeMask.AId | ;

        /*public const AnimeMask AllMask = AnimeMask.AId | AnimeMask.DateFlags | AnimeMask.Year | AnimeMask.Type |
                                         AnimeMask.RelatedAIdList | AnimeMask.RelatedAIdType | AnimeMask.RomajiName |
                                         AnimeMask.KanjiName | AnimeMask.EnglishName | AnimeMask.OtherName |
                                         AnimeMask.ShortNameList | AnimeMask.SynonymList | AnimeMask.Episodes |
                                         AnimeMask.HighestEpisodeNumber | AnimeMask.SpecialEpCount | AnimeMask.AirDate |
                                         AnimeMask.EndDate | AnimeMask.Url | AnimeMask.PicName | AnimeMask.Rating |
                                         AnimeMask.VoteCount | AnimeMask.TempRating | AnimeMask.TempVoteCount |
                                         AnimeMask.AverageReviewRating | AnimeMask.ReviewCount | AnimeMask.AwardList |
                                         AnimeMask.IsAdultRestricted | AnimeMask.ANNId | AnimeMask.AllCinemaId |
                                         AnimeMask.AnimeNfoId | AnimeMask.TagNameList | AnimeMask.TagIdList |
                                         AnimeMask.TagWeightList | AnimeMask.DateRecordUpdated |
                                         AnimeMask.CharacterIdList | AnimeMask.SpecialsCount | AnimeMask.CreditsCount |
                                         AnimeMask.OtherCount | AnimeMask.TrailerCount | AnimeMask.ParodyCount;*/

        public async ValueTask<UdpApiResult> Anime(int id, /*AnimeMask mask,*/ CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return UdpApiResult.CreateMissingSessionError(_logger, "ANIME");

            /*var commandString = mask == DefaultMask
                ? CreateCommandString("ANIME", true, $"aid={id.ToString()}")
                : CreateCommandString("ANIME", true, $"aid={id.ToString()}", $"amask={mask:X}");*/
            //var commandString = CreateCommandString("ANIME", true, $"aid={id.ToString()}", $"amask={mask:X}");

            var commandString = CreateCommandString("ANIME", true, $"aid={id.ToString()}");
            return await SendAndReceive("ANIME", commandString, cancellationToken);
        }

        public async ValueTask<UdpApiResult> Anime(string name, /*AnimeMask mask,*/ CancellationToken cancellationToken = default)
        {
            if (!IsAuthenticated)
                return UdpApiResult.CreateMissingSessionError(_logger, "ANIME");

            /*var commandString = mask == DefaultMask
                ? CreateCommandString("ANIME", true, $"aname={name}")
                : CreateCommandString("ANIME", true, $"aname={name}", $"amask={mask:X}");*/
            //var commandString = CreateCommandString("ANIME", true, $"aname={name}", $"amask={mask:X}");

            // TODO: Anime by name is broken, returns 330 NO SUCH ANIME for "Overlord"...
            var commandString = CreateCommandString("ANIME", true, $"aname={name}");
            return await SendAndReceive("ANIME", commandString, cancellationToken);
        }

        public ValueTask<UdpApiResult> AnimeDesc(int id, int part, CancellationToken cancellationToken = default)
            => CreateCommand("ANIMEDESC", cancellationToken, $"aid={id.ToString()}", $"part={part.ToString()}");

        public ValueTask<UdpApiResult> Calendar(CancellationToken cancellationToken = default)
            => CreateCommand("CALENDAR", cancellationToken);

        public ValueTask<UdpApiResult> Character(int characterId, CancellationToken cancellationToken = default)
            => CreateCommand("CHARACTER", cancellationToken, $"charid={characterId.ToString()}");

        public ValueTask<UdpApiResult> Creator(int creatorId, CancellationToken cancellationToken = default)
            => CreateCommand("CREATOR", cancellationToken, $"creatorid={creatorId.ToString()}");

        public ValueTask<UdpApiResult> Episode(int episodeId, CancellationToken cancellationToken = default)
            => CreateCommand("EPISODE", cancellationToken, $"eid={episodeId.ToString()}");

        public ValueTask<UdpApiResult> Episode(string animeName, int episodeNumber, CancellationToken cancellationToken = default)
            => CreateCommand("EPISODE", cancellationToken, $"aname={animeName}", $"epno={episodeNumber.ToString()}");

        public ValueTask<UdpApiResult> Episode(int animeId, int episodeNumber, CancellationToken cancellationToken = default)
            => CreateCommand("EPISODE", cancellationToken, $"aid={animeId.ToString()}", $"epno={episodeNumber.ToString()}");

        #region File Command

        public ValueTask<UdpApiResult> File(int fileId, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default)
            => CreateCommand("FILE", cancellationToken, $"fid={fileId.ToString()}", $"fmask={fileMask.ToString("X")[6..]}", $"amask={animeMask:X}");

        public ValueTask<UdpApiResult> File(long size, string hash, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default)
            => CreateCommand("FILE", cancellationToken, $"size={size.ToString()}", $"ed2k={hash}", $"fmask={fileMask.ToString("X")[6..]}", $"amask={animeMask:X}");

        public ValueTask<UdpApiResult> File(string animeName, string groupName, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default)
            => CreateCommand("FILE", cancellationToken, $"aname={animeName}", $"gname={groupName}", $"epno={episodeNumber.ToString()}", $"fmask={fileMask.ToString("X")[6..]}", $"amask={animeMask:X}");

        public ValueTask<UdpApiResult> File(string animeName, int groupId, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default)
            => CreateCommand("FILE", cancellationToken, $"aname={animeName}", $"gid={groupId.ToString()}", $"epno={episodeNumber.ToString()}", $"fmask={fileMask.ToString("X")[6..]}", $"amask={animeMask:X}");

        public ValueTask<UdpApiResult> File(int animeId, string groupName, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default)
            => CreateCommand("FILE", cancellationToken, $"aid={animeId.ToString()}", $"gname={groupName}", $"epno={episodeNumber.ToString()}", $"fmask={fileMask.ToString("X")[6..]}", $"amask={animeMask:X}");

        public ValueTask<UdpApiResult> File(int animeId, int groupId, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default)
            => CreateCommand("FILE", cancellationToken, $"aid={animeId.ToString()}", $"gid={groupId.ToString()}", $"epno={episodeNumber.ToString()}", $"fmask={fileMask.ToString("X")[6..]}", $"amask={animeMask:X}");

        #endregion

        public ValueTask<UdpApiResult> Group(int groupId, CancellationToken cancellationToken = default)
            => CreateCommand("GROUP", cancellationToken, $"gid={groupId.ToString()}");

        public ValueTask<UdpApiResult> Group(string groupName, CancellationToken cancellationToken = default)
            => CreateCommand("GROUP", cancellationToken, $"gname={groupName}");

        public ValueTask<UdpApiResult> GroupStatus(int animeId, CancellationToken cancellationToken = default)
            => CreateCommand("GROUPSTATUS", cancellationToken, $"aid={animeId.ToString()}");

        public ValueTask<UdpApiResult> GroupStatus(int animeId, GroupStatusCompletionState completionState, CancellationToken cancellationToken = default)
            => CreateCommand("GROUPSTATUS", cancellationToken, $"aid={animeId.ToString()}", $"state={((int)completionState).ToString()}");

        public ValueTask<UdpApiResult> Updated(CancellationToken cancellationToken = default)
            => CreateCommand("UPDATED", cancellationToken, "entity=1");

        public ValueTask<UdpApiResult> Updated(int age, CancellationToken cancellationToken = default)
            => CreateCommand("UPDATED", cancellationToken, "entity=1", $"age={age.ToString()}");

        public ValueTask<UdpApiResult> Updated(long time, CancellationToken cancellationToken = default)
            => CreateCommand("UPDATED", cancellationToken, "entity=1", $"time={time.ToString()}");
    }
}
