using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace MavLink.Client.DemoApp.Controls;

public partial class HeadingIndicator : UserControl
{
    public static readonly StyledProperty<float> YawAngleProperty =
        AvaloniaProperty.Register<HeadingIndicator, float>(nameof(YawAngle),
            defaultValue: 0f,
            defaultBindingMode: BindingMode.TwoWay);

    public float YawAngle
    {
        get => GetValue(YawAngleProperty);
        set => SetValue(YawAngleProperty, value);
    }
    
    public HeadingIndicator()
    {
        InitializeComponent();
    }
}