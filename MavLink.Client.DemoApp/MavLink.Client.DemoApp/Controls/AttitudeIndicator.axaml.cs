using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace MavLink.Client.DemoApp.Controls;

public partial class AttitudeIndicator : UserControl
{
    public static readonly StyledProperty<float> RollAngleProperty =
        AvaloniaProperty.Register<AttitudeIndicator, float>(nameof(RollAngle),
            defaultValue: 0f,
            defaultBindingMode: BindingMode.TwoWay);

    public float RollAngle
    {
        get => GetValue(RollAngleProperty);
        set => SetValue(RollAngleProperty, value);
    }
    
    public static readonly StyledProperty<float> PitchAngleProperty =
        AvaloniaProperty.Register<AttitudeIndicator, float>(nameof(PitchAngle),
            defaultValue: 0f,
            defaultBindingMode: BindingMode.TwoWay);

    public float PitchAngle
    {
        get => GetValue(PitchAngleProperty);
        set => SetValue(PitchAngleProperty, value);
    }
    
    public AttitudeIndicator()
    {
        InitializeComponent();
    }
}

public static class MyConverters 
{
    public static FuncValueConverter<float, float> PitchAngleConverter { get; } = 
        new FuncValueConverter<float, float> (num => num - 50);
    
    public static FuncValueConverter<float, float> RollAngleConverter { get; } = 
        new FuncValueConverter<float, float> (num => num * -1.0f);

}
