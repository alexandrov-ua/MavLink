using MavLink.Serialize.Messages;

namespace MavLink.Serialize.Dialects;

public interface IDialect
{
    public string Name { get; }

    IPocket<IPayload>? CreatePocket(uint messageId, bool isMavlinkV2, byte sequenceNumber,
        byte systemId, byte componentId, ReadOnlySpan<byte> payload);
}