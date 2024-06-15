using MavLink.Serialize.Messages;

namespace MavLink.Serialize.Dialects.Test;

public class TestDialect : IDialect
{
    public static IDialect Default { get; } = new TestDialect();

    public IPocket<IPayload> CreatePocket(uint messageId, bool isMavlinkV2, byte sequenceNumber,
        byte systemId, byte componentId, ReadOnlySpan<byte> payload)
    {
        return messageId switch
        {
            2 => new SystemTimePocket(isMavlinkV2, sequenceNumber, systemId, componentId,
                SystemTimePayload.Deserialize(payload)),
            1 => new SysStatusPocket(isMavlinkV2, sequenceNumber, systemId, componentId,
                SysStatusPayload.Deserialize(payload)),
            75 => new CommandIntPocket(isMavlinkV2, sequenceNumber, systemId, componentId,
                CommandIntPayload.Deserialize(payload)),
            30 => new AttitudePocket(isMavlinkV2, sequenceNumber, systemId, componentId,
                AttitudePayload.Deserialize(payload)),
            _ => throw new NotImplementedException()
        };
    }
}

#region Enums

#endregion

#region Messages

public class AttitudePocket : Pocket<AttitudePayload>
{
    public AttitudePocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        AttitudePayload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 30;
    public override string MessageName => "ATTITUDE";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 39;
}

public class AttitudePayload : IPayload<AttitudePayload>, IPayload
{
    public uint TimeBootMs { get; private set; }
    public float Roll { get; private set; }
    public float Pitch { get; private set; }
    public float Yaw { get; private set; }
    public float RollSpeed { get; private set; }
    public float PitchSpeed { get; private set; }
    public float YawSeed { get; private set; }

    public static AttitudePayload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new AttitudePayload();
        payload.TimeBootMs = BitConverterHelper.Read<uint>(ref span);
        payload.Roll = BitConverterHelper.Read<float>(ref span);
        payload.Pitch = BitConverterHelper.Read<float>(ref span);
        payload.Yaw = BitConverterHelper.Read<float>(ref span);
        payload.RollSpeed = BitConverterHelper.Read<float>(ref span);
        payload.PitchSpeed = BitConverterHelper.Read<float>(ref span);
        payload.YawSeed = BitConverterHelper.Read<float>(ref span);
        return payload;
    }

    public static void Serialize(AttitudePayload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.Write(TimeBootMs, ref span);
        BitConverterHelper.Write(Roll, ref span);
        BitConverterHelper.Write(Pitch, ref span);
        BitConverterHelper.Write(Yaw, ref span);
        BitConverterHelper.Write(RollSpeed, ref span);
        BitConverterHelper.Write(PitchSpeed, ref span);
        BitConverterHelper.Write(YawSeed, ref span);
    }

    public int GetMaxByteSize() => 28;
}

public class CommandIntPocket : Pocket<CommandIntPayload>
{
    public CommandIntPocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        CommandIntPayload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 75;
    public override string MessageName => "COMMAND_INT";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 158;
}

public class CommandIntPayload : IPayload<CommandIntPayload>, IPayload
{
    public float Param1 { get; private set; }
    public float Param2 { get; private set; }
    public float Param3 { get; private set; }
    public float Param4 { get; private set; }
    public int X { get; private set; }
    public int Y { get; private set; }
    public float Z { get; private set; }
    public ushort Command { get; private set; }
    public byte TargetSystem { get; private set; }
    public byte TargetComponent { get; private set; }
    public byte Frame { get; private set; }
    public byte Current { get; private set; }
    public byte AutoContinue { get; private set; }

    public static CommandIntPayload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new CommandIntPayload();
        payload.Param1 = BitConverterHelper.Read<float>(ref span);
        payload.Param2 = BitConverterHelper.Read<float>(ref span);
        payload.Param3 = BitConverterHelper.Read<float>(ref span);
        payload.Param4 = BitConverterHelper.Read<float>(ref span);
        payload.X = BitConverterHelper.Read<int>(ref span);
        payload.Y = BitConverterHelper.Read<int>(ref span);
        payload.Z = BitConverterHelper.Read<float>(ref span);
        payload.Command = BitConverterHelper.Read<ushort>(ref span);
        payload.TargetSystem = BitConverterHelper.Read<byte>(ref span);
        payload.TargetComponent = BitConverterHelper.Read<byte>(ref span);
        payload.Frame = BitConverterHelper.Read<byte>(ref span);
        payload.Current = BitConverterHelper.Read<byte>(ref span);
        payload.AutoContinue = BitConverterHelper.Read<byte>(ref span);
        return payload;
    }

    public static void Serialize(CommandIntPayload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.Write(Param1, ref span);
        BitConverterHelper.Write(Param2, ref span);
        BitConverterHelper.Write(Param3, ref span);
        BitConverterHelper.Write(Param4, ref span);
        BitConverterHelper.Write(X, ref span);
        BitConverterHelper.Write(Y, ref span);
        BitConverterHelper.Write(Z, ref span);
        BitConverterHelper.Write(Command, ref span);
        BitConverterHelper.Write(TargetSystem, ref span);
        BitConverterHelper.Write(TargetComponent, ref span);
        BitConverterHelper.Write(Frame, ref span);
        BitConverterHelper.Write(Current, ref span);
        BitConverterHelper.Write(AutoContinue, ref span);
    }

    public int GetMaxByteSize() => 35;
}

public class SysStatusPocket : Pocket<SysStatusPayload>
{
    public SysStatusPocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        SysStatusPayload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 1;
    public override string MessageName => "SYS_STATUS";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 124;
}

public class SysStatusPayload : IPayload<SysStatusPayload>, IPayload
{
    public uint OnboardControlSensorsPresent { get; private set; }
    public uint OnboardControlSensorsEnabled { get; private set; }
    public uint OnboardControlSensorsHealth { get; private set; }
    public ushort Load { get; private set; }
    public ushort VoltageBattery { get; private set; }
    public ushort CurrentBattery { get; private set; }
    public byte BatteryRemaining { get; private set; }
    public ushort DropRateComm { get; private set; }
    public ushort ErrorsComm { get; private set; }
    public ushort ErrorsCount1 { get; private set; }
    public ushort ErrorsCount2 { get; private set; }
    public ushort ErrorsCount3 { get; private set; }
    public ushort ErrorsCount4 { get; private set; }

    public uint OnboardControlSensorsPresentExtended { get; private set; }
    public uint OnboardControlSensorsEnabledExtended { get; private set; }
    public uint OnboardControlSensorsHealthExtended { get; private set; }

    public static SysStatusPayload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new SysStatusPayload();
        payload.OnboardControlSensorsPresent = BitConverterHelper.Read<uint>(ref span);
        payload.OnboardControlSensorsEnabled = BitConverterHelper.Read<uint>(ref span);
        payload.OnboardControlSensorsHealth = BitConverterHelper.Read<uint>(ref span);
        payload.Load = BitConverterHelper.Read<ushort>(ref span);
        payload.VoltageBattery = BitConverterHelper.Read<ushort>(ref span);
        payload.CurrentBattery = BitConverterHelper.Read<ushort>(ref span);
        payload.DropRateComm = BitConverterHelper.Read<ushort>(ref span);
        payload.ErrorsComm = BitConverterHelper.Read<ushort>(ref span);
        payload.ErrorsCount1 = BitConverterHelper.Read<ushort>(ref span);
        payload.ErrorsCount2 = BitConverterHelper.Read<ushort>(ref span);
        payload.ErrorsCount3 = BitConverterHelper.Read<ushort>(ref span);
        payload.ErrorsCount4 = BitConverterHelper.Read<ushort>(ref span);
        payload.BatteryRemaining = BitConverterHelper.Read<byte>(ref span);
        payload.OnboardControlSensorsPresentExtended = BitConverterHelper.Read<uint>(ref span);
        payload.OnboardControlSensorsEnabledExtended = BitConverterHelper.Read<uint>(ref span);
        payload.OnboardControlSensorsHealthExtended = BitConverterHelper.Read<uint>(ref span);
        return payload;
    }

    public static void Serialize(SysStatusPayload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.Write(OnboardControlSensorsPresent, ref span);
        BitConverterHelper.Write(OnboardControlSensorsEnabled, ref span);
        BitConverterHelper.Write(OnboardControlSensorsHealth, ref span);
        BitConverterHelper.Write(Load, ref span);
        BitConverterHelper.Write(VoltageBattery, ref span);
        BitConverterHelper.Write(CurrentBattery, ref span);
        BitConverterHelper.Write(DropRateComm, ref span);
        BitConverterHelper.Write(ErrorsComm, ref span);
        BitConverterHelper.Write(ErrorsCount1, ref span);
        BitConverterHelper.Write(ErrorsCount2, ref span);
        BitConverterHelper.Write(ErrorsCount3, ref span);
        BitConverterHelper.Write(ErrorsCount4, ref span);
        BitConverterHelper.Write(BatteryRemaining, ref span);
        BitConverterHelper.Write(OnboardControlSensorsPresentExtended, ref span);
        BitConverterHelper.Write(OnboardControlSensorsEnabledExtended, ref span);
        BitConverterHelper.Write(OnboardControlSensorsHealthExtended, ref span);
    }

    public byte GetChecksumExtra() => 124;
    public int GetMaxByteSize() => 43;
}

public class SystemTimePocket : Pocket<SystemTimePayload>
{
    public SystemTimePocket(bool isMavlink2, byte sequenceNumber, byte systemId, byte componentId,
        SystemTimePayload payload) : base(isMavlink2, sequenceNumber, systemId, componentId, payload)
    {
    }

    public override uint MessageId => 2;
    public override string MessageName => "SYSTEM_TIME";
    public override int GetMaxByteSize() => Payload.GetMaxByteSize() + 12;

    public override byte GetChecksumExtra() => 137;
}

public class SystemTimePayload : IPayload<SystemTimePayload>, IPayload
{
    public ulong TimeUnixUsec { get; private set; }
    public uint TimeBootMs { get; private set; }

    public static SystemTimePayload Deserialize(ReadOnlySpan<byte> span)
    {
        var payload = new SystemTimePayload();
        payload.TimeUnixUsec = BitConverterHelper.Read<ulong>(ref span);
        payload.TimeBootMs = BitConverterHelper.Read<uint>(ref span);
        return payload;
    }

    public static void Serialize(SystemTimePayload payload, Span<byte> span)
    {
        payload.Serialize(span);
    }

    public void Serialize(Span<byte> span)
    {
        BitConverterHelper.Write(TimeUnixUsec, ref span);
        BitConverterHelper.Write(TimeBootMs, ref span);
    }


    public int GetMaxByteSize() => 12;
}

#endregion