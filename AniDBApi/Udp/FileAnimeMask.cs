using System;
using JetBrains.Annotations;

namespace AniDBApi
{
    [Flags]
    [PublicAPI]
    public enum FileAnimeMask : uint
    {
        // Byte 1
        AnimeTotalEpisodes = 128u << 24,
        HighestEpisodeNumber = 64u << 24,
        Year = 32u << 24,
        Type = 16u << 24,
        RelatedAnimeIdList = 8u << 24,
        RelatedAnimeIdType = 4u << 24,
        CategoryList = 2u << 24,

        // Byte 2
        RomajiName = 128u << 16,
        KanjiName = 64u << 16,
        EnglishName = 32u << 16,
        OtherName = 16u << 16,
        ShortName = 8u << 16,
        SynonymList = 4u << 16,

        // Byte 3
        EpisodeNumber = 128u << 8,
        EpisodeName = 64u << 8,
        EpisodeRomajiName = 32u << 8,
        EpisodeKanjiName = 16u << 8,
        EpisodeRating = 8u << 8,
        EpisodeVoteCount = 4u << 8,

        // Byte 4
        GroupName = 128u,
        GroupShortName = 64u,
        DateRecordUpdated = 1u
    }
}
