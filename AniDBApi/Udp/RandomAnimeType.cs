using JetBrains.Annotations;

namespace AniDBApi
{
    [PublicAPI]
    public enum RandomAnimeType : byte
    {
        FromDb = 0,
        Watched = 1,
        Unwatched = 2,
        AllMyList = 3
    }
}
