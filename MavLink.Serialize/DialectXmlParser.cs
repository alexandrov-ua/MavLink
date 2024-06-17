using System.Xml.Linq;
using Scriban.Parsing;

namespace MavLink.Serialize;

public static class DialectXmlParser
{
    public static RootDefinition Parse(string fileName)
    {
        var document = XDocument.Load(fileName);
        
        var mavlink = document.Element("mavlink");
        var version = mavlink!.Element("version")?.Value;
        
        var enumDefinitions =
            mavlink.Element("enums")!
                .Elements("enum")
                .Select(t => new EnumDefinition(
                    Name: t.Attribute("name")?.Value ?? "",
                    Description: t.Element("description")?.Value ?? "",
                    IsBitmask: t.Attribute("bitmask")?.Value == "true",
                    Items: t.Elements("entry")
                        .Select(q => new EnumItemDefinition(
                            Name: q.Attribute("name")?.Value ?? "",
                            Description: q.Element("description")?.Value ?? "",
                            Index: int.Parse(q.Attribute("value")?.Value ?? "0")
                        )).ToList()
                )).ToList();
        var messagesDefinitions =
            mavlink.Element("messages")!
                .Elements("message")
                .Select(t => new MessageDefinition(
                    Name: t.Attribute("name")?.Value ?? "",
                    Id: int.Parse(t.Attribute("id")?.Value ?? "0"),
                    Description: t.Element("description")?.Value ?? "",
                    ExtensionIndex: t.Descendants()
                        .Select((val, i) => (val, i))
                        .Where(tup => tup.val.Name.ToString() == "extensions")
                        .Select(tup => tup.i - 1)
                        .FirstOrDefault(),
                    Items: t.Elements("field")
                        .Select(q => new MessageItemDefinition(
                            Name: q.Attribute("name")?.Value ?? "",
                            Type: q.Attribute("type")?.Value ?? "",
                            Enum: q.Attribute("enum")?.Value ?? "",
                            Display: q.Attribute("display")?.Value ?? ""
                        )).ToList()
                )).ToList();
        
        return new RootDefinition(version, enumDefinitions, messagesDefinitions);
    }
}

public record RootDefinition(string? Version, List<EnumDefinition> Enums, List<MessageDefinition> Messages)
{
}

public record class EnumDefinition(string Name, string Description, bool IsBitmask, List<EnumItemDefinition> Items)
{
}

public record class EnumItemDefinition(int Index, string Name, string Description)
{
}

public record MessageDefinition(
    string Name,
    int Id,
    string Description,
    List<MessageItemDefinition> Items,
    int? ExtensionIndex)
{
}

public record MessageItemDefinition(string Type, string Name, string Enum, string Display)
{
}