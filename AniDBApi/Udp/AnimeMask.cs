using System;
using JetBrains.Annotations;

namespace AniDBApi
{
    // TODO: API Definition https://wiki.anidb.net/UDP_API_Definition#ANIME:_Retrieve_Anime_Data is outdated and mask values are wrong
    [Flags]
    [PublicAPI]
    public enum AnimeMask : ulong
    {
        // Byte 1
        AId = 128UL << 48,
        DateFlags = 64UL << 48,
        Year = 32UL << 48,
        Type = 16UL << 48,
        RelatedAIdList = 8UL << 48,
        RelatedAIdType = 4UL << 48,
        //Categories = 2UL << 48,

        // Byte 2
        RomajiName = 128UL << 40,
        KanjiName = 64UL << 40,
        EnglishName = 32UL << 40,
        OtherName = 16UL << 40,
        ShortNameList = 8UL << 40,
        SynonymList = 4UL << 40,

        // Byte 3
        Episodes = 128UL << 32,
        HighestEpisodeNumber = 64UL << 32,
        SpecialEpCount = 32UL << 32,
        AirDate = 16UL << 32,
        EndDate = 8UL << 32,
        Url = 4UL << 32,
        PicName = 2UL << 32,

        // Byte 4
        Rating = 128UL << 24,
        VoteCount = 64UL << 24,
        TempRating = 32UL << 24,
        TempVoteCount = 16UL << 24,
        AverageReviewRating = 8UL << 24,
        ReviewCount = 4UL << 24,
        AwardList = 2UL << 24,
        IsAdultRestricted = 1UL << 24,

        // Byte 5
        ANNId = 64UL << 16,
        AllCinemaId = 32UL << 16,
        AnimeNfoId = 16UL << 16,
        TagNameList = 8UL << 16,
        TagIdList = 4UL << 16,
        TagWeightList = 2UL << 16,
        DateRecordUpdated = 1UL << 16,

        // Byte 6
        CharacterIdList = 128UL << 8,

        // Byte 7
        SpecialsCount = 128UL,
    }
}

