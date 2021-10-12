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
        private DateTime _lastTrigger = DateTime.UnixEpoch;

        public RateLimiter(TimeSpan interval)
        {
            _interval = interval;
        }

        public async Task Trigger(CancellationToken cancellationToken = default)
        {
            using (await _lock.LockAsync(cancellationToken))
            {
                var now = DateTime.UtcNow;
                var diff = now - _lastTrigger;

                if (diff > _interval)
                {
                    _lastTrigger = now;
                    return;
                }

                var waitTimeSpan = _interval - diff;
                await Task.Delay(waitTimeSpan, cancellationToken);
                _lastTrigger = DateTime.UtcNow;
            }
        }
    }
}
