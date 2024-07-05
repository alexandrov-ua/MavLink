using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MavLink.Client.DemoApp.ViewModels;
using ReactiveUI;

namespace MavLink.Client.DemoApp.Views;

public partial class ConnectToVehicleWindow : ReactiveWindow<ConnectToVehicleViewModel>
{
    public ConnectToVehicleWindow()
    {
        InitializeComponent();
        this.WhenActivated(action =>
            action(ViewModel!.SelectVehicle.Subscribe((Action<object>)Close)));
    }
}