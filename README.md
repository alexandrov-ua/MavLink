## Description

This repo contains:
 - Implementation of MavLink client for communication with MavLink vehicles and GCSs.
 - Its own serializer/deserializer based on Spans that provides good performance and optimal memory allocation.
 - Its own model generator based on DotNet Source Generators that gives convenience and flexibility
 - Demo App (desktop app that show your vehicle on the map)

## Sope of work

**This project currently in progress.**

 - Serialization/deserialization
   - MavLink V2 - `DONE`
   - MavLink V1 backwark compatibility - `DONE`
   - Signing - `TO DO`
 - Model generator
   - Enums - `DONE`
   - Messages - `DONE`
     - Units - `TO DO`
   - Commands - `TO DO`
   - Dependencies between dialects - `IN PROGRESS`
 - Client - `IN PROGRESS`
   - ReactiveClient - `IN PROGRESS`
   - MavLink's microservices - `TO DO`
   - Vehicle abstraction - `TO DO`
 - Publish as NuGet - `TO DO`

## Documentation

**Client creation:**

```CSharp
using var client = new MavLinkReactiveClient(
    MavLinkClient.Create("udp://0.0.0.0:14550", ArduPilotMegaDialect.Default)
);
```

**Consuming messages:**

```CSharp
using var _ = client
    .OfType<GlobalPositionIntPocket>()
    .Subscribe(p =>
    {
        Console.WriteLine($"{p.MessageName}, {p.MessageId}");
        Console.WriteLine($"{p.Payload.Alt}");
    });
```

**Messages generation (Dialect generation):**

For example see `MavLink.Client.TestApp project`

1. Add references to MavLink.Serialize and MavLink.Serialize.Generator in your `.csproj`
```XML
        <ProjectReference Include="..\MavLink.Serialize\MavLink.Serialize.csproj" />
        <ProjectReference Include="..\MavLink.Serialize.Generator\MavLink.Serialize.Generator.csproj" OutputItemType="Analyzer" ReferenceOutputAssembly="true"/>
```
NOTE: `OutputItemType="Analyzer" ReferenceOutputAssembly="true"` is important

2. Add MavLink schema XML file in your project:
```XML
    <ItemGroup>
        <None Remove="Dialects\minimal.xml"/>
        <AdditionalFiles Include="Dialects\minimal.xml"/>
    </ItemGroup>
```
3. Create dialect class in your project
```CSharp
using MavLink.Serialize.Dialects;
namespace MavLink.Client.TestApp.Dialects;

[Dialect("minimal.xml")]
public partial class MyMinimalDialect
{
}
```
NOTE: all models will be generated in the same namespace with MyMinimalDialect class
4. Use this dialest with the client:
```CSharp
using var client = MavLinkClient.Create("udp://0.0.0.0:14550", MyMinimalDialect.Default);
```



