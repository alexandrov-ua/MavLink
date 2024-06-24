using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Data;
using Avalonia.ReactiveUI;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Projections;
using Mapsui.Styles;
using Mapsui.UI.Avalonia;
using Mapsui.Widgets;
using Mapsui.Widgets.ButtonWidget;
using ReactiveUI;

namespace MavLink.Client.DemoApp.Controls;

public partial class Map : ReactiveUserControl<object>
{
    public static readonly StyledProperty<VehiclePosition> CenterProperty =
        AvaloniaProperty.Register<Map, VehiclePosition>(nameof(Center),
            defaultValue: new VehiclePosition(0.0d, 0.0d, 0.0d, 0.0d),
            defaultBindingMode: BindingMode.TwoWay);

    public VehiclePosition Center
    {
        get => GetValue(CenterProperty);
        set => SetValue(CenterProperty, value);
    }

    public Map()
    {
        InitializeComponent();
        var mapControl = new Mapsui.UI.Avalonia.MapControl();
        mapControl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
        var busPointProvider = new VehiclePositionProvider();
        mapControl.Map?.Layers.Add(VehicleLayerHelper.CreateAnimatedPointLayer(busPointProvider));

        var centerButton = CreateButton("Center", Mapsui.Widgets.VerticalAlignment.Top,
            Mapsui.Widgets.HorizontalAlignment.Left);
        centerButton.WidgetTouched += (s, a) =>
        {
            var point = new MPoint(Center.Longitude, Center.Latitude);
            var coordinate = SphericalMercator.FromLonLat(point.X, point.Y).ToMPoint();

            var res = mapControl.Map?.Navigator.Resolutions[17];
            mapControl.Map?.Navigator.CenterOnAndZoomTo(coordinate, res!.Value, 1000);
        };
        mapControl.Map?.Widgets.Add(centerButton);

        Content = mapControl;

        this.WhenActivated(d =>
        {
            this.WhenAnyValue(t => t.Center).Subscribe(t => busPointProvider.SetVal(Center))
                .DisposeWith(d);
        });
    }

    private static ButtonWidget CreateButton(string text,
        VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment)
    {
        return new ButtonWidget()
        {
            Text = text,
            VerticalAlignment = verticalAlignment,
            HorizontalAlignment = horizontalAlignment,
            MarginX = 30,
            MarginY = 30,
            PaddingX = 10,
            PaddingY = 8,
            CornerRadius = 8,
            BackColor = new Color(0, 123, 255),
            TextColor = Color.White,
        };
    }
}