using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using MavLink.Client.CommonVehicle;
using MavLink.Client.DemoApp.ViewModels;
using ReactiveUI;

namespace MavLink.Client.DemoApp.Views;

public partial class MainView : ReactiveUserControl<MainViewModel>
{
    public MainView()
    {
        InitializeComponent();
        this.WhenActivated((Action<IDisposable> action) => 
            action(ViewModel!.ConnectToVehicleDialog.RegisterHandler(DoShowDialogAsync)));
        this.WhenActivated((Action<IDisposable> action) => 
            action(Observable.FromAsync(ViewModel!.ShowDialog, RxApp.MainThreadScheduler).Publish().Connect()));
    }
    
    private async Task DoShowDialogAsync(InteractionContext<IVehicleMultiplexer, IVehicle?> interaction)
    {
        var dialog = new ConnectToVehicleWindow();
        dialog.DataContext = new ConnectToVehicleViewModel(interaction.Input);

        var result = await dialog.ShowDialog<IVehicle?>((Window)this.Parent);
        interaction.SetOutput(result);
    }
}