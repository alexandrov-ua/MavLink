namespace MavLink.Serialize.Generator;

public record RootRenderModel(
    string Namespace,
    string ClassName,
    List<MessageRenderModel> Messages,
    List<EnumRenderModel> Enums,
    List<string> Includes)
{
    public bool AnyIncludes => Includes.Any();
    public string IncludesParameters => string.Join(", ", Includes.Select(t => $"IDialect {t.Replace(".","")}"));

    public bool AnyHiddenEnums => HiddenEnums.Any();

    public IEnumerable<EnumRenderModel> GlobalEnums => Enums.Where(t => t.Name != "MAV_CMD");
    public IEnumerable<EnumRenderModel> HiddenEnums => Enums.Where(t => t.Name == "MAV_CMD");
    
    
    public static RootRenderModel CreateFromDefinition(RootDefinition rootDefinition, ClassNodeInfo classNodeInfo)
    {
        return new RootRenderModel(
            classNodeInfo.NameSpace,
            classNodeInfo.DisplayName,
            rootDefinition.Messages.Select(t => MessageRenderModel.CreateFromDefinition(t)).ToList(),
            rootDefinition.Enums.Select(t => EnumRenderModel.CreateFromDefinition(t)).ToList(),
            rootDefinition.Includes
        );
    }
}

public record MessageRenderModel(string Name, int Id, string Description, List<MessageItemRenderModel> Items, int? ExtensionIndex)
{
    public int Size => Items.Select(t => t.Type.ActualSize).Sum();

    public byte ChecksumExtra
    {
        get
        {
            var acc = ChecksumHelper.Accumulate(Name + " ", 0xFFFF);
            foreach (var i in Items.Take(ExtensionIndex ?? Items.Count))
            {
                acc = ChecksumHelper.Accumulate($"{i.Type.OriginalType} {i.Name} ", acc);
                if (i.Type.IsArray)
                    acc = ChecksumHelper.Accumulate(i.Type.ArrayLength!.Value, acc);
            }

            return (byte) (((byte) acc & (byte) 0xFF) ^ (byte) (acc >> 8));
        }
    }

    public static MessageRenderModel CreateFromDefinition(MessageDefinition messageDefinition)
    {
        var itemsModel = messageDefinition.Items.Select(t => MessageItemRenderModel.CreateFromDefinition(t));

        if (messageDefinition.ExtensionIndex.HasValue)
        {
            itemsModel = itemsModel.Take(messageDefinition.ExtensionIndex.Value)
                .Select((val, i) => (val, i))
                .OrderByDescending(t => t.val.Type.Size).ThenBy(t => t.i)
                .Select(t => t.val)
                .Concat<MessageItemRenderModel>(itemsModel.Skip(messageDefinition.ExtensionIndex.Value));
        }
        else
        {
            itemsModel = itemsModel.Select((val, i) => (val, i))
                .OrderByDescending(t => t.val.Type.Size).ThenBy(t => t.i)
                .Select(t => t.val);
        }

        var orderedItems = itemsModel.ToList();
        var message = new MessageRenderModel(messageDefinition.Name, messageDefinition.Id,
            messageDefinition.Description ?? "", orderedItems, messageDefinition.ExtensionIndex);
        return message;
    }
}

public record MessageItemRenderModel(string Name, TypeRenderModel Type, string Description)
{
    public static MessageItemRenderModel CreateFromDefinition(MessageItemDefinition messageItemDefinition)
    {
        return new MessageItemRenderModel(messageItemDefinition.Name,
            TypeRenderModel.CreateFromType(messageItemDefinition.Type, messageItemDefinition.Enum), 
            messageItemDefinition.Description);
    }
}

public record TypeRenderModel(string OriginalType, string CsType, int Size, byte? ArrayLength, string? Enum)
{
    public bool IsEnum => Enum != null;
    public bool IsArray => ArrayLength != null;

    public bool IsHidden => Enum == "MAV_CMD";

    public int ActualSize => IsArray ? Size * ArrayLength!.Value : Size;

    public static TypeRenderModel CreateFromType(string type, string? @enum)
    {
        type = type.Replace("_mavlink_version", "");
        var arraySize = GetArraySize(type);
        type = type.Split("[").First();
        return GetCsType(type, arraySize, @enum);
    }

    private static byte? GetArraySize(string str)
    {
        var t = str.Split('[').Skip(1).LastOrDefault();
        if (t != null)
        {
            return (byte) int.Parse(t.Replace("]", ""));
        }

        return null;
    }

    private static TypeRenderModel GetCsType(string type, byte? arrayLanght, string @enum)
    {
        return type switch
        {
            "char" => new TypeRenderModel(type, "char", 2, arrayLanght, @enum),
            "double" => new TypeRenderModel(type, "double", 4, arrayLanght, @enum),
            "float" => new TypeRenderModel(type, "float", 8, arrayLanght, @enum),
            "int8_t" => new TypeRenderModel(type, "sbyte", 1, arrayLanght, @enum),
            "int16_t" => new TypeRenderModel(type, "short", 2, arrayLanght, @enum),
            "int32_t" => new TypeRenderModel(type, "int", 4, arrayLanght, @enum),
            "int64_t" => new TypeRenderModel(type, "long", 8, arrayLanght, @enum),
            "uint8_t" => new TypeRenderModel(type, "byte", 1, arrayLanght, @enum),
            "uint16_t" => new TypeRenderModel(type, "ushort", 2, arrayLanght, @enum),
            "uint32_t" => new TypeRenderModel(type, "uint", 4, arrayLanght, @enum),
            "uint64_t" => new TypeRenderModel(type, "ulong", 8, arrayLanght, @enum),

            _ => throw new NotImplementedException()
        };
    }
}

public record class EnumRenderModel(string Name, string Description, bool IsBitmask, List<EnumItemRenderModel> Items)
{
    public static EnumRenderModel CreateFromDefinition(EnumDefinition enumDefinition)
    {
        return new EnumRenderModel(enumDefinition.Name, enumDefinition.Description, enumDefinition.IsBitmask,
            enumDefinition.Items.Select(t => new EnumItemRenderModel(t.Index, t.Name, t.Description)).ToList());
    }
}

public record class EnumItemRenderModel(uint Index, string Name, string Description)
{
}