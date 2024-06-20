using FluentAssertions;
using MavLink.Serialize.Dialects;


namespace MavLink.Serialize.Generator.Tests;

[Dialect("DialectXmls/minimal.xml")]
public partial class MyMinimalDialect
{
}

[Dialect("DialectXmls/standard.xml")]
public partial class MyStandardDialect
{
}

[Dialect("DialectXmls/common.xml")]
public partial class MyCommonDialect
{
}

[Dialect("DialectXmls/ASLUAV.xml")]
public partial class MyAsluavDialect
{
}

public class GeneratorIntegrationTest
{
    [Fact]
    public void IsDefaultDenerated()
    {
        MyCommonDialect.Create(MyStandardDialect.Create(MyMinimalDialect.Create())).Should().NotBeNull();
    }

    [Fact]
    public void TestMessageDeserialization()
    {
        var buffer = new byte[]
        {
            0xfd, 0x09, 0x00, 0x00, 0x12, 0xff, 0xbe, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x08, 0xc0, 0x04,
            0x03, 0x5d, 0xc6,
        };
        var pocket = (HeartbeatPocket) MavlinkSerialize.Deserialize(buffer,
            MyCommonDialect.Create(MyStandardDialect.Create(MyMinimalDialect.Create())));

        pocket.MessageId.Should().Be(0);
        pocket.SequenceNumber.Should().Be(18);
        pocket.SystemId.Should().Be(255);
        pocket.ComponentId.Should().Be(190);
        pocket.Payload.Type.Should().Be(MavType.MAV_TYPE_GCS);
        pocket.Payload.Autopilot.Should().Be(MavAutopilot.MAV_AUTOPILOT_INVALID);
        pocket.Payload.BaseMode.Should()
            .Be(MavModeFlag.MAV_MODE_FLAG_SAFETY_ARMED | MavModeFlag.MAV_MODE_FLAG_MANUAL_INPUT_ENABLED);
        pocket.Payload.CustomMode.Should().Be(0);
        pocket.Payload.SystemStatus.Should().Be(MavState.MAV_STATE_ACTIVE);
        pocket.Payload.MavlinkVersion.Should().Be(3);
    }

    [Fact]
    public void TestMessageDeserialization1()
    {
        var buffer = new byte[]
        {
            0xfd, 0x0c, 0x00, 0x00, 0x48, 0x01, 0x01, 0x02, 0x00, 0x00, 0x89, 0x66, 0xbb, 0xf2, 0xdc, 0x1a, 0x06, 0x00,
            0xa4, 0x50, 0x02, 0x01, 0x63, 0x58,
        };
        var pocket = (SystemTimePocket) MavlinkSerialize.Deserialize(buffer,
            MyCommonDialect.Create(MyStandardDialect.Create(MyMinimalDialect.Create())));

        pocket.Payload.TimeBootMs.Should().Be(16928932);
        pocket.Payload.TimeUnixUsec.Should().Be(1718386127758985);
    }

    [Fact]
    public void TestMessageDeserialization2()
    {
        var buffer = new byte[]
        {
            0xfd, 0x10, 0x00, 0x00, 0x6b, 0x01, 0x01, 0x01, 0x00, 0x00, 0x2f, 0xfc, 0x71, 0x53, 0x2f, 0xfc, 0x61, 0x52,
            0x2f, 0xfc, 0x71, 0x57, 0x00, 0x00, 0x38, 0x31, 0x83, 0x3d,
        };
        var pocket = (SysStatusPocket) MavlinkSerialize.Deserialize(buffer,
            MyCommonDialect.Create(MyStandardDialect.Create(MyMinimalDialect.Create())));

        pocket.Payload.OnboardControlSensorsPresent.Should().Be((MavSysStatusSensor) 0x5371fc2f);
        pocket.Payload.OnboardControlSensorsEnabled.Should().Be((MavSysStatusSensor) 0x5261fc2f);
        pocket.Payload.OnboardControlSensorsHealth.Should().Be((MavSysStatusSensor) 0x5771fc2f);
        pocket.Payload.Load.Should().Be(0);
        pocket.Payload.VoltageBattery.Should().Be(12600);
        pocket.Payload.CurrentBattery.Should().Be(0);
        pocket.Payload.BatteryRemaining.Should().Be(0);
        pocket.Payload.DropRateComm.Should().Be(0);
        pocket.Payload.ErrorsComm.Should().Be(0);
        pocket.Payload.ErrorsCount1.Should().Be(0);
        pocket.Payload.ErrorsCount2.Should().Be(0);
        pocket.Payload.ErrorsCount3.Should().Be(0);
        pocket.Payload.ErrorsCount4.Should().Be(0);
    }

    [Fact]
    public void TestMessageDeserialization3()
    {
        var buffer = new byte[]
        {
            0xfd, 0x1c, 0x00, 0x00, 0xb9, 0x01, 0x01, 0x1e, 0x00, 0x00, 0xb4, 0xd1, 0x1e, 0x00, 0x2e, 0x78, 0xa1, 0x3a,
            0x13, 0xcf, 0x87, 0x3a, 0x58, 0xe2, 0xd4, 0xbd, 0xb8, 0xd9, 0xac, 0x39, 0xc0, 0xda, 0xa5, 0x39, 0x84, 0x32,
            0x4f, 0x3a, 0x80, 0xc3,
        };
        var pocket = (AttitudePocket) MavlinkSerialize.Deserialize(buffer,
            MyCommonDialect.Create(MyStandardDialect.Create(MyMinimalDialect.Create())));
    }


    [Fact]
    public void TestMessageDeserialization4()
    {
        var buffer = new byte[]
        {
            0xfd, 0x19, 0x00, 0x00, 0x90, 0x01, 0x01, 0x16, 0x00, 0x00, 0x00, 0xa0, 0x43, 0x45, 0x6a, 0x05, 0xff, 0xff,
            0x53, 0x54, 0x41, 0x54, 0x5f, 0x52, 0x55, 0x4e, 0x54, 0x49, 0x4d, 0x45, 0x00, 0x00, 0x00, 0x00, 0x06, 0xc7,
            0x87,
        };
        var pocket = (ParamValuePocket) MavlinkSerialize.Deserialize(buffer,
            MyCommonDialect.Create(MyStandardDialect.Create(MyMinimalDialect.Create())));

        pocket.Payload.ParamId.Should().Be("STAT_RUNTIME");
    }
}