using FluentAssertions;
using MavLink.Serialize.Messages;

namespace MavLink.Serialize.Generator.Tests;

public class CommandLongHelper : ICommandProvider<CommandLongPayload>
{
    private readonly byte _systemId;
    private readonly byte _componentId;
    private readonly byte _targetSystemId;
    private readonly byte _targetComponentId;
    private readonly MavFrame _frame;

    public CommandLongHelper(byte systemId, byte componentId, byte targetSystemId, byte targetComponentId, MavFrame frame)
    {
        _systemId = systemId;
        _componentId = componentId;
        _targetSystemId = targetSystemId;
        _targetComponentId = targetComponentId;
        _frame = frame;
    }
    public IPocket<CommandLongPayload> Create(uint mavCmd, Func<int, float> fieldInitializer)
    {
        var command = new CommandLongPocket(
            isMavlink2: true,
            sequenceNumber: 0,
            systemId: _systemId,
            componentId: _componentId,
            new CommandLongPayload()
            {
                Command = (MyCommonDialect.MavCmd)mavCmd,
                Confirmation = 0,
                TargetComponent = _targetComponentId,
                TargetSystem = _targetSystemId,
                Param1 = fieldInitializer(1),
                Param2 = fieldInitializer(2),
                Param3 = fieldInitializer(3),
                Param4 = fieldInitializer(4),
                Param5 = fieldInitializer(5),
                Param6 = fieldInitializer(6),
                Param7 = fieldInitializer(7),
            });
        return command;
    }
}

public class CommandIntHelper : ICommandProvider<CommandIntPayload>
{
    private readonly byte _systemId;
    private readonly byte _componentId;
    private readonly byte _targetSystemId;
    private readonly byte _targetComponentId;
    private readonly MavFrame _frame;

    public CommandIntHelper(byte systemId, byte componentId, byte targetSystemId, byte targetComponentId, MavFrame frame)
    {
        _systemId = systemId;
        _componentId = componentId;
        _targetSystemId = targetSystemId;
        _targetComponentId = targetComponentId;
        _frame = frame;
    }
    public IPocket<CommandIntPayload> Create(uint mavCmd, Func<int, float> fieldInitializer)
    {
        var command = new CommandIntPocket(
            isMavlink2: true,
            sequenceNumber: 0,
            systemId: _systemId,
            componentId: _componentId,
            new CommandIntPayload()
            {
                Command = (MyCommonDialect.MavCmd)mavCmd,
                TargetComponent = _targetComponentId,
                TargetSystem = _targetSystemId,
                Param1 = fieldInitializer(1),
                Param2 = fieldInitializer(2),
                Param3 = fieldInitializer(3),
                Param4 = fieldInitializer(4),
                X = (int)fieldInitializer(5),
                Y = (int)fieldInitializer(6),
                Z = fieldInitializer(7),
            });
        return command;
    }
}

public class Test
{
    [Fact]
    public void Foo()
    {
        var commandHelper = new CommandLongHelper(255, 170, 1,1, MavFrame.MAV_FRAME_GLOBAL);
        var command = commandHelper.CreateMavCmdNavLand(123, PrecisionLandMode.PRECISION_LAND_MODE_REQUIRED, 12, 1, 2, 3);

        command.SystemId.Should().Be(255);
        command.ComponentId.Should().Be(170);
        command.SequenceNumber.Should().Be(0);
        command.Payload.Confirmation.Should().Be(0);
        command.Payload.Command.Should().Be(MyCommonDialect.MavCmd.MAV_CMD_NAV_LAND);
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
    public void Bar()
    {
        var commandHelper = new CommandIntHelper(255, 170, 1,1, MavFrame.MAV_FRAME_GLOBAL);
        var command = commandHelper.CreateMavCmdNavLand(123, PrecisionLandMode.PRECISION_LAND_MODE_REQUIRED, 12, 1, 2, 3);

        command.SystemId.Should().Be(255);
        command.ComponentId.Should().Be(170);
        command.SequenceNumber.Should().Be(0);
        command.Payload.Command.Should().Be(MyCommonDialect.MavCmd.MAV_CMD_NAV_LAND);
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