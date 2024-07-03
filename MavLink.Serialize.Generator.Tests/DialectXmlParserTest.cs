using System.Text.Json;
using System.Xml.Linq;
using FluentAssertions;
using Xunit.Abstractions;

namespace MavLink.Serialize.Generator.Tests;

public class DialectXmlParserTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public DialectXmlParserTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Foo()
    {
        var document = XDocument.Load("DialectXmls/common.xml");

        var mavlink = document.Element("mavlink");
        mavlink.Should().NotBeNull();
        mavlink!.Element("version").Should().NotBeNull();
        var values =
            mavlink.Element("messages")!
                .Elements("message")
                .Select(t => int.Parse(t.Attribute("id")?.Value!)).ToList();
        _testOutputHelper.WriteLine(string.Join(" ", values.Select(t => t.ToString())));
    }

    [Fact]
    public void Bar()
    {
        var document = XDocument.Load("DialectXmls/common.xml");

        var mavlink = document.Element("mavlink");
        var values =
            mavlink.Element("enums")!
                .Elements("enum")
                .SelectMany(t => t.Elements("entry")).Select(q => uint.Parse(q.Attribute("value")?.Value ?? "0"))
                .ToList();
        _testOutputHelper.WriteLine(string.Join(" ", values.Select(t => t.ToString())));
    }

    [Fact]
    public void Quxx()
    {
        var document = XDocument.Load("DialectXmls/common.xml");

        var mavlink = document.Element("mavlink");
        mavlink.Should().NotBeNull();
        mavlink!.Element("version").Should().NotBeNull();
        mavlink!.Element("version")!.Value.Should().Be("3");
        var types =
            mavlink.Element("messages")!
                .Elements("message")
                .SelectMany(t => t.Elements("field"))
                .Select(t => t.Attribute("type")?.Value)
                .Select(t => t.Split("[").First())
                .Distinct().Order().ToList();
        _testOutputHelper.WriteLine(string.Join("\n", types));
    }

    [Fact]
    public void FooBar()
    {
        var document = XDocument.Load("DialectXmls/common.xml");

        var mavlink = document.Element("mavlink");
        var enumDef =
            mavlink!.Element("enums")?
                .Elements("enum")
                .Select(t => new EnumDefinition(
                    Name: t.Attribute("name")?.Value ?? "",
                    Description: t.Element("description")?.Value ?? "",
                    IsBitmask: t.Attribute("bitmask")?.Value == "true",
                    Items: t.Elements("entry")
                        .Select(q => new EnumItemDefinition(
                            Name: q.Attribute("name")?.Value ?? "",
                            Description: q.Element("description")?.Value ?? "",
                            Index: uint.Parse(q.Attribute("value")?.Value ?? "0"),
                            Params: q.Elements("param")
                                .Select(x => new EnumParams(
                                    Index: int.Parse(x.Attribute("index")?.Value ?? "0"),
                                    Value: x.Value,
                                    Label: x.Attribute("label")?.Value ?? "",
                                    Units: x.Attribute("units")?.Value ?? "",
                                    Enum: x.Attribute("enum")?.Value ?? "",
                                    Reserved: bool.Parse(x.Attribute("reserved")?.Value ?? "false")
                                )).ToList()
                        )).ToList()
                )).ToList().Where(t=>t.Items.First().Params.Count > 0).SelectMany(t=>t.Items).SelectMany(t=>t.Params)
                .OrderBy(t=>t.Value);

        foreach (var e in enumDef)
        {
            _testOutputHelper.WriteLine(":"+e.Label+"----------"+e.Value);
        }
    }
}