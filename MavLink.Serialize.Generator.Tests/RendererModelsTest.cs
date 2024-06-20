using FluentAssertions;

namespace MavLink.Serialize.Generator.Tests;

public class RendererModelsTest
{
    [Fact]
    public void CheckCrc1()
    {
        new MessageRenderModel("SYSTEM_TIME", 2, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("time_unix_usec", TypeRenderModel.CreateFromType("uint64_t", null),""),
            new MessageItemRenderModel("time_boot_ms", TypeRenderModel.CreateFromType("uint32_t", null),"")
        }, null).ChecksumExtra.Should().Be(137);
    }
    
    [Fact]
    public void CheckCrc2()
    {
        new MessageRenderModel("FILE_TRANSFER_PROTOCOL", 110, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("target_network", TypeRenderModel.CreateFromType("uint8_t", null),""),
            new MessageItemRenderModel("target_system", TypeRenderModel.CreateFromType("uint8_t", null),""),
            new MessageItemRenderModel("target_component", TypeRenderModel.CreateFromType("uint8_t", null),""),
            new MessageItemRenderModel("payload", TypeRenderModel.CreateFromType("uint8_t[251]", null),"")
        }, null).ChecksumExtra.Should().Be(84);
    }
    
    [Fact]
    public void CheckCrc_WithExtensions()
    {
        new MessageRenderModel("SYS_STATUS", 1, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("onboard_control_sensors_present", TypeRenderModel.CreateFromType("uint32_t", null),""),
            new MessageItemRenderModel("onboard_control_sensors_enabled", TypeRenderModel.CreateFromType("uint32_t", null),""),
            new MessageItemRenderModel("onboard_control_sensors_health", TypeRenderModel.CreateFromType("uint32_t", null),""),
            new MessageItemRenderModel("load", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("voltage_battery", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("current_battery", TypeRenderModel.CreateFromType("int16_t", null),""),
            new MessageItemRenderModel("drop_rate_comm", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("errors_comm", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("errors_count1", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("errors_count2", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("errors_count3", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("errors_count4", TypeRenderModel.CreateFromType("uint16_t", null),""),
            new MessageItemRenderModel("battery_remaining", TypeRenderModel.CreateFromType("int8_t", null),""),
            new MessageItemRenderModel("onboard_control_sensors_present_extended", TypeRenderModel.CreateFromType("uint32_t", null),""),
            new MessageItemRenderModel("onboard_control_sensors_enabled_extended", TypeRenderModel.CreateFromType("uint32_t", null),""),
            new MessageItemRenderModel("onboard_control_sensors_health_extended", TypeRenderModel.CreateFromType("uint32_t", null),""),
        }, 13).ChecksumExtra.Should().Be(124);
    }
}