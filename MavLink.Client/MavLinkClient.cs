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
        if (connectionString.StartsWith("udp://"))
        {
            var hostAndPort = connectionString.Replace("udp://", "");
            return new MavLinkClient(new UdpTransport(IPEndPoint.Parse(hostAndPort)), dialect);
        }
        else if (connectionString.StartsWith("serial://"))
        {
            var devAndBaud = connectionString.Replace("serial://", "").Split(":");
            return new MavLinkClient(new SerialTransport(devAndBaud[0], int.Parse(devAndBaud[1])), dialect);
        }
        else
        {
            throw new InvalidProgramException("Connection string contains ambiguous schema. Use something like udp://127.0.0.1:14550");
        }

    }

    public MavLinkClient(IMavLinkTransport transport, IDialect dialect)
    {
        _transport = transport;
        _dialect = dialect;
    }

    public IPocket<IPayload> Receive()
    {
        Span<byte> buffer = stackalloc byte[500];
        var read = _transport.Receive(buffer);
        var t = MavlinkSerialize.Deserialize(buffer.Slice(0, read), _dialect);
        return t;
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