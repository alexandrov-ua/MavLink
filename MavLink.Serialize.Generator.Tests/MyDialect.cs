
using MavLink.Serialize;
using MavLink.Serialize.Dialects;
using MavLink.Serialize.Messages;

namespace My.Test;

public class MyDialect : IDialect
{
    public static IDialect Default { get; } = new MyDialect();

    public IPocket<IPayload> CreatePocket(uint messageId, bool isMavlinkV2, byte sequenceNumber,
        byte systemId, byte componentId, ReadOnlySpan<byte> payload)
    {
        return messageId switch
        {
            0 => new HeartbeatPocket(isMavlinkV2, sequenceNumber, systemId, componentId, HeartbeatPayload.Deserialize(payload)),
            300 => new ProtocolVersionPocket(isMavlinkV2, sequenceNumber, systemId, componentId, ProtocolVersionPayload.Deserialize(payload)),
            301 => new ProtocolVersion1Pocket(isMavlinkV2, sequenceNumber, systemId, componentId, ProtocolVersion1Payload.Deserialize(payload)),
            _ => throw new NotImplementedException()
        };
    }
}

#region Enums

/// <summary>
/// Micro air vehicle / autopilot classes
/// </summary>
public enum MavAutopilot
{
    /// <summary>
    /// Generic autopilot, full support for everything
    /// </summary>
    MAV_AUTOPILOT_GENERIC = 0,
    /// <summary>
    /// Reserved for future use
    /// </summary>
    MAV_AUTOPILOT_RESERVED = 1,
}
/// <summary>
/// These flags encode the MAV mode
/// </summary>

[Flags]
public enum MavModeFlag
{
    /// <summary>
    /// 0b00000001 Reserved for future use
    /// </summary>
    MAV_MODE_FLAG_CUSTOM_MODE_ENABLED = 1,
    /// <summary>
    /// Reserved for future use
    /// </summary>
    MAV_MODE_FLAG_TEST_ENABLED = 2,
    /// <summary>
    /// Reserved for future use
    /// </summary>
    MAV_MODE_FLAG_AUTO_ENABLED = 4,
}

#endregion

#region Messages


public class HeartbeatPocket : Pocket<HeartbeatPayload>
{
    public HeartbeatPocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        HeartbeatPayload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 0;
    public override string MessageName => "HEARTBEAT";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 4;
}

public class HeartbeatPayload : IPayload<HeartbeatPayload>, IPayload
{
    /// <summary>
    /// 
    /// </summary>
    public byte Type { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public uint CustomMode { get; private set; }

    public static HeartbeatPayload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new HeartbeatPayload();
        payload.Type = BitConverterHelper.Read<byte>(ref span);
        payload.CustomMode = BitConverterHelper.Read<uint>(ref span);
        return payload;
    }

    public static void Serialize(HeartbeatPayload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.Write(Type, ref span);
        BitConverterHelper.Write(CustomMode, ref span);
    }

    public int GetMaxByteSize() => 5;
}


public class ProtocolVersionPocket : Pocket<ProtocolVersionPayload>
{
    public ProtocolVersionPocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        ProtocolVersionPayload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 300;
    public override string MessageName => "PROTOCOL_VERSION";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 178;
}

public class ProtocolVersionPayload : IPayload<ProtocolVersionPayload>, IPayload
{
    /// <summary>
    /// 
    /// </summary>
    public ushort[] Version { get; private set; } = new ushort[10];
    /// <summary>
    /// 
    /// </summary>
    public uint CustomMode { get; private set; }

    public static ProtocolVersionPayload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new ProtocolVersionPayload();
        BitConverterHelper.ReadArray<ushort>(payload.Version, ref span);
        payload.CustomMode = BitConverterHelper.Read<uint>(ref span);
        return payload;
    }

    public static void Serialize(ProtocolVersionPayload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.WriteArray(Version, ref span);
        BitConverterHelper.Write(CustomMode, ref span);
    }

    public int GetMaxByteSize() => 24;
}


public class ProtocolVersion1Pocket : Pocket<ProtocolVersion1Payload>
{
    public ProtocolVersion1Pocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        ProtocolVersion1Payload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 301;
    public override string MessageName => "PROTOCOL_VERSION1";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 98;
}

public class ProtocolVersion1Payload : IPayload<ProtocolVersion1Payload>, IPayload
{
    /// <summary>
    /// 
    /// </summary>
    public MavAutopilot Version { get; private set; }
    /// <summary>
    /// 
    /// </summary>
    public uint CustomMode { get; private set; }

    public static ProtocolVersion1Payload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new ProtocolVersion1Payload();
        payload.Version = (MavAutopilot)BitConverterHelper.Read<ushort>(ref span);
        payload.CustomMode = BitConverterHelper.Read<uint>(ref span);
        return payload;
    }

    public static void Serialize(ProtocolVersion1Payload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.Write((ushort)Version, ref span);
        BitConverterHelper.Write(CustomMode, ref span);
    }

    public int GetMaxByteSize() => 6;
}


#endregion
