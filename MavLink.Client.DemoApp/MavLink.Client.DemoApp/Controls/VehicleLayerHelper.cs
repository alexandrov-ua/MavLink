using System;
using System.Collections.ObjectModel;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Layers.AnimatedLayers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Styles.Thematics;
using Mapsui.Utilities;

namespace MavLink.Client.DemoApp.Controls;

public static class VehicleLayerHelper
{
    private static Lazy<int> _bitmapId = 
        new Lazy<int>(BitmapRegistry.Instance.Register(
        SvgHelper.LoadSvgPicture(
            "<svg version=\"1.1\" id=\"icons\" xmlns=\"http://www.w3.org/2000/svg\" x=\"0\" y=\"0\" viewBox=\"0 0 128 128\" style=\"enable-background:new 0 0 128 128\" xml:space=\"preserve\"><style>.st0,.st1{display:none;fill:#191919}.st1,.st3{fill-rule:evenodd;clip-rule:evenodd}.st3,.st4{display:inline;fill:#191919}</style><g id=\"row1\"><path id=\"nav:2_3_\" d=\"M64 1 17.9 127 64 99.8l46.1 27.2L64 1zm0 20.4 32.6 89.2L64 91.3V21.4z\" style=\"fill:#191919\"/></g></svg>"),
        "fullName123"));
    
    public static ILayer CreateAnimatedPointLayer(IProvider provider)
    {
        return new AnimatedPointLayer(provider)
        {
            Name = "Animated Points",
            Style = new ThemeStyle(f => CreateVehicleStyle(0.25, f))
        };
    }

    private static IStyle CreateVehicleStyle(double scale, IFeature feature)
    {
        return new StyleCollection()
        {
            Styles = new Collection<IStyle>()
            {
                new SymbolStyle()
                {
                    BitmapId = _bitmapId.Value,
                    BlendModeColor = new Color(255,0,0),
                    SymbolScale = scale,
                    SymbolOffset = new RelativeOffset(0.0, 0.0),
                    Opacity = 1f,
                    SymbolRotation = (double)feature["rotation"]!,
                },
                new LabelStyle
                {
                    Offset = new RelativeOffset(0,2.5),
                    BackColor = new Brush(Color.Black),
                    ForeColor = Color.White,
                    Text = (string)feature["ID"]!,
                    Opacity = 0.5f,
                }
            }
        };
    }
}