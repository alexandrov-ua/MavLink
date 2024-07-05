using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MavLink.Client.CommonVehicle;
using MavLink.Serialize;
using MavLink.Serialize.Dialects.Common;
using MavLink.Serialize.Messages;
using ReactiveUI;

namespace MavLink.Client.DemoApp.ViewModels;

public class MainViewModel : ViewModelBase, IActivatableViewModel
{
    private IVehicleMultiplexer _vehicleMultiplexer = new VehicleMultiplexer();

    public MainViewModel()
    {
        ConnectToVehicleDialog = new Interaction<IVehicleMultiplexer, IVehicle>();
        MainScreenViewModel = new MainScreenViewModel(new StubVehicle(), () => ShowDialog());
        this.WhenActivated((CompositeDisposable d) =>
        {
        });
    }

    public Interaction<IVehicleMultiplexer, IVehicle?> ConnectToVehicleDialog { get; }

    private MainScreenViewModel _mainScreenViewModel;

    public MainScreenViewModel MainScreenViewModel
    {
        get => _mainScreenViewModel;
        set => this.RaiseAndSetIfChanged(ref _mainScreenViewModel, value);
    }

    public async Task ShowDialog()
    {
        var vehicle = await ConnectToVehicleDialog.Handle(_vehicleMultiplexer);
        MainScreenViewModel.Activator.Deactivate();
        MainScreenViewModel = new MainScreenViewModel(vehicle ?? new StubVehicle(), () => ShowDialog());
    }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
}

public class StubVehicle : IVehicle
{
    public IDisposable Subscribe(IObserver<IPocket<IPayload>> observer)
    {
        return new Subject<IPocket<IPayload>>().Subscribe(observer);
    }

    public void Dispose()
    {
    }

    public byte SystemId { get; }
    public byte ComponentId { get; }
    public byte TargetSystemId { get; }
    public byte TargetComponentId { get; }
    public byte SequenceNumber { get; }

    public Task SendCommand(Func<ICommandProvider<CommandLongPocket>, CommandLongPocket> func)
    {
        return Task.CompletedTask;
    }
}