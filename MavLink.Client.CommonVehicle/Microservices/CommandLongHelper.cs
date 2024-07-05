using MavLink.Serialize;
using MavLink.Serialize.Dialects.Common;

namespace MavLink.Client.CommonVehicle.Microservices;

public class CommandLongHelper : ICommandProvider<CommandLongPocket>
{
    private readonly byte _systemId;
    private readonly byte _componentId;
    private readonly byte _targetSystemId;
    private readonly byte _targetComponentId;
    private readonly MavFrame _frame;
    private byte _sequenceNumber = 0;

    public CommandLongHelper(byte systemId, byte componentId, byte targetSystemId, byte targetComponentId, MavFrame frame)
    {
        _systemId = systemId;
        _componentId = componentId;
        _targetSystemId = targetSystemId;
        _targetComponentId = targetComponentId;
        _frame = frame;
    }
    public CommandLongPocket Create(uint mavCmd, Func<int, float> fieldInitializer)
    {
        var command = new CommandLongPocket(
            isMavlink2: true,
            sequenceNumber: _sequenceNumber++,
            systemId: _systemId,
            componentId: _componentId,
            new CommandLongPayload()
            {
                Command = (CommonDialect.MavCmd)mavCmd,
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