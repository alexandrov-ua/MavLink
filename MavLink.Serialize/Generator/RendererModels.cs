namespace MavLink.Serialize.Generator;

public record RootRenderModel(
    string Namespace,
    string ClassName,
    List<MessageRenderModel> Messages,
    List<EnumRenderModel> Enums)
{
    public static RootRenderModel CreateFromDefinition(RootDefinition rootDefinition, ClassNodeInfo classNodeInfo)
    {
        return new RootRenderModel(
            classNodeInfo.NameSpace,
            classNodeInfo.DisplayName,
            rootDefinition.Messages.Select(t=>MessageRenderModel.CreateFromDefinition(t)).ToList(),
            rootDefinition.Enums.Select(t=>EnumRenderModel.CreateFromDefinition(t)).ToList()
        );
    }
}

public record MessageRenderModel(string Name, int Id, string Description, List<MessageItemRenderModel> Items)
{
    public int Size { get; private set; }

    public byte ChecksumExtra { get; private set; }

    public static MessageRenderModel CreateFromDefinition(MessageDefinition messageDefinition)
    {
        var itemsModel = messageDefinition.Items.Select(t => MessageItemRenderModel.CreateFromDefinition(t));
        
        //TODO: sorting
        var message = new MessageRenderModel(messageDefinition.Name, messageDefinition.Id,
            messageDefinition.Description, itemsModel.ToList());
        message.ChecksumExtra = message.CalculateChecksumExtra();
        message.Size = message.Items.Select(t => t.Type.ActualSize).Sum();
        return message;
    }

    private byte CalculateChecksumExtra()
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

public record MessageItemRenderModel(string Name, TypeRenderModel Type)
{
    public static MessageItemRenderModel CreateFromDefinition(MessageItemDefinition messageItemDefinition)
    {
        return new MessageItemRenderModel(messageItemDefinition.Name, TypeRenderModel.CreateFromType(messageItemDefinition.Type));
    }
}

public record TypeRenderModel(string OriginalType, string CsType, int Size, byte? ArrayLength, string? Enum)
{
    public bool IsEnum => Enum != null;
    public bool IsArray => ArrayLength != null;
    
    public int ActualSize => IsArray ? Size * ArrayLength!.Value : Size;

    public static TypeRenderModel CreateFromType(string type)
    {
        throw new NotImplementedException();
    }
}

public record class EnumRenderModel(string Name, string Description, bool IsBitmask, List<EnumItemRenderModel> Items)
{
    public static EnumRenderModel CreateFromDefinition(EnumDefinition enumDefinition)
    {
        throw new NotImplementedException();
    }
}

public record class EnumItemRenderModel(int Index, string Name, string Description)
{
}