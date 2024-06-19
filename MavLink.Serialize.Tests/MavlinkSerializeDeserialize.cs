using FluentAssertions;
using MavLink.Serialize.Tests.Dialects;
using MavLink.Serialize.Tests.Mavgen;

namespace MavLink.Serialize.Tests;

public class MavlinkSerializeDeserialize
{
    [Fact]
    public void AttitudePocket()
    {
        var data = new byte[] //Altitude
        {
            0xfd, 0x1c, 0x00, 0x00, 0x4c, 0x01, 0x01, 0x1e, 0x00, 0x00, 0xcd, 0xd1, 0x23, 0x00, 0x4e, 0xd2, 0x90, 0xb9,
            0xef, 0x64, 0x99, 0xb9, 0x00, 0x30, 0x1f, 0xc0, 0x84, 0x78, 0x9a, 0xb9, 0xe4, 0x68, 0xd6, 0xb9, 0xfa, 0xdb,
            0x3e, 0xba, 0x64, 0xfa
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<AttitudePocket>()
            .Which.Payload.Should().BeAssignableTo<AttitudePayload>();

        var concretePocket = (AttitudePocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_attitude_t) mavgenPocket.data;

        concretePocket.Payload.Pitch.Should().Be(mavgenConcretePocket.pitch);
        concretePocket.Payload.Roll.Should().Be(mavgenConcretePocket.roll);
        concretePocket.Payload.Yaw.Should().Be(mavgenConcretePocket.yaw);
        concretePocket.Payload.PitchSpeed.Should().Be(mavgenConcretePocket.pitchspeed);
        concretePocket.Payload.RollSpeed.Should().Be(mavgenConcretePocket.rollspeed);
        concretePocket.Payload.YawSeed.Should().Be(mavgenConcretePocket.yawspeed);
        concretePocket.Payload.TimeBootMs.Should().Be(mavgenConcretePocket.time_boot_ms);
    }

    [Fact]
    public void SysTimePocket()
    {
        var data = new byte[] //SysTime
        {
            0xfd, 0x0b, 0x00, 0x00, 0x32, 0x01, 0x01, 0x02, 0x00, 0x00, 0xb1, 0x6e, 0x91, 0x93, 0xb2, 0x1a, 0x06, 0x00,
            0xc8, 0x55, 0x23, 0xF9, 0x4A
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<SystemTimePocket>()
            .Which.Payload.Should().BeAssignableTo<SystemTimePayload>();

        var concretePocket = (SystemTimePocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_system_time_t) mavgenPocket.data;

        concretePocket.Payload.TimeBootMs.Should().Be(mavgenConcretePocket.time_boot_ms);
        concretePocket.Payload.TimeUnixUsec.Should().Be(mavgenConcretePocket.time_unix_usec);
    }

    [Fact]
    public void SysStatusPocket()
    {
        var data = new byte[]
        {
            0xfd, 0x1f, 0x00, 0x00, 0x38, 0x01, 0x01, 0x01, 0x00, 0x00, 0x2f, 0xfc, 0x71, 0x53, 0x2f, 0xfc, 0x61, 0x53,
            0x2f, 0xfc, 0x71, 0x57, 0x00, 0x00, 0x38, 0x31, 0xfa, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x5b, 0xee, 0x50
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<SysStatusPocket>()
            .Which.Payload.Should().BeAssignableTo<SysStatusPayload>();

        var concretePocket = (SysStatusPocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_sys_status_t) mavgenPocket.data;

        concretePocket.Payload.Load.Should().Be(mavgenConcretePocket.load);
        concretePocket.Payload.BatteryRemaining.Should().Be((byte) mavgenConcretePocket.battery_remaining);
        concretePocket.Payload.CurrentBattery.Should().Be((ushort) mavgenConcretePocket.current_battery);
        concretePocket.Payload.ErrorsComm.Should().Be(mavgenConcretePocket.errors_comm);
        concretePocket.Payload.ErrorsCount1.Should().Be(mavgenConcretePocket.errors_count1);
        concretePocket.Payload.ErrorsCount2.Should().Be(mavgenConcretePocket.errors_count2);
        concretePocket.Payload.ErrorsCount3.Should().Be(mavgenConcretePocket.errors_count3);
        concretePocket.Payload.ErrorsCount4.Should().Be(mavgenConcretePocket.errors_count4);
    }

    [Fact]
    public void CommandIntPocket()
    {
        var data = new byte[]
        {
            0xfd, 0x20, 0x00, 0x00, 0xdb, 0xff, 0xbe, 0x4b, 0x00, 0x00, 0x00, 0x00, 0x80, 0xbf, 0x00, 0x00, 0x80, 0x3f,
            0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x7f, 0x92, 0x90, 0xec, 0xea, 0x06, 0x13, 0xe9, 0x58, 0xa4, 0xa0,
            0x13, 0x44, 0xc0, 0x00, 0x01, 0x01, 0x1f, 0x85,
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<CommandIntPocket>()
            .Which.Payload.Should().BeAssignableTo<CommandIntPayload>();

        var concretePocket = (CommandIntPocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_command_int_t) mavgenPocket.data;

        concretePocket.Payload.Command.Should().Be(mavgenConcretePocket.command);
        concretePocket.Payload.Current.Should().Be(mavgenConcretePocket.current);
        concretePocket.Payload.Frame.Should().Be(mavgenConcretePocket.frame);
        concretePocket.Payload.Param1.Should().Be(mavgenConcretePocket.param1);
        concretePocket.Payload.Param2.Should().Be(mavgenConcretePocket.param2);
        concretePocket.Payload.Param3.Should().Be(mavgenConcretePocket.param3);
        concretePocket.Payload.Param4.Should().Be(mavgenConcretePocket.param4);
        concretePocket.Payload.X.Should().Be(mavgenConcretePocket.x);
        concretePocket.Payload.Y.Should().Be(mavgenConcretePocket.y);
        concretePocket.Payload.Z.Should().Be(mavgenConcretePocket.z);
        concretePocket.Payload.AutoContinue.Should().Be(mavgenConcretePocket.autocontinue);
        concretePocket.Payload.TargetComponent.Should().Be(mavgenConcretePocket.target_component);
        concretePocket.Payload.TargetSystem.Should().Be(mavgenConcretePocket.target_system);
    }

    [Fact]
    public void FileTransferProtocol()
    {
        var data = new byte[]
        {
            0xfd, 0xfe, 0x00, 0x00, 0x42, 0x01, 0x01, 0x6e, 0x00, 0x00, 0x00, 0xff, 0xbe, 0x3f, 0x00, 0x00, 0x80, 0xef,
            0x0f, 0x00, 0x00, 0x15, 0x37, 0x00, 0x00, 0x00, 0x01, 0x80, 0x56, 0x49, 0x53, 0x4f, 0x5f, 0x54, 0x59, 0x50,
            0x45, 0x00, 0x01, 0x81, 0x54, 0x58, 0x5f, 0x45, 0x4e, 0x41, 0x42, 0x4c, 0x45, 0x00, 0x01, 0xd0, 0x4d, 0x53,
            0x50, 0x5f, 0x4f, 0x53, 0x44, 0x5f, 0x4e, 0x43, 0x45, 0x4c, 0x4c, 0x53, 0x00, 0x01, 0x55, 0x50, 0x54, 0x49,
            0x4f, 0x4e, 0x53, 0x00, 0x01, 0xe0, 0x46, 0x52, 0x53, 0x4b, 0x59, 0x5f, 0x55, 0x50, 0x4c, 0x49, 0x4e, 0x4b,
            0x5f, 0x49, 0x44, 0x0d, 0x01, 0x96, 0x44, 0x4e, 0x4c, 0x49, 0x4e, 0x4b, 0x31, 0x5f, 0x49, 0x44, 0x14, 0x01,
            0x3c, 0x32, 0x5f, 0x49, 0x44, 0x07, 0x01, 0x2c, 0x5f, 0x49, 0x44, 0x1b, 0x01, 0x66, 0x4f, 0x50, 0x54, 0x49,
            0x4f, 0x4e, 0x53, 0x00, 0x01, 0x70, 0x47, 0x45, 0x4e, 0x5f, 0x54, 0x59, 0x50, 0x45, 0x00, 0x01, 0x90, 0x45,
            0x41, 0x48, 0x52, 0x53, 0x5f, 0x54, 0x59, 0x50, 0x45, 0x00, 0x01, 0x61, 0x46, 0x49, 0x5f, 0x54, 0x59, 0x50,
            0x45, 0x00, 0x01, 0xb0, 0x41, 0x52, 0x53, 0x50, 0x44, 0x5f, 0x45, 0x4e, 0x41, 0x42, 0x4c, 0x45, 0x00, 0x01,
            0xe0, 0x43, 0x55, 0x53, 0x54, 0x5f, 0x52, 0x4f, 0x54, 0x5f, 0x45, 0x4e, 0x41, 0x42, 0x4c, 0x45, 0x00, 0x01,
            0xe0, 0x45, 0x53, 0x43, 0x5f, 0x54, 0x4c, 0x4d, 0x5f, 0x4d, 0x41, 0x56, 0x5f, 0x4f, 0x46, 0x53, 0x00, 0x01,
            0xb0, 0x46, 0x45, 0x4e, 0x43, 0x45, 0x5f, 0x45, 0x4e, 0x41, 0x42, 0x4c, 0x45, 0x00, 0x01, 0x36, 0x54, 0x59,
            0x50, 0x45, 0x07, 0x01, 0x56, 0x41, 0x43, 0x54, 0x49, 0x4f, 0x4e, 0x01, 0x04, 0x57, 0x4c, 0x54, 0x5f, 0x4d,
            0x41, 0x58, 0x00, 0x00, 0xc8, 0x42, 0x04, 0x56, 0x52, 0x41, 0x44, 0x49, 0x62, 0xaf,
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<FileTransferProtocolPocket>()
            .Which.Payload.Should().BeAssignableTo<FileTransferProtocolPayload>();

        var concretePocket = (FileTransferProtocolPocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_file_transfer_protocol_t) mavgenPocket.data;

        concretePocket.Payload.TargetComponent.Should().Be(mavgenConcretePocket.target_component);
        concretePocket.Payload.TargetSystem.Should().Be(mavgenConcretePocket.target_system);
        concretePocket.Payload.TargetNetwork.Should().Be(mavgenConcretePocket.target_network);

        Assert.Equal(concretePocket.Payload.Payload, mavgenConcretePocket.payload);
    }


    [Fact]
    public void FileTransferProtocol2()
    {
        var data = new byte[]
        {
            0xfd, 0x09, 0x00, 0x00, 0x48, 0x01, 0x01, 0x6e, 0x00, 0x00, 0x00, 0xff, 0xbe, 0x46, 0x00, 0x00, 0x80, 0x00,
            0x02, 0x1b, 0x86,
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<FileTransferProtocolPocket>()
            .Which.Payload.Should().BeAssignableTo<FileTransferProtocolPayload>();

        var concretePocket = (FileTransferProtocolPocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_file_transfer_protocol_t) mavgenPocket.data;

        concretePocket.Payload.TargetComponent.Should().Be(mavgenConcretePocket.target_component);
        concretePocket.Payload.TargetSystem.Should().Be(mavgenConcretePocket.target_system);
        concretePocket.Payload.TargetNetwork.Should().Be(mavgenConcretePocket.target_network);

        Assert.Equal(concretePocket.Payload.Payload, mavgenConcretePocket.payload);
    }

    [Fact]
    public void TerrainData()
    {
        var data = new byte[]
        {
            0xFD, 0x27, 0x00, 0x00, 0x7B, 0xFF, 0x7B, 0x86, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x03, 0x00, 0x04, 0x00, 0x05, 0x00, 0x06, 0x00, 0x07, 0x00, 0x08, 0x00,
            0x09, 0x00, 0x0A, 0x00, 0x0B, 0x00, 0x0C, 0x00, 0x0D, 0x00, 0x0E, 0x00, 0x0F, 0x54, 0x53
        };

        var pocket = MavlinkSerialize.Deserialize(data, TestDialect.Default);
        var parser = new MAVLink.MavlinkParse(false);
        var mavgenPocket = parser.ReadPacket(new MemoryStream(data));

        pocket.ToString().Should().Be(mavgenPocket.ToString());
        pocket.Should().BeAssignableTo<TerrainDataPocket>()
            .Which.Payload.Should().BeAssignableTo<TerrainDataPayload>();

        var concretePocket = (TerrainDataPocket) pocket;
        var mavgenConcretePocket = (MAVLink.mavlink_terrain_data_t) mavgenPocket.data;

        concretePocket.Payload.Gridbit.Should().Be(mavgenConcretePocket.gridbit);
        concretePocket.Payload.Lat.Should().Be(mavgenConcretePocket.lat);
        concretePocket.Payload.Lon.Should().Be(mavgenConcretePocket.lon);
        concretePocket.Payload.GridSpacing.Should().Be(mavgenConcretePocket.grid_spacing);

        Assert.Equal(concretePocket.Payload.Data, mavgenConcretePocket.data);
    }
}