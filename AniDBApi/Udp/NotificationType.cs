using JetBrains.Annotations;

namespace AniDBApi
{
    [PublicAPI]
    public enum NotificationType : byte
    {
        All = 0,
        New = 1,
        Group = 2,
        Complete = 3
    }
}
