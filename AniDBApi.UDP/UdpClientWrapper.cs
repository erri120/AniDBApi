using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi.UDP
{
    [PublicAPI]
    public class UdpClientWrapper : IUdpClient
    {
        private readonly UdpClient _client;

        public UdpClientWrapper(UdpClient client)
        {
            _client = client;
        }

        public void Connect(string hostname, int port) => _client.Connect(hostname, port);

        public ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default)
            => _client.SendAsync(datagram, cancellationToken);

        public ValueTask<UdpReceiveResult> ReceiveAsync(string commandName, CancellationToken cancellationToken)
            => _client.ReceiveAsync(cancellationToken);
    }
}


