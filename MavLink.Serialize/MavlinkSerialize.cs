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
        if (buffer[0] != 0xFD && buffer[0] != 0xFE)
        {
            throw new Exception("Not a MavLink");
        }

        var magic = buffer[0];
        var length = buffer[1];
        var incompatFlags = buffer[2];
        var compatFlags = buffer[3];
        var sequenceNumber = buffer[4];
        var systemId = buffer[5];
        var componentId = buffer[6];
        var messageId = (uint) buffer[7] | (uint) buffer[8] << 8 | (uint) buffer[9] << 16;

        var checksum = BitConverter.ToUInt16(buffer.Slice(10 + length, 2));

        var result = dialect.CreatePocket(messageId, magic == 0xFD, sequenceNumber, systemId, componentId,
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

    public static void Serialize(IPocket<IPayload> pocket, Span<byte> span, out int bytesWritten)
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
}