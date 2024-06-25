using MavLink.Serialize.Dialects;
using MavLink.Serialize.Messages;

namespace MavLink.Serialize;

//TODO:
// - v1 backward compatibility
// - signature
// - in case of error return an result
public static class MavlinkSerialize
{
    public static IPocket<IPayload> Deserialize(ReadOnlySpan<byte> buffer, IDialect dialect)
    {
        return buffer[0] switch
        {
            0xFD => DeserializeV2(buffer, dialect),
            0xFE => DeserializeV1(buffer, dialect),
            _ => throw new Exception("Not a MavLink")
        };
    }

    static IPocket<IPayload> DeserializeV2(ReadOnlySpan<byte> buffer, IDialect dialect)
    {
        var length = buffer[1];
        var incompatFlags = buffer[2];
        var compatFlags = buffer[3];
        var sequenceNumber = buffer[4];
        var systemId = buffer[5];
        var componentId = buffer[6];
        var messageId = (uint) buffer[7] | (uint) buffer[8] << 8 | (uint) buffer[9] << 16;

        var checksum = BitConverter.ToUInt16(buffer.Slice(10 + length, 2));

        var result = dialect.CreatePocket(messageId, true, sequenceNumber, systemId, componentId,
            buffer.Slice(10, length));
        if (result == null)
            throw new Exception("unknown message id");
        var actualChecksum = ChecksumHelper.Calculate(buffer.Slice(1, 10 + length - 1), result.GetChecksumExtra());

        if (checksum != actualChecksum)
        {
            throw new Exception("check sum fail");
        }

        return result;
    }

    static IPocket<IPayload> DeserializeV1(ReadOnlySpan<byte> buffer, IDialect dialect)
    {
        var length = buffer[1];
        var sequenceNumber = buffer[2];
        var systemId = buffer[3];
        var componentId = buffer[4];
        var messageId = (uint) buffer[5];

        var checksum = BitConverter.ToUInt16(buffer.Slice(6 + length, 2));

        var result = dialect.CreatePocket(messageId, false, sequenceNumber, systemId, componentId,
            buffer.Slice(6, length));
        if (result == null)
            throw new Exception("unknown message id");
        var actualChecksum = ChecksumHelper.Calculate(buffer.Slice(1, 6 + length - 1), result.GetChecksumExtra());

        if (checksum != actualChecksum)
        {
            throw new Exception("check sum fail");
        }

        return result;
    }


    public static void Serialize(IPocket<IPayload> pocket, Span<byte> span, out int bytesWritten)
    {
        if (pocket.IsMavlinkV2)
        {
            SerializeV2(pocket, span, out bytesWritten);
        }
        else
        {
            SerializeV1(pocket, span, out bytesWritten);
        }
    }

    private static void SerializeV2(IPocket<IPayload> pocket, Span<byte> span, out int bytesWritten)
    {
        var header = span.Slice(0, 10);
        var payload = span.Slice(10);
        pocket.Payload.Serialize(payload);
        var length = payload.Length;
        for (; length > 0 && payload[length - 1] == 0; length--)
        {
        }

        var crc = payload.Slice(length);

        BitConverterHelper.Write<byte>(0xFD, ref header);
        BitConverterHelper.Write<byte>((byte) length, ref header);
        BitConverterHelper.Write<byte>((byte) 0, ref header); //TODO: add 
        BitConverterHelper.Write<byte>((byte) 0, ref header);
        BitConverterHelper.Write(pocket.SequenceNumber, ref header);
        BitConverterHelper.Write(pocket.SystemId, ref header);
        BitConverterHelper.Write(pocket.ComponentId, ref header);
        foreach (var b in BitConverter.GetBytes((uint) pocket.MessageId).Take(3))
        {
            BitConverterHelper.Write(b, ref header);
        }

        var actualChecksum = ChecksumHelper.Calculate(span.Slice(1, 10 + length - 1),
            pocket.GetChecksumExtra());
        bytesWritten = 10 + length + 2;
        BitConverterHelper.Write(actualChecksum, ref crc);
    }

    private static void SerializeV1(IPocket<IPayload> pocket, Span<byte> span, out int bytesWritten)
    {
        var header = span.Slice(0, 6);
        var payload = span.Slice(6, pocket.Payload.GetMaxByteSize());
        pocket.Payload.Serialize(payload);
        var length = payload.Length;

        var crc = span.Slice(6 + length, 2);

        BitConverterHelper.Write<byte>(0xFE, ref header);
        BitConverterHelper.Write<byte>((byte) length, ref header);
        BitConverterHelper.Write(pocket.SequenceNumber, ref header);
        BitConverterHelper.Write(pocket.SystemId, ref header);
        BitConverterHelper.Write(pocket.ComponentId, ref header);
        BitConverterHelper.Write((byte) pocket.MessageId, ref header);

        var actualChecksum = ChecksumHelper.Calculate(span.Slice(1, 5 + length),
            pocket.GetChecksumExtra());
        bytesWritten = 6 + length + 2;
        BitConverterHelper.Write(actualChecksum, ref crc);
    }
}