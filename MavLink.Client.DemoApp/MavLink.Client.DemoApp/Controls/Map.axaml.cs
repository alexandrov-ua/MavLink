using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Fetcher;
using Mapsui.Layers;
using Mapsui.Layers.AnimatedLayers;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.UI.Avalonia;
using ReactiveUI;

namespace MavLink.Client.DemoApp.Controls;

public partial class Map : ReactiveUserControl<object>
{
    public static readonly StyledProperty<ValueTuple<float, float>> CenterProperty =
        AvaloniaProperty.Register<Map, ValueTuple<float, float>>(nameof(Center), defaultValue: new ValueTuple<float, float>(0.0f,0.0f),
            defaultBindingMode: BindingMode.TwoWay);

    private readonly MapControl _mapControl;

    public ValueTuple<float, float> Center
    {
        get => GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    } 
    
    public Map()
    {

        //map.CRS = "EPSG:3857";
        //map.Navigator.CenterOnAndZoomTo(new MPoint(2776952, 8442653), map.Navigator.Resolutions[18]);
        
        InitializeComponent();
        _mapControl = new Mapsui.UI.Avalonia.MapControl();
        _mapControl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        var busPointProvider = new BusPointProvider();
        _mapControl.Map.Layers.Add(new AnimatedPointLayer(busPointProvider)
        {
            Name = "Buses",
            Style = new LabelStyle
            {
                BackColor = new Brush(Color.Black),
                ForeColor = Color.White,
                Text = "Bus",
            }
        });
        Content = _mapControl;

        this.WhenActivated(d =>
        {
            this.WhenAnyValue(t => t.Center).Subscribe(t => busPointProvider.SetVal((t.Item1, t.Item2)))
                .DisposeWith(d);
        });
        
    }
}

internal sealed class BusPointProvider : MemoryProvider, IDynamic, IDisposable
{
    public event DataChangedEventHandler? DataChanged;

    private (double Lon, double Lat) _previousCoordinates = (24.945831, 60.192059);

    public void SetVal((double Lon, double Lat) point)
    {
        _previousCoordinates.Lat = point.Lat;
        _previousCoordinates.Lon = point.Lon;
        OnDataChanged();
    }

    void IDynamic.DataHasChanged()
    {
        OnDataChanged();
    }

    private void OnDataChanged()
    {
        DataChanged?.Invoke(this, new DataChangedEventArgs());
    }

    public override Task<IEnumerable<IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var busFeature = new PointFeature(SphericalMercator.FromLonLat(_previousCoordinates.Lon, _previousCoordinates.Lat).ToMPoint());
        busFeature["ID"] = "bus";
        return Task.FromResult((IEnumerable<IFeature>)[busFeature]);
    }

    public void Dispose()
    {
        //_timer.Dispose();
    }
}