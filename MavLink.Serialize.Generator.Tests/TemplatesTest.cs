using System.Text;
using FluentAssertions;
using MavLink.Serialize.Messages;
using Scriban;
using Scriban.Runtime;
using Xunit.Abstractions;

namespace MavLink.Serialize.Generator.Tests;

public class TemplatesTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TemplatesTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Smoke()
    {
        var model = new RootRenderModel(
            "My.Test",
            "MyDialect",
            new List<MessageRenderModel>()
            {
                new MessageRenderModel(
                    "HEARTBEAT", 
                    0, 
                    "The heartbeat message shows that a system or component is present and responding. The type and autopilot fields (along with the message component id),",
                    new List<MessageItemRenderModel>()
                    {
                        new MessageItemRenderModel("type", new TypeRenderModel("uint8_t", "byte", 1,null, null)),
                        new MessageItemRenderModel("custom_mode", new TypeRenderModel("uint32_t", "uint", 4,null, null)),
                    }, null),
                new MessageRenderModel(
                    "PROTOCOL_VERSION", 
                    300, 
                    "The heartbeat message shows that a system or component is present and responding. The type and autopilot fields (along with the message component id),",
                    new List<MessageItemRenderModel>()
                    {
                        new MessageItemRenderModel("version", new TypeRenderModel("uint16_t", "ushort", 2,10, null)),
                        new MessageItemRenderModel("custom_mode", new TypeRenderModel("uint32_t", "uint", 4,null, null)),
                    }, null),
                new MessageRenderModel(
                    "PROTOCOL_VERSION1", 
                    301, 
                    "The heartbeat message shows that a system or component is present and responding. The type and autopilot fields (along with the message component id),",
                    new List<MessageItemRenderModel>()
                    {
                        new MessageItemRenderModel("version", new TypeRenderModel("uint16_t", "ushort", 2,null, "MAV_AUTOPILOT")),
                        new MessageItemRenderModel("custom_mode", new TypeRenderModel("uint32_t", "uint", 4,null, null)),
                    }, null),
            },
            new List<EnumRenderModel>()
            {
                new EnumRenderModel("MAV_AUTOPILOT", "Micro air vehicle / autopilot classes", false,
                    new List<EnumItemRenderModel>()
                    {
                        new EnumItemRenderModel(0, "MAV_AUTOPILOT_GENERIC",
                            "Generic autopilot, full support for everything"),
                        new EnumItemRenderModel(1, "MAV_AUTOPILOT_RESERVED", "Reserved for future use"),
                    }),
                new EnumRenderModel("MAV_MODE_FLAG", "These flags encode the MAV mode", true,
                    new List<EnumItemRenderModel>()
                    {
                        new EnumItemRenderModel(1, "MAV_MODE_FLAG_CUSTOM_MODE_ENABLED",
                            "0b00000001 Reserved for future use"),
                        new EnumItemRenderModel(2, "MAV_MODE_FLAG_TEST_ENABLED", "Reserved for future use"),
                        new EnumItemRenderModel(4, "MAV_MODE_FLAG_AUTO_ENABLED", "Reserved for future use"),
                    }),
            },
            new List<string>());

        var text = TemplateHelper.RenderTemplate(model);

        
        _testOutputHelper.WriteLine(text);
    }
    
    [Fact]
    public void Smoke2()
    {
        var model = new RootRenderModel(
            "My.Test",
            "MyDialect",
            new List<MessageRenderModel>(),
            new List<EnumRenderModel>(),
            new List<string>()
            {
                "standard.xml",
                "minimal.xml"
            });

        var text = TemplateHelper.RenderTemplate(model);

        
        _testOutputHelper.WriteLine(text);
    }

    public static string GetResource(string name)
    {
        using (Stream stream = typeof(IPayload).Assembly.GetManifestResourceStream(name))
        using (StreamReader reader = new StreamReader(stream))
        {
            return reader.ReadToEnd();
        }
    }
}
