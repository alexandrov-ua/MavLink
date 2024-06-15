using BenchmarkDotNet.Attributes;
using MavLink.Serialize.Dialects.Test;
using MavLink.Serialize.Messages;
using MavLink.Serialize.Tests.Mavgen;

namespace MavLink.Serialize.Benchmark;

[MemoryDiagnoser]
public class DeserializeBenchmark
{
    public byte[] Data = new byte[]
    {
        0xfd, 0x1f, 0x00, 0x00, 0x38, 0x01, 0x01, 0x01, 0x00, 0x00, 0x2f, 0xfc, 0x71, 0x53, 0x2f, 0xfc, 0x61, 0x53,
        0x2f, 0xfc, 0x71, 0x57, 0x00, 0x00, 0x38, 0x31, 0xfa, 0x0a, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
        0x00, 0x00, 0x00, 0x00, 0x5b, 0xee, 0x50
    };

    private MAVLink.MavlinkParse _parser = new MAVLink.MavlinkParse(false);

    [Benchmark]
    public IPocket<IPayload> Deserialize()
    {
        return MavlinkSerialize.Deserialize(Data, TestDialect.Default);
    }

    [Benchmark(Baseline = true)]
    public MAVLink.MAVLinkMessage DeserializeMavgen()
    {
        return _parser.ReadPacket(new MemoryStream(Data));
    }
}