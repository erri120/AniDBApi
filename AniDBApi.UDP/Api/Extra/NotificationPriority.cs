using JetBrains.Annotations;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public enum NotificationPriority : byte
    {
        Low = 0,
        Medium = 1,
        High = 2
    }
}
