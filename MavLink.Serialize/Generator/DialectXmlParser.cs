using System.Xml.Linq;

namespace MavLink.Serialize.Generator;

public static class DialectXmlParser
{
    public static RootDefinition Parse(string fileName)
    {
        var document = XDocument.Load(fileName);
        
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
                            Index: uint.Parse(q.Attribute("value")?.Value ?? "0")
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
                        .Select((val, i) => new { Value = val, Index = i - 1 })
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

public record RootDefinition(string? Version, List<EnumDefinition> Enums, List<MessageDefinition> Messages, List<string> Includes)
{
}

public record class EnumDefinition(string Name, string Description, bool IsBitmask, List<EnumItemDefinition> Items)
{
}

public record class EnumItemDefinition(uint Index, string Name, string Description)
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