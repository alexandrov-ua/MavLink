using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MavLink.Serialize.Generator;

[Generator]
public class DialectGenerator : ISourceGenerator
{
    public const string Attribute = """
                                    using System;
                                    namespace MavLink.DialectGenerator;
                                    [AttributeUsage(AttributeTargets.Class)]
                                    public class DialectAttribute : Attribute
                                    {
                                        public DialectAttribute(string filePath)
                                        {
                                            FilePath = filePath;
                                        }
                                        public string FilePath { get; }
                                    }
                                    """;
    
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        //context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver());
        // context.RegisterForPostInitialization(ctx => ctx.AddSource(
        //     "GenerateServiceAttribute.g.cs",
        //     SourceText.From(Attribute, Encoding.UTF8)));
    }
    

    
    public static ClassDeclarationSyntax GetTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;
        return classDeclarationSyntax;
    }

    public void Execute(GeneratorExecutionContext context)
    {
        //var mainMethod = context.Compilation.GetEntryPoint(context.CancellationToken);

        if (context.SyntaxContextReceiver is SyntaxReceiver receiver)
        {
            string source = $@"// <auto-generated/>
using System;
using MavLink.Serialize.Messages;
using MavLink.Serialize.Dialects;

namespace {receiver.Roots.First().NameSpace}
{{
    public partial class MinimalDialect : IDialect
    {{
        public IPocket<IPayload> CreatePocket(uint messageId, bool isMavlinkV2, byte sequenceNumber, byte systemId, byte componentId, ReadOnlySpan<byte> payload)
        {{
            //{receiver.Roots.First().FilePath}
            //{receiver.Roots.First().DisplayName}
            //{receiver.Roots.First().NameSpace}
            throw new NotImplementedException();
        }}

        public static IDialect Default = new MinimalDialect();
    }}
}}
";
            //var typeName = mainMethod.ContainingType.Name;

            // Add the source code to the compilation
            context.AddSource($"MinimalDialect.g.cs", source);
            
        }
    }
}