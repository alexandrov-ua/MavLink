using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Xml.Linq;
using FluentAssertions;
using Xunit.Abstractions;

namespace MavLink.Serialize.Generator.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _testOutputHelper;

    public UnitTest1(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }
    
    [Fact]
    public void Foo()
    {
        var document = XDocument.Load("minimal.xml");

        var mavlink = document.Element("mavlink");
        mavlink.Element("version").Value.Should().Be("3");
        var enumDefinitions =
            mavlink.Element("enums")
                .Elements("enum")
                .Select(t => new EnumDefinition(
                    Name: t.Attribute("name")?.Value,
                    Description: t.Element("description")?.Value,
                    IsBitmask: t.Attribute("bitmask")?.Value == "true",
                    Items: t.Elements("entry")
                        .Select(q=>new EnumDefinitionItem(
                            Name: q.Attribute("name")?.Value,
                            Description: q.Element("description")?.Value,
                            Index: int.Parse(q.Attribute("value")?.Value ?? "0") 
                        )).ToList()
                )).ToList();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(enumDefinitions));
        

    }

    [Fact]
    public void Bar()
    {
        var document = XDocument.Load("minimal.xml");

        var mavlink = document.Element("mavlink");
        mavlink.Element("version").Value.Should().Be("3");
        var messagesDefinitions =
            mavlink.Element("messages")
                .Elements("message")
                .Select(t => new MessageDefinition(
                    Name: t.Attribute("name")?.Value,
                    Id: int.Parse(t.Attribute("id")?.Value ?? "0"),
                    Description: t.Element("description")?.Value,
                    ExtensionIndex: 0,
                    Items: t.Elements("field")
                        .Select(q=>new MessageDefinitionItem(
                            Name: q.Attribute("name")?.Value,
                            Type: q.Attribute("type")?.Value,
                            Enum: q.Attribute("enum")?.Value,
                            Display: q.Attribute("display")?.Value
                        )).ToList()
                )).ToList();
        _testOutputHelper.WriteLine(JsonSerializer.Serialize(messagesDefinitions));
        
    }

    public record class EnumDefinition(string Name, string Description, bool IsBitmask, List<EnumDefinitionItem> Items)
    {
    }

    public record class EnumDefinitionItem(int Index, string Name, string Description)
    {

    }

    public record MessageDefinition(string Name, int Id, string Description, List<MessageDefinitionItem> Items, int ExtensionIndex)
    {
        
    }

    public record MessageDefinitionItem(string Type, string Name, string Enum, string Display)
    {
    }
}