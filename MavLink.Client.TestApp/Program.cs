// See https://aka.ms/new-console-template for more information

using System.Text.Json;
using System.Text.Json.Serialization;
using MavLink.Client;
using MavLink.Client.TestApp.Dialects;

var client = MavLinkClient.Create("udp://0.0.0.0:14550", ArduPilotMegaDialect.Default);

while (true)
{
    var pocket = client.Receive();
    Console.WriteLine(pocket.ToString());
    Console.WriteLine(JsonSerializer.Serialize(pocket.Payload, pocket.Payload.GetType(), new JsonSerializerOptions()
    {
        NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals
    }));
}