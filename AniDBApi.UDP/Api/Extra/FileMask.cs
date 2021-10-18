using System;
using JetBrains.Annotations;

namespace AniDBApi.UDP
{
    [Flags]
    [PublicAPI]
    public enum FileMask : ulong
    {
        // Byte 1
        AnimeId = 64UL << 32,
        EpisodeId = 32UL << 32,
        GroupId = 16UL << 32,
        MyListId = 8UL << 32,
        OtherEpisodes = 4UL << 32,
        IsDeprecated = 2UL << 32,
        State = 1UL << 32,

        // Byte 2
        Size = 128UL << 24,
        Ed2k = 64UL << 24,
        MD5 = 32UL << 24,
        SHA1 = 16UL << 24,
        CRC32 = 8UL << 24,
        VideoColorDepth = 2UL << 24,

        // Byte 3
        Quality = 128UL << 16,
        Source = 64UL << 16,
        AudioCodecList = 32UL << 16,
        AudioBitrateList = 16UL << 16,
        VideoCodec = 8UL << 16,
        VideoBitrate = 4UL << 16,
        VideoResolution = 2UL << 16,
        FileType = 1UL << 16,

        // Byte 4
        DubLanguage = 128UL << 8,
        SubLanguage = 64UL << 8,
        LengthInSeconds = 32UL << 8,
        Description = 16UL << 8,
        AiredDate = 8UL << 8,
        AniDbFileName = 1UL << 8,

        // Byte 5
        MyListState = 128UL,
        MyListFileState = 64UL,
        MyListViewed = 32UL,
        MyListViewDate = 16UL,
        MyListStorage = 8UL,
        MyListSource = 4UL,
        MyListOther = 2UL
    }
}


