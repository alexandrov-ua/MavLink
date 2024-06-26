using System;
using System.Linq;
using MavLink.Client.DemoApp.Dialects;
using MavLink.Client.DemoApp.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace MavLink.Client.DemoApp;

public static class AppServices
{
    public static void AddCommonServices(this IServiceCollection collection, string[] args)
    {
        collection.AddTransient<MainViewModel>();
        
        var client = new MavLinkReactiveClient(MavLinkClient.Create(args.FirstOrDefault() ??
            "udp://0.0.0.0:14550",
            ArduPilotMegaDialect.Default));
        collection.AddSingleton<MavLinkReactiveClient>(client);
    }
}