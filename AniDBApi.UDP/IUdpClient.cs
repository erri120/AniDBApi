using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace AniDBApi.UDP;

[PublicAPI]
public interface IUdpClient
{
    void Connect(string hostname, int port);
    ValueTask<int> SendAsync(ReadOnlyMemory<byte> datagram, CancellationToken cancellationToken = default);
    ValueTask<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken);
}
