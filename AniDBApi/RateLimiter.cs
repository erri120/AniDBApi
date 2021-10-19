using System;
using System.Threading;
using System.Threading.Tasks;
using NeoSmart.AsyncLock;

namespace AniDBApi
{
    internal class RateLimiter
    {
        private readonly AsyncLock _lock = new();
        private readonly TimeSpan _interval;
        public DateTime LastTrigger { get; private set; } = DateTime.UnixEpoch;

        public RateLimiter(TimeSpan interval)
        {
            _interval = interval;
        }

        public async Task Trigger(CancellationToken cancellationToken = default)
        {
            using (await _lock.LockAsync(cancellationToken))
            {
                var now = DateTime.UtcNow;
                var diff = now - LastTrigger;

                if (diff > _interval)
                {
                    LastTrigger = now;
                    return;
                }

                var waitTimeSpan = _interval - diff + TimeSpan.FromMilliseconds(100);
                await Task.Delay(waitTimeSpan, cancellationToken);
                LastTrigger = DateTime.UtcNow;
            }
        }
    }
}
