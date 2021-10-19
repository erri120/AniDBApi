using JetBrains.Annotations;

namespace AniDBApi
{
    // TODO: is this a flag?
    [PublicAPI]
    public enum GroupStatusCompletionState : byte
    {
        Ongoing = 1,
        Stalled = 2,
        Complete = 3,
        Dropped = 4,
        Finished = 5,
        SpecialsOnly = 6
    }
}


