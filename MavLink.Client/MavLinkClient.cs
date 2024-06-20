using System.Dynamic;
using System.Net;
using MavLink.Client.Transports;
using MavLink.Serialize;
using MavLink.Serialize.Dialects;
using MavLink.Serialize.Messages;

namespace MavLink.Client;

public interface IMavLinkClient
{
    IPocket<IPayload> Receive();
}

public class MavLinkClient : IMavLinkClient
{
    private readonly IMavLinkTransport _transport;
    private readonly IDialect _dialect;

    public static IMavLinkClient Create(string connectionString, IDialect dialect)
    {
        var hostAndPort = connectionString.Replace("udp://", "");
        return new MavLinkClient(new UdpTansport(IPEndPoint.Parse(hostAndPort)), dialect);
    }

    public MavLinkClient(IMavLinkTransport transport, IDialect dialect)
    {
        _transport = transport;
        _dialect = dialect;
    }

    public IPocket<IPayload> Receive()
    {
        Span<byte> buffer = stackalloc byte[300];
        var read = _transport.Receive(buffer);
        return MavlinkSerialize.Deserialize(buffer.Slice(0, read), _dialect);
    }
}