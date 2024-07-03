using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MavLink.Serialize.Generator;

public static class DialectXmlParser
{
    public static RootDefinition Parse(Stream stream)
    {
        var document = XDocument.Load(stream);

        var mavlink = document.Element("mavlink");
        var version = mavlink?.Element("version")?.Value ?? "";

        var enumDefinitions =
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
                )).ToList() ?? new List<EnumDefinition>();
        var messagesDefinitions =
            mavlink.Element("messages")?
                .Elements("message")
                .Select(t => new MessageDefinition(
                    Name: t.Attribute("name")?.Value!,
                    Id: int.Parse(t.Attribute("id")?.Value!),
                    Description: t.Element("description")?.Value,
                    ExtensionIndex: t.Descendants()
                        .Select((val, i) => new {Value = val, Index = i - 1})
                        .FirstOrDefault(o => o.Value.Name.ToString() == "extensions")?.Index,
                    Items: t.Elements("field")
                        .Select(q => new MessageItemDefinition(
                            Name: q.Attribute("name")?.Value!,
                            Type: q.Attribute("type")?.Value!,
                            Enum: q.Attribute("enum")?.Value,
                            Display: q.Attribute("display")?.Value,
                            Description: q.Value
                        )).ToList()
                )).ToList() ?? new List<MessageDefinition>();

        var includes = mavlink.Elements("include")
            .Select(t => t.Value).ToList();

        return new RootDefinition(version, enumDefinitions, messagesDefinitions, includes);
    }
}

public record RootDefinition(
    string? Version,
    List<EnumDefinition> Enums,
    List<MessageDefinition> Messages,
    List<string> Includes)
{
}

public record class EnumDefinition(string Name, string Description, bool IsBitmask, List<EnumItemDefinition> Items)
{
}

public record class EnumItemDefinition(uint Index, string Name, string Description, List<EnumParams>? Params = null)
{
}

public record EnumParams(int Index, string Value, string Label, string Units, string Enum, bool Reserved)
{
}

public record MessageDefinition(
    string Name,
    int Id,
    string? Description,
    List<MessageItemDefinition> Items,
    int? ExtensionIndex)
{
}

public record MessageItemDefinition(string Type, string Name, string Description, string? Enum, string? Display)
{
}