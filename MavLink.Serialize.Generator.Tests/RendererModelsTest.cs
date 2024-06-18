using FluentAssertions;

namespace MavLink.Serialize.Generator.Tests;

public class RendererModelsTest
{
    [Fact]
    public void CheckCrc1()
    {
        new MessageRenderModel("SYSTEM_TIME", 2, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("time_unix_usec", new TypeRenderModel("uint64_t", "", 4, null, null)),
            new MessageItemRenderModel("time_boot_ms", new TypeRenderModel("uint32_t", "", 4, null, null))
        }).ChecksumExtra.Should().Be(137);
    }
    
    [Fact]
    public void CheckCrc2()
    {
        new MessageRenderModel("FILE_TRANSFER_PROTOCOL", 110, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("target_network", new TypeRenderModel("uint8_t", "", 1, null, null)),
            new MessageItemRenderModel("target_system", new TypeRenderModel("uint8_t", "", 1, null, null)),
            new MessageItemRenderModel("target_component", new TypeRenderModel("uint8_t", "", 1, null, null)),
            new MessageItemRenderModel("payload", new TypeRenderModel("uint8_t", "", 1, 251, null))
        }).ChecksumExtra.Should().Be(84);
    }
}