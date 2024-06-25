using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
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
            var filesDictionary = context.AdditionalFiles
                .Where(at => at.Path.EndsWith(".xml"))
                .ToDictionary(k => Path.GetFileName(k.Path), v=>v);
            foreach (var root in receiver.Roots)
            {
                try
                {
                     if (!filesDictionary.ContainsKey(Path.GetFileName(root.FilePath)))
                     {
                        context.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor("mavlink02", "File doesn't exists", "File {0} doesn't exists", "category", DiagnosticSeverity.Error, true),
                            root.Symbol.Locations.First(), root.FilePath
                        ));
                        continue;
                    }


                    var fileText = filesDictionary[root.FilePath].GetText(CancellationToken.None);
                    var definition = DialectXmlParser.Parse(new MemoryStream(Encoding.UTF8.GetBytes(fileText.ToString())));

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