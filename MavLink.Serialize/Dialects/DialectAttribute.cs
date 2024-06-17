using System;
namespace MavLink.Serialize.Dialects;
[AttributeUsage(AttributeTargets.Class)]
public class DialectAttribute : Attribute
{
    public DialectAttribute(string filePath)
    {
        FilePath = filePath;
    }
    public string FilePath { get; }
}