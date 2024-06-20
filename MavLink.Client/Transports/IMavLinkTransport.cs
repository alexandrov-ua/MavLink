using System.Net;
using System.Net.Sockets;

namespace MavLink.Client.Transports;

public interface IMavLinkTransport
{
    int Receive(Span<byte> buffer);
}

public class UdpTansport : IMavLinkTransport
{
    private readonly Socket _socket;

    public UdpTansport(EndPoint endPoint)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        _socket.Bind(endPoint);
    }

    public void Connect()
    {
        
    }

    public int Receive(Span<byte> buffer)
    {
        return _socket.Receive(buffer);
    }
}