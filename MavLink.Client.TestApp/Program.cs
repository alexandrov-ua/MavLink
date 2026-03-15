using System.Reactive.Linq;
using MavLink.Client;
using MavLink.Serialize.Dialects.ArduPilotMega;
using MavLink.Serialize.Dialects.Common;


using var client =
    new MavLinkReactiveClient(MavLinkClient.Create(args.FirstOrDefault() ?? "serial:///dev/ttyUSB0:115200",
        ArduPilotMegaDialect.Default));

using var _ = client.Retry().GroupBy(t => t.MessageName)
    .Select(t => t.TimeInterval())
    .SelectMany(t => t)
    .Buffer(TimeSpan.FromSeconds(3))
    .Subscribe((t) =>
    {
        Console.Clear();
        Console.WriteLine(
            $"{"MessageName",-35}{"Version",-10}{"MsgId",-10}{"Count(3s)",-10}{"Frequency",-20}");
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
                $"{i.Value.MessageName,-35}{(i.Value.IsMavlinkV2 ? "V2" : "V1"),-10}{i.Value.MessageId,-10}{i.Count,-10}{(1000 / i.Avg).ToString("F") + " Hz",-20}");
        }

        Console.WriteLine();
    });


await Task.Run(async () =>
{
    byte seq = 0;
    while (true)
    {
        await client.Send(new HeartbeatPocket(true, seq++, 2, 2, new HeartbeatPayload(){ Autopilot = MavAutopilot.MAV_AUTOPILOT_GENERIC, BaseMode=0, CustomMode=0, MavlinkVersion=2, SystemStatus= MavState.MAV_STATE_ACTIVE, Type = MavType.MAV_TYPE_FIXED_WING }));
        await Task.Delay(1000);
    }
});

Console.ReadKey();