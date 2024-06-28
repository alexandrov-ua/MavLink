using System;
namespace MavLink.Serialize.Dialects;
[AttributeUsage(AttributeTargets.Class)]
public class DialectAttribute : Attribute
{
    /// <summary>
    /// Indicates class to generate IDialect realization
    /// </summary>
    /// <param name="fileName">File name added as AdditionalFiles</param>
    public DialectAttribute(string fileName)
    {
        FileName = fileName;
    }
    public string FileName { get; }
}