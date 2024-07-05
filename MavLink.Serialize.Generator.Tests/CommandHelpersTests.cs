using FluentAssertions;
using MavLink.Client.CommonVehicle.Microservices;
using MavLink.Serialize.Dialects.Common;

namespace MavLink.Serialize.Generator.Tests;


public class CommandHelpersTests
{
    [Fact]
    public void CommandLongHelper()
    {
        var commandHelper = new CommandLongHelper(255, 170, 1,1, MavLink.Serialize.Dialects.Common.MavFrame.MAV_FRAME_GLOBAL);
        var command = commandHelper.CreateMavCmdNavLand(123, PrecisionLandMode.PRECISION_LAND_MODE_REQUIRED, 12, 1, 2, 3);

        command.SystemId.Should().Be(255);
        command.ComponentId.Should().Be(170);
        command.SequenceNumber.Should().Be(0);
        command.Payload.Confirmation.Should().Be(0);
        command.Payload.Command.Should().Be(CommonDialect.MavCmd.MAV_CMD_NAV_LAND);
        command.Payload.TargetSystem.Should().Be(1);
        command.Payload.TargetComponent.Should().Be(1);
        command.Payload.Param1.Should().Be(123);
        command.Payload.Param2.Should().Be(2);
        command.Payload.Param3.Should().Be(0);
        command.Payload.Param4.Should().Be(12);
        command.Payload.Param5.Should().Be(1);
        command.Payload.Param6.Should().Be(2);
        command.Payload.Param7.Should().Be(3);
    }
    
    [Fact]
    public void CommandIntHelper()
    {
        var commandHelper = new CommandIntHelper(255, 170, 1,1, MavLink.Serialize.Dialects.Common.MavFrame.MAV_FRAME_GLOBAL);
        var command = commandHelper.CreateMavCmdNavLand(123, PrecisionLandMode.PRECISION_LAND_MODE_REQUIRED, 12, 1, 2, 3);

        command.SystemId.Should().Be(255);
        command.ComponentId.Should().Be(170);
        command.SequenceNumber.Should().Be(0);
        command.Payload.Command.Should().Be(CommonDialect.MavCmd.MAV_CMD_NAV_LAND);
        command.Payload.TargetSystem.Should().Be(1);
        command.Payload.TargetComponent.Should().Be(1);
        command.Payload.Param1.Should().Be(123);
        command.Payload.Param2.Should().Be(2);
        command.Payload.Param3.Should().Be(0);
        command.Payload.Param4.Should().Be(12);
        command.Payload.X.Should().Be(1);
        command.Payload.Y.Should().Be(2);
        command.Payload.Z.Should().Be(3);
    }
}