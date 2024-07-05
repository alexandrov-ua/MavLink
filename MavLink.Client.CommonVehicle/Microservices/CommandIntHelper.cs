using MavLink.Serialize;
using MavLink.Serialize.Dialects.Common;

namespace MavLink.Client.CommonVehicle.Microservices;

public class CommandIntHelper: ICommandProvider<CommandIntPocket>
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
    public CommandIntPocket Create(uint mavCmd, Func<int, float> fieldInitializer)
    {
        var command = new CommandIntPocket(
            isMavlink2: true,
            sequenceNumber: 0,
            systemId: _systemId,
            componentId: _componentId,
            new CommandIntPayload()
            {
                Command = (CommonDialect.MavCmd)mavCmd,
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