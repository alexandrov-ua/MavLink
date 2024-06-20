using MavLink.Serialize.Messages;

namespace MavLink.Client;

public interface IMavLinkClient : IDisposable
{
    IPocket<IPayload> Receive();
    Task Send(IPocket<IPayload> pocket);
}