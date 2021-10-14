using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public class AuthenticatedSession : IDisposable, IAsyncDisposable
    {
        private readonly UdpApi _api;

        public readonly UdpApiResult? AuthResult;
        public bool IsActive => _api.IsAuthenticated;

        public AuthenticatedSession(UdpApi api, string username, string password)
        {
            _api = api;
            if (_api.IsAuthenticated) return;
            AuthResult = api.Auth(username, password).Result;
        }

        public AuthenticatedSession(UdpApi api, (string, string) credentials)
            : this(api, credentials.Item1, credentials.Item2) { }

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
