using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi
{
    [PublicAPI]
    public class AuthenticatedSession : IDisposable, IAsyncDisposable
    {
        private protected readonly IUdpApi API;

        public UdpApiResult? AuthResult { get; private protected set; }
        public virtual bool IsActive => API.IsAuthenticated;

        internal AuthenticatedSession(IUdpApi api)
        {
            API = api;
        }

        public static async Task<AuthenticatedSession> CreateSession(IUdpApi api, string username, string password, CancellationToken cancellationToken = default)
        {
            var session = new AuthenticatedSession(api);
            session.AuthResult = await api.Auth(username, password, cancellationToken);
            return session;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if (IsActive)
            {
                // TODO: maybe do something with this
                var res = API.Logout().Result;
            }
        }

        public async ValueTask DisposeAsync()
        {
            GC.SuppressFinalize(this);
            if (IsActive)
            {
                // TODO: maybe do something with this
                var res = await API.Logout().ConfigureAwait(false);
            }
        }
    }
}
