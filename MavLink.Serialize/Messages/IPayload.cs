namespace MavLink.Serialize.Messages;

public interface IPayload
{
    void Serialize(Span<byte> span);
    int GetMaxByteSize();
}

public interface IPayload<T> where T : IPayload<T>, IPayload
{
    static abstract T Deserialize(ReadOnlySpan<byte> span);
    static abstract void Serialize(T value, Span<byte> span);
}