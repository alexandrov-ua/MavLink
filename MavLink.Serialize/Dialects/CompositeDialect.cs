using System.Data.SqlTypes;
using MavLink.Serialize.Messages;

namespace MavLink.Serialize.Dialects;

public class CompositeDialect : IDialect
{
    private readonly IReadOnlyCollection<IDialect> _dialectsCollection;

    public CompositeDialect(IReadOnlyCollection<IDialect> dialectsCollection)
    {
        _dialectsCollection = dialectsCollection;
    }
    
    public IPocket<IPayload>? CreatePocket(uint messageId, bool isMavlinkV2, byte sequenceNumber, byte systemId, byte componentId,
        ReadOnlySpan<byte> payload)
    {
        foreach (var dialect in _dialectsCollection)
        {
            var result = dialect.CreatePocket(messageId, isMavlinkV2, sequenceNumber, systemId, componentId, payload);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }
}