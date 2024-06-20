namespace MavLink.Client.Transports;

public interface IMavLinkTransport : IDisposable
{
    int Receive(Span<byte> buffer);
    Task Send(ReadOnlyMemory<byte> memory);
}