using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MavLink.Serialize.Generator;

internal class SyntaxReceiver : ISyntaxContextReceiver
{
    public List<ClassNodeInfo> Roots { get; } = new List<ClassNodeInfo>();


    public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclarationSyntax &&
            classDeclarationSyntax.AttributeLists.Count > 0 &&
            classDeclarationSyntax.AttributeLists
                .Any(al => al.Attributes
                    .Any(a => a.Name.ToString() == "Dialect")))
        {
            
            var classSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationSyntax);
            var attributeData = classSymbol?.GetAttributes()
                .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString() == "MavLink.Serialize.Dialects.DialectAttribute" /*typeof(DialectAttribute).FullName*/);
            var dependencies = classSymbol?.GetAttributes()
                .Where(ad =>
                    ad.AttributeClass?.ContainingNamespace.ToDisplayString() == "MavLink.Serialize.Dialects" && ad.AttributeClass.Name == "DialectDependencyAttribute")
                .Select(t => t.AttributeClass.TypeArguments.First())
                .Select(t => t.Name).ToArray();
            var filePath = attributeData?.ConstructorArguments.FirstOrDefault().Value as string;
            var sourceFilePath = context.SemanticModel.SyntaxTree.FilePath;
            Roots.Add(new ClassNodeInfo(
                classSymbol?.Name ?? "",
                classSymbol?.ContainingNamespace?.ToString() ?? "",
                filePath ?? "",
                sourceFilePath,
                classSymbol,
                dependencies
                ));

        }
    } 
}

public record class ClassNodeInfo(string DisplayName, string NameSpace, string FileName, string SourceFilePath, ISymbol Symbol, string[] DependencyClassNames)
{
}