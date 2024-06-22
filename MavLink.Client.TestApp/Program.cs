using System.Reactive.Linq;
using MavLink.Client;
using MavLink.Client.TestApp.Dialects;


using var client =
    new MavLinkReactiveClient(MavLinkClient.Create(args.FirstOrDefault() ?? "udp://127.0.0.1:14550",
        ArduPilotMegaDialect.Default));

using var _ = client.GroupBy(t => t.MessageName)
    .Select(t => t.TimeInterval())
    .SelectMany(t => t)
    .Buffer(TimeSpan.FromSeconds(1))
    .Subscribe((t) =>
    {
        Console.Clear();
        Console.WriteLine("====================================");
        foreach (var i in t.GroupBy(z => z.Value.MessageName)
                     .Select(z => new
                     {
                         Value = z.First().Value,
                         Count = z.Count(),
                         Avg = z.Average(q => q.Interval.TotalMilliseconds)
                     })
                     .OrderBy(t => t.Count))
        {
            Console.WriteLine(
                $"{i.Value.MessageName,40}{i.Value.MessageId,10}{i.Count,10}{(1000 / i.Avg).ToString("F"),20} Hz");
        }

        Console.WriteLine();
    });

Console.ReadKey();