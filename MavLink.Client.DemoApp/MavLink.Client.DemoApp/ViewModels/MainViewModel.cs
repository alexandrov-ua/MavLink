using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using MavLink.Serialize.Dialects.Common;
using ReactiveUI;

namespace MavLink.Client.DemoApp.ViewModels;

public class MainViewModel : ViewModelBase, IActivatableViewModel
{
    private VehiclePosition _position;
    private readonly MavLinkReactiveClient _client;


    public MainViewModel(MavLinkReactiveClient client)
    {
        _client = client;
        this.WhenActivated((CompositeDisposable disposable) =>
        {
            _client.OfType<GlobalPositionIntPocket>().Subscribe(t =>
            {
                var payload = t.Payload;
                Position = new VehiclePosition(
                    payload.Lon / 10000000d, 
                    payload.Lat / 10000000d, 
                    payload.Alt / 1000d,
                    payload.Hdg / 100d);
            }).DisposeWith(disposable);
        });
    }

    public VehiclePosition Position
    {
        get => _position;
        set => this.RaiseAndSetIfChanged(ref _position, value);
    }

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
}