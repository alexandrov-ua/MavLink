using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using MavLink.Client.DemoApp.ViewModels;
using MavLink.Client.DemoApp.Views;
using Microsoft.Extensions.DependencyInjection;

namespace MavLink.Client.DemoApp;

public partial class App : Application, IDisposable
{
    private ServiceProvider _services;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();
        collection.AddCommonServices();

        _services = collection.BuildServiceProvider();

        var vm = _services.GetRequiredService<MainViewModel>()!;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = vm
            };
            desktop.Exit += (sender, args) =>
            {
                _services.Dispose();
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = vm,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public void Dispose()
    {
        _services.Dispose();
    }
}