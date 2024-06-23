using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MavLink.Client.DemoApp.ViewModels;
using ReactiveUI;

namespace MavLink.Client.DemoApp.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
    }
}