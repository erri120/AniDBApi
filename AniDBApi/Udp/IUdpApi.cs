using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi
{
    [PublicAPI]
    public interface IUdpApi
    {
        bool IsAuthenticated { get; }
        bool IsEncrypted { get; }
        DateTime LastApiCall { get; }

        #region Authing

        ValueTask<UdpApiResult> Encrypt(string username, string apiKey, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Auth(string username, string password, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Logout(CancellationToken cancellationToken = default);

        #endregion

        #region Notify

        // TODO: PUSH
        // TODO: NOTIFY
        // TODO: NOTIFYLIST
        // TODO: NOTIFTYGET
        // TODO: NOTIFYACK
        // TODO: PUSHACK

        #endregion

        #region Notification

        ValueTask<UdpApiResult> NotificationAddAnime(int animeId, NotificationType type, NotificationPriority priority, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> NotificationAddGroup(int groupId, NotificationType type, NotificationPriority priority, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> NotificationDelAnime(int animeId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> NotificationDelGroup(int groupId, CancellationToken cancellationToken = default);

        #endregion

        #region Buddy

        ValueTask<UdpApiResult> BuddyAdd(int userId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> BuddyAdd(string username, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> BuddyDel(int userId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> BuddyAccept(int userId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> BuddyDeny(int userId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> BuddyList(int startAt, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> BuddyState(int startAt, CancellationToken cancellationToken = default);

        #endregion

        #region Data

        ValueTask<UdpApiResult> Anime(int id, /*AnimeMask mask,*/ CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Anime(string name, /*AnimeMask mask,*/ CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> AnimeDesc(int id, int part, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Calendar(CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Character(int characterId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Creator(int creatorId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Episode(int episodeId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Episode(string animeName, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Episode(int animeId, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> File(int fileId, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> File(long size, string hash, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> File(string animeName, string groupName, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> File(string animeName, int groupId, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> File(int animeId, string groupName, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> File(int animeId, int groupId, int episodeNumber, FileMask fileMask, FileAnimeMask animeMask, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Group(int groupId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Group(string groupName, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> GroupStatus(int animeId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> GroupStatus(int animeId, GroupStatusCompletionState completionState, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Updated(CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Updated(int age, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Updated(long time, CancellationToken cancellationToken = default);

        #endregion

        #region MyList

        ValueTask<UdpApiResult> MyList(int fileId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyList(long size, string hash, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyList(string animeName, string groupName, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyList(string animeName, int groupId, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyList(int animeId, string groupName, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyList(int animeId, int groupId, int episodeNumber, CancellationToken cancellationToken = default);

        // TODO: MYLISTADD

        ValueTask<UdpApiResult> MyListDel(int fileId, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListDel(long size, string hash, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListDel(string animeName, string groupName, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListDel(string animeName, int groupId, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListDel(int animeId, string groupName, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListDel(int animeId, int groupId, int episodeNumber, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListStats(CancellationToken cancellationToken = default);

        // TODO: VOTE

        ValueTask<UdpApiResult> RandomAnime(RandomAnimeType type, CancellationToken cancellationToken = default);

        #endregion

        #region Misc

        ValueTask<UdpApiResult> MyListExportQueue(string templateName, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> MyListExportCancel(CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Ping(CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Version(CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> Uptime(CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> SendMsg(string username, string title, string body, CancellationToken cancellationToken = default);
        ValueTask<UdpApiResult> User(string user, CancellationToken cancellationToken = default);

        #endregion
    }
}


