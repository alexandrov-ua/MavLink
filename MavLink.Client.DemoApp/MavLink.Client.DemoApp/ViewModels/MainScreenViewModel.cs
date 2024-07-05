using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using MavLink.Client.CommonVehicle;
using MavLink.Serialize.Dialects.Common;
using MavLink.Serialize.Messages;
using ReactiveUI;

namespace MavLink.Client.DemoApp.ViewModels;

public class MainScreenViewModel : ViewModelBase, IActivatableViewModel
{
    
    private readonly IVehicle _vehicle;
    private readonly Func<Task> _showVehicleDialogCallback;


    public MainScreenViewModel(IVehicle vehicle, Func<Task> showVehicleDialogCallback)
    {
        _vehicle = vehicle;
        _showVehicleDialogCallback = showVehicleDialogCallback;
        this.WhenActivated((CompositeDisposable disposable) =>
        {
            _vehicle.OfType<GlobalPositionIntPocket>().Subscribe(t =>
            {
                var payload = t.Payload;
                Position = new VehiclePosition(
                    payload.Lon / 10000000d, 
                    payload.Lat / 10000000d, 
                    payload.Alt / 1000d,
                    payload.Hdg / 100d);
            }).DisposeWith(disposable);

            _vehicle.OfType<AttitudePocket>().Subscribe(t =>
            {
                Attitude = new VehicleAttitude(t.Payload.Roll * 180 / Math.PI,
                    t.Payload.Pitch * 180 / Math.PI,
                    t.Payload.Yaw * 180 / Math.PI);
            }).DisposeWith(disposable);
        });

        ExecuteCommand = ReactiveCommand.CreateFromTask(MyCommand);
        ExecuteCommand.ThrownExceptions.Subscribe(exception => Console.WriteLine(exception.ToString()));
    }
    private VehiclePosition _position;
    public VehiclePosition Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }
    private VehicleAttitude _attitude;
    public VehicleAttitude Attitude
    {
        get => _attitude;
        set => this.RaiseAndSetIfChanged(ref _attitude, value);
    }

    public ReactiveCommand<Unit,Unit> ExecuteCommand { get; set; }

    public async Task MyCommand()
    {
        await _vehicle.SendCommand(t => t.CreateMavCmdDoSetMode((MavMode)1, 4, 0));
        await _vehicle.SendCommand(t => t.CreateMavCmdComponentArmDisarm(1,0));
        await _vehicle.SendCommand(t => t.CreateMavCmdNavTakeoff(0, 0, 0, 0, 10));
        await Task.Delay(1000);
    }

    public async Task ShowVehicleDialog()
    {
        await _showVehicleDialogCallback();
    }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
}

public record VehicleAttitude(double Roll, double Pitch, double Yaw);