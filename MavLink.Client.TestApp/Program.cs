// See https://aka.ms/new-console-template for more information

using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using MavLink.Client;
using MavLink.Client.TestApp.Dialects;
using MavLink.Serialize.Messages;

var queue = new ConcurrentQueue<IPocket<IPayload>>();
var cts = new CancellationTokenSource();
Console.CancelKeyPress += new ConsoleCancelEventHandler((_, e) =>
{
    //cts.Cancel();
    
    queue.Enqueue(new CommandLongPocket(true, 0, 255, 190, new CommandLongPayload()
    {
        Command = CommonDialect.MavCmd.MAV_CMD_REQUEST_MESSAGE,
        TargetComponent = 1,
        TargetSystem = 1,
        Param1 = 148
    }));
    
    e.Cancel = true;
});

//Console.

void Foo()
{
    var e = new UdpClient();
    //e.Receive(ref new IPEndPoint((long)1234567,123));
}




using var client = MavLinkClient.Create("udp://127.0.0.1:14550", ArduPilotMegaDialect.Default);

var cancellationToken = cts.Token;



while (!cancellationToken.IsCancellationRequested)
{
    var pocket = client.Receive();
    Console.WriteLine(pocket.ToString());
    Console.WriteLine(JsonSerializer.Serialize(pocket.Payload, pocket.Payload.GetType(), new JsonSerializerOptions()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    }));

    if (queue.TryDequeue(out var pocketToSend))
    {
        pocketToSend.SequenceNumber = (byte)(pocket.SequenceNumber + 1);
        await client.Send(pocketToSend);   
        Console.WriteLine("Sent: "+pocketToSend.ToString());
    }
}