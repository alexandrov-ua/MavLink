using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Scriban.Runtime;

[assembly:InternalsVisibleTo("MavLink.Serialize.Generator.Tests")]
namespace MavLink.Serialize.Generator;

[Generator]
public class DialectGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    }

    //TODO: add validation and emmit errors
    //TODO: add includes support
    public void Execute(GeneratorExecutionContext context)
    {
        if (context.SyntaxContextReceiver is SyntaxReceiver receiver)
        {
            foreach (var root in receiver.Roots)
            {
                try
                {
                    var finalPath = Path.Combine(Path.GetDirectoryName(root.SourceFilePath) ?? "", root.FilePath);
                    if (!Path.Exists(finalPath))
                    {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor("mavlink02", "File doesn't exists", "File {0} doesn't exists", "category", DiagnosticSeverity.Error, true),
                            root.Symbol.Locations.First(), finalPath
                        ));
                        continue;
                    }



                    var definition = DialectXmlParser.Parse(finalPath);

                    var model = RootRenderModel.CreateFromDefinition(definition, root);

                    var source = TemplateHelper.RenderTemplate(model);
                    context.AddSource($"{root.DisplayName}.g.cs", source);
                }
                catch (Exception e)
                {
                    context.ReportDiagnostic(Diagnostic.Create(
                        new DiagnosticDescriptor("mavlink01", "Exception", "Exception thrown: {0}", "category", DiagnosticSeverity.Error, true),
                        root.Symbol.Locations.First(), e
                    ));
                }
            }
        }
    }
}