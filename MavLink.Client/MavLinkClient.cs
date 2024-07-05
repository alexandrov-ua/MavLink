using System.Dynamic;
using System.Net;
using MavLink.Client.Transports;
using MavLink.Serialize;
using MavLink.Serialize.Dialects;
using MavLink.Serialize.Messages;

namespace MavLink.Client;

public class MavLinkClient : IMavLinkClient
{
    private readonly IMavLinkTransport _transport;
    private readonly IDialect _dialect;

    public static IMavLinkClient Create(string connectionString, IDialect dialect)
    {
        //TODO: handle connection string to choose appropriate transport
        var hostAndPort = connectionString.Replace("udp://", "");
        return new MavLinkClient(new UdpTransport(IPEndPoint.Parse(hostAndPort)), dialect);
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

    public async Task Send(IPocket<IPayload> pocket)
    {
        Memory<byte> memory = new byte[pocket.GetMaxByteSize()];
        MavlinkSerialize.Serialize(pocket, memory.Span, out var written);
        await _transport.Send(memory.Slice(0, written));
    }

    public void Dispose()
    {
        _transport.Dispose();
    }
}