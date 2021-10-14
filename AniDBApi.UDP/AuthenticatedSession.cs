using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public class AuthenticatedSession : IDisposable, IAsyncDisposable
    {
        private readonly UdpApi _api;

        public UdpApiResult? AuthResult { get; private set; }
        public bool IsActive => _api.IsAuthenticated;

        internal AuthenticatedSession(UdpApi api)
        {
            _api = api;
        }

        public static async Task<AuthenticatedSession> CreateSession(UdpApi api, string username, string password, CancellationToken cancellationToken = default)
        {
            var session = new AuthenticatedSession(api);
            session.AuthResult = await api.Auth(username, password, cancellationToken);
            return session;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (_api.IsAuthenticated)
            {
                // TODO: maybe do something with this
                var res = _api.Logout().Result;
            }
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            if (_api.IsAuthenticated)
            {
                // TODO: maybe do something with this
                var res = await _api.Logout().ConfigureAwait(false);
            }
        }
    }
}
