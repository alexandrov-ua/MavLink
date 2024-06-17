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
    public void Bar()
    {
        new MessageRenderModel("SYSTEM_TIME", 2, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("time_unix_usec", new TypeRenderModel("uint64_t", "", 4, null, null)),
            new MessageItemRenderModel("time_boot_ms", new TypeRenderModel("uint32_t", "", 4, null, null))
        }).ChecksumExtra.Should().Be(137);
    }
    
    [Fact]
    public void Quxx()
    {
        new MessageRenderModel("FILE_TRANSFER_PROTOCOL", 110, "", new List<MessageItemRenderModel>()
        {
            new MessageItemRenderModel("target_network", new TypeRenderModel("uint8_t", "", 1, null, null)),
            new MessageItemRenderModel("target_system", new TypeRenderModel("uint8_t", "", 1, null, null)),
            new MessageItemRenderModel("target_component", new TypeRenderModel("uint8_t", "", 1, null, null)),
            new MessageItemRenderModel("payload", new TypeRenderModel("uint8_t", "", 1, 251, null))
        }).ChecksumExtra.Should().Be(84);
    }

    [Fact]
    public void Foo()
    {
        var result = GetResource("MavLink.Serialize.Generator.Templates.RootTemplate.scriban");
        var template = Template.Parse(result);


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
                    }),
                new MessageRenderModel(
                    "PROTOCOL_VERSION", 
                    300, 
                    "The heartbeat message shows that a system or component is present and responding. The type and autopilot fields (along with the message component id),",
                    new List<MessageItemRenderModel>()
                    {
                        new MessageItemRenderModel("version", new TypeRenderModel("uint16_t", "ushort", 2,10, null)),
                        new MessageItemRenderModel("custom_mode", new TypeRenderModel("uint32_t", "uint", 4,null, null)),
                    }),
                new MessageRenderModel(
                    "PROTOCOL_VERSION1", 
                    301, 
                    "The heartbeat message shows that a system or component is present and responding. The type and autopilot fields (along with the message component id),",
                    new List<MessageItemRenderModel>()
                    {
                        new MessageItemRenderModel("version", new TypeRenderModel("uint16_t", "ushort", 2,null, "MAV_AUTOPILOT")),
                        new MessageItemRenderModel("custom_mode", new TypeRenderModel("uint32_t", "uint", 4,null, null)),
                    }),
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
            });


        var context = new TemplateContext {MemberRenamer = member => member.Name};

        var scriptObject = new ScribanHelper();
        context.PushGlobal(scriptObject);

        var entityScriptObject = new ScriptObject();
        entityScriptObject.Import(model, renamer: member => member.Name);
        context.PushGlobal(entityScriptObject);

        var rendered = template.Render(context);
        //var rendered = template.Render(model, member => member.Name);
        _testOutputHelper.WriteLine(rendered);
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

public record RootRenderModel(
    string Namespace,
    string ClassName,
    List<MessageRenderModel> Messages,
    List<EnumRenderModel> Enums)
{
}

public record MessageRenderModel(string Name, int Id, string Description, List<MessageItemRenderModel> Items)
{
    public int Size => Items.Select(t => t.Type.ActualSize).Sum();

    public byte ChecksumExtra
    {
        get
        {

            var acc = ChecksumHelper.Accumulate(Name + " ", 0xFFFF);
            foreach (var i in Items)
            {
                acc = ChecksumHelper.Accumulate($"{i.Type.OriginalType} {i.Name} ", acc);
                if (i.Type.IsArray)
                    acc = ChecksumHelper.Accumulate(i.Type.ArrayLength!.Value, acc);
            }
            
            return (byte) (((byte)acc & (byte)0xFF) ^ (byte) (acc >> 8));
        }
    }
}

public record MessageItemRenderModel(string Name, TypeRenderModel Type)
{
    
}

public record TypeRenderModel(string OriginalType, string CsType, int Size, byte? ArrayLength, string? Enum)
{
    public bool IsEnum => Enum != null;
    public bool IsArray => ArrayLength != null;
    
    public int ActualSize => IsArray ? Size * ArrayLength!.Value : Size;
}

public record class EnumRenderModel(string Name, string Description, bool IsBitmask, List<EnumItemRenderModel> Items)
{
}

public record class EnumItemRenderModel(int Index, string Name, string Description)
{
}

public class ScribanHelper : ScriptObject
{
    public static string ToPascalCase(string word)
    {
        if (string.IsNullOrEmpty(word))
            return string.Empty;

        return string.Join("", word.Split('_')
            .Select(w => w.Trim())
            .Where(w => w.Length > 0)
            .Select(w => w.Substring(0, 1).ToUpper() + w.Substring(1).ToLower()));
    }
}