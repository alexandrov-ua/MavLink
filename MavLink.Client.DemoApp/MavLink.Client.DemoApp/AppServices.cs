using System;
using MavLink.Client.DemoApp.Dialects;
using MavLink.Client.DemoApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MavLink.Client.DemoApp;

public static class AppServices
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddTransient<MainViewModel>();
        
        var client = new MavLinkReactiveClient(MavLinkClient.Create(
            "udp://127.0.0.1:14550",
            ArduPilotMegaDialect.Default));
        collection.AddSingleton<MavLinkReactiveClient>(client);
    }
}