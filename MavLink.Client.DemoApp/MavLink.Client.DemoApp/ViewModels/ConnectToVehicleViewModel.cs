using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using MavLink.Client.CommonVehicle;
using MavLink.Serialize.Dialects.ArduPilotMega;
using ReactiveUI;

namespace MavLink.Client.DemoApp.ViewModels;

public class ConnectToVehicleViewModel : ViewModelBase, IActivatableViewModel, IDisposable
{
    private readonly IVehicleMultiplexer _vehicleMultiplexer;

    private string _connectionString = "udp://0.0.0.0:14550";

    public string ConnectionString
    {
        get => _connectionString;
        set => this.RaiseAndSetIfChanged(ref _connectionString, value);
    }

    private bool _isConnected = false;

    public bool IsConnected
    {
        get => _isConnected;
        set => this.RaiseAndSetIfChanged(ref _isConnected, value);
    }

    private bool _isLoading = false;

    public bool IsLoading
    {
        get => _isLoading;
        set => this.RaiseAndSetIfChanged(ref _isLoading, value);
    }


    private BehaviorSubject<IEnumerable<VehicleInfo>> _availableVehicles =
        new BehaviorSubject<IEnumerable<VehicleInfo>>([]);
    private IDisposable? _availableVehiclesSub = null;
    public IObservable<IEnumerable<VehicleInfo>> AvailableVehicles => _availableVehicles;

    public ReactiveCommand<Unit, Unit> DoConnect { get; }

    public ReactiveCommand<VehicleInfo, IVehicle> SelectVehicle { get; }

    public ConnectToVehicleViewModel(IVehicleMultiplexer interactionInput)
    {
        _vehicleMultiplexer = interactionInput;
        IsConnected = _vehicleMultiplexer.IsConnected;
        ConnectionString = _vehicleMultiplexer.ConnectionString ?? _connectionString;
        if (_vehicleMultiplexer.IsConnected)
        {
            _availableVehiclesSub = _vehicleMultiplexer.AvailableVehicles.Subscribe(_availableVehicles);
        }

        DoConnect = ReactiveCommand.CreateFromTask(t => DoConnectImpl());
        SelectVehicle = ReactiveCommand.Create<VehicleInfo, IVehicle>(SelectVehicleImpl);
        this.WhenActivated((CompositeDisposable disposable) =>
        {
            disposable.Add(this);
        });
    }

    public void DoReconnect()
    {
        IsConnected = false;
        _vehicleMultiplexer.Disconnect();
        _availableVehiclesSub?.Dispose();
        _availableVehiclesSub = null;
        _availableVehicles.OnNext([]);
    }

    public async Task DoConnectImpl()
    {
        IsLoading = true;
        IsConnected = true;
        _vehicleMultiplexer.Connect(ConnectionString, ArduPilotMegaDialect.Default);
        _availableVehiclesSub = _vehicleMultiplexer.AvailableVehicles.Subscribe(_availableVehicles);
        await _vehicleMultiplexer.AvailableVehicles.Where(t => t.Any()).FirstAsync();

        IsLoading = false;
    }

    public IVehicle SelectVehicleImpl(VehicleInfo vehicleInfo)
    {
        return _vehicleMultiplexer.ConnectToVehicle(vehicleInfo);
    }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();

    public void Dispose()
    {
        DoConnect.Dispose();
        SelectVehicle.Dispose();
        _availableVehiclesSub?.Dispose();
        _availableVehicles.Dispose();
    }
}