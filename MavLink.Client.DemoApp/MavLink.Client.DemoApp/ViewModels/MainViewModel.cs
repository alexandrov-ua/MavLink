using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Mapsui.Providers.Wfs.Utilities;
using MavLink.Client.DemoApp.Dialects;
using ReactiveUI;

namespace MavLink.Client.DemoApp.ViewModels;

public class MainViewModel : ViewModelBase, IActivatableViewModel
{
    private ValueTuple<float, float> _center;
    private readonly MavLinkReactiveClient _client;


    public MainViewModel(MavLinkReactiveClient client)
    {
        _client = client;
        this.WhenActivated((CompositeDisposable disposable) =>
        {
            _client.Subscribe(t =>
            {
                if (t is TerrainCheckPocket tr)
                {
                    Center = (tr.Payload.Lat, tr.Payload.Lon);
                    Console.WriteLine("set center");
                }
            }).DisposeWith(disposable);

            //_messageCount = _client.Count().ToProperty(this, t=>t.MessageCounr).DisposeWith(disposable);
        });
    }
    
    public ValueTuple<float, float> Center
    {
        get => _center;
        set => this.RaiseAndSetIfChanged(ref _center, value);
    }

    // private ObservableAsPropertyHelper<int> _messageCount;
    // public int MessageCounr => _messageCount.Value;

    public void SetCenter()
    {
            Center = (Center.Item1 + 1, Center.Item2);
    }
    

    public ViewModelActivator Activator { get; } = new ViewModelActivator();
}