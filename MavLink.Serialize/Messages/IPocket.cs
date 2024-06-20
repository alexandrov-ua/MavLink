namespace MavLink.Serialize.Messages;

public interface IPocket<out TPayload> where TPayload : IPayload
{
    uint MessageId { get; }
    public string MessageName { get; }
    bool IsMavlinkV2 { get; }
    byte SequenceNumber { get; set; }
    byte SystemId { get; }
    byte ComponentId { get; }
    TPayload Payload { get; }
    
    int GetMaxByteSize();
    byte GetChecksumExtra();
}