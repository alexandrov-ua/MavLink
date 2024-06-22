using System.Net;
using System.Net.Sockets;

namespace MavLink.Client.Transports;

public class UdpTansport : IMavLinkTransport
{
    private EndPoint _endPoint;
    private readonly Socket _socket;

    public UdpTansport(EndPoint endPoint)
    {
        _endPoint = new IPEndPoint(IPAddress.Any, 0);
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(endPoint);

    }

    public int Receive(Span<byte> buffer)
    {
        return _socket.ReceiveFrom(buffer, SocketFlags.None, ref _endPoint);
    }

    public async Task Send(ReadOnlyMemory<byte> memory)
    {
        await _socket.SendToAsync(memory, SocketFlags.None, _endPoint);
    }

    public void Dispose()
    {
        _socket.Dispose();
    }

    ~UdpTansport()
    {
        _socket.Dispose();
    }
}