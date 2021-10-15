﻿using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public class EncryptedSession : AuthenticatedSession
    {
        public UdpApiResult? EncryptResult;
        public override bool IsActive => API.IsAuthenticated && API.IsEncrypted;

        internal EncryptedSession(UdpApi api) : base(api) { }

        public static async Task<EncryptedSession> CreateSession(UdpApi api, string username, string password, string apiKey, CancellationToken cancellationToken = default)
        {
            var session = new EncryptedSession(api);
            session.EncryptResult = await api.Encrypt(username, apiKey, cancellationToken);
            if (session.API.IsEncrypted)
                session.AuthResult = await api.Auth(username, password, cancellationToken);
            return session;
        }
    }
}


