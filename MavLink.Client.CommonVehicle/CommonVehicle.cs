using System.Reactive.Disposables;
using System.Reactive.Linq;
using MavLink.Serialize;
using MavLink.Serialize.Dialects.Common;
using MavLink.Serialize.Messages;

namespace MavLink.Client.CommonVehicle;

public interface IVehicle : IObservable<IPocket<IPayload>>, IDisposable
{
    byte SystemId { get; }
    byte ComponentId { get; }
    byte TargetSystemId { get; }
    byte TargetComponentId { get; }
    byte SequenceNumber { get; }
    Task SendCommand(Func<ICommandProvider<CommandLongPocket>, CommandLongPocket> func);
}

public class CommonVehicle : ICommandProvider<CommandLongPocket>, IVehicle
{
    public byte SystemId { get; }
    public byte ComponentId { get; }
    public byte TargetSystemId { get; }
    public byte TargetComponentId { get; }

    public byte SequenceNumber => _sequenceNumber;

    private readonly IMavLinkReactiveClient _mavLinkReactiveClient;
    private readonly MavFrame _frame;
    private byte _sequenceNumber = 0;
    private readonly IObservable<IPocket<IPayload>> _receivedMessages;
    private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

    public CommonVehicle(IMavLinkReactiveClient mavLinkReactiveClient, byte targetSystemId, byte targetComponentId,
        byte systemId = 255, byte componentId = 190, MavFrame frame = MavFrame.MAV_FRAME_GLOBAL)
    {
        _mavLinkReactiveClient = mavLinkReactiveClient;
        SystemId = systemId;
        ComponentId = componentId;
        TargetSystemId = targetSystemId;
        TargetComponentId = targetComponentId;
        _frame = frame;

        var receivedMessages =
            _mavLinkReactiveClient.Where(t => t.SystemId == TargetSystemId && t.ComponentId == TargetComponentId)
                .Publish();
        _receivedMessages = receivedMessages;
        _compositeDisposable.Add(receivedMessages.Connect());
    }

    public async Task SendCommand(Func<ICommandProvider<CommandLongPocket>, CommandLongPocket> func)
    {
        var result = 
            await _mavLinkReactiveClient.SendWithResponse<CommandLongPocket, CommandAckPocket>(
                func(this),
                (req, resp) => 
                    resp.Payload.Command == req.Payload.Command && 
                    resp.SystemId == req.Payload.TargetSystem && 
                    resp.ComponentId == req.Payload.TargetComponent);
        if (result.Payload.Result != MavResult.MAV_RESULT_ACCEPTED)
        {
            throw new InvalidOperationException($"Response is {result.Payload.Result.ToString()}");
        }
    }
    
    public CommandLongPocket Create(uint mavCmd, Func<int, float> fieldInitializer)
    {
        var command = new CommandLongPocket(
            isMavlink2: true,
            sequenceNumber: _sequenceNumber++,
            systemId: SystemId,
            componentId: ComponentId,
            new CommandLongPayload()
            {
                Command = (CommonDialect.MavCmd) mavCmd,
                Confirmation = 0,
                TargetComponent = TargetComponentId,
                TargetSystem = TargetSystemId,
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

    public IDisposable Subscribe(IObserver<IPocket<IPayload>> observer)
    {
        return _receivedMessages.Subscribe(observer);
    }

    public void Dispose()
    {
        _mavLinkReactiveClient.Dispose();
        _compositeDisposable.Dispose();
    }
}
