using FluentAssertions;
using MavLink.Serialize.Dialects;


namespace MavLink.Serialize.Generator.Tests;

[Dialect("minimal.xml")]
public partial class MyMinimalDialect
{
    
}

public class GeneratorIntegrationTest
{
    [Fact]
    public void IsDefaultDenerated()
    {
        MyMinimalDialect.Default.Should().NotBeNull();
    }
    
    [Fact]
    public void TestMessageDeserialization()
    {
        var buffer = new byte[]
        {
            0xfd, 0x09, 0x00, 0x00, 0x12, 0xff, 0xbe, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x08, 0xc0, 0x04, 0x03, 0x5d, 0xc6, 
        };
        var pocket = (HeartbeatPocket)MavlinkSerialize.Deserialize(buffer, MyMinimalDialect.Default);

        pocket.MessageId.Should().Be(0);
        pocket.SequenceNumber.Should().Be(18);
        pocket.SystemId.Should().Be(255);
        pocket.ComponentId.Should().Be(190);
        pocket.Payload.Type.Should().Be(MavType.MAV_TYPE_GCS);
        pocket.Payload.Autopilot.Should().Be(MavAutopilot.MAV_AUTOPILOT_INVALID);
        pocket.Payload.BaseMode.Should().Be(MavModeFlag.MAV_MODE_FLAG_SAFETY_ARMED | MavModeFlag.MAV_MODE_FLAG_MANUAL_INPUT_ENABLED);
        pocket.Payload.CustomMode.Should().Be(0);
        pocket.Payload.SystemStatus.Should().Be(MavState.MAV_STATE_ACTIVE);
        pocket.Payload.MavlinkVersion.Should().Be(3);
    }
}