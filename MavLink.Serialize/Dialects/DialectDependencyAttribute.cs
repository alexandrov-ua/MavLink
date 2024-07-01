namespace MavLink.Serialize.Dialects;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DialectDependencyAttribute<T> : Attribute where T : IDialect
{
    
}