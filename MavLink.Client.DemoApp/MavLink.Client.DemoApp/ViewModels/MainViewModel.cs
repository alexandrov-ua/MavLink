using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Mapsui.Providers.Wfs.Utilities;
using MavLink.Client.DemoApp.Dialects;
using ReactiveUI;

namespace MavLink.Client.DemoApp.ViewModels;

public class MainViewModel : ViewModelBase, IDisposable
{
    private ValueTuple<float, float> _center;
    private readonly MavLinkReactiveClient _client;


    public MainViewModel(MavLinkReactiveClient client)
    {
        _client = client;
        _t = _client.Subscribe(t =>
        {
            if (t is TerrainCheckPocket tr)
            {
                Center = (tr.Payload.Lat, tr.Payload.Lon);
                Console.WriteLine("set center");
            }
        });

        // var t  = _client.OfType<TerrainDataPocket>().Subscribe(t =>
        // {
        //     Center = (t.Payload.Lat, t.Payload.Lon);
        //     Console.WriteLine("Center");
        // });
        //
        // _messageCount = _client.Count().ToProperty(this, t => t.MessageCounr);
    }
    
    public ValueTuple<float, float> Center
    {
        get => _center;
        set => this.RaiseAndSetIfChanged(ref _center, value);
    }

    private ObservableAsPropertyHelper<int> _messageCount;
    private readonly IDisposable _t;
    public int MessageCounr => _messageCount.Value;

    public void SetCenter()
    {
            Center = (Center.Item1 + 1, Center.Item2);
    }


    public void Dispose()
    {
        _client.Dispose();
        _t.Dispose();
    }
}