namespace MavLink.Serialize.Messages;


public abstract class Pocket<T> : IPocket<T> where T : IPayload
{
    public abstract uint MessageId { get; }
    public abstract string MessageName { get; }
    public abstract int GetMaxByteSize();
    public abstract byte GetChecksumExtra();
    public bool IsMavlinkV2 { get; } 
    public byte SequenceNumber { get; }
    public byte SystemId { get; } 
    public byte ComponentId { get; }
    public T Payload { get; }

    public Pocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId, T payload)
    {
        IsMavlinkV2 = isMavlink2;
        SequenceNumber = sequenceNumber;
        SystemId = systemId;
        ComponentId = componentId;
        Payload = payload;
    }

    public override string ToString()
    {
        return $"{DateTime.MinValue},{IsMavlinkV2},{SystemId},{ComponentId},{(uint)MessageId},{MessageName}";
    }
}