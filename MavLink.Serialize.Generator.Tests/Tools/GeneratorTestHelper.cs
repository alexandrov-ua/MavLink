using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MavLink.Serialize.Generator.Tests.Tools;

public class GeneratorTestHelper<TGenerator> where TGenerator : ISourceGenerator, new()
{
    public (string, IEnumerable<Diagnostic>) GetGeneratedOutput(string source)
    {
        Compilation inputCompilation = CreateCompilation(new[] { source });
        var generator = new TGenerator();

        var xmlPaths = Path.Combine(
            Path.GetDirectoryName(typeof(GeneratorTestHelper<>).Assembly.Location), 
            "DialectXmls");
        var files = Directory.GetFiles(xmlPaths)
            .Select(t => Path.GetFileName(t))
            .Where(t => Path.GetExtension(t).ToLower() == ".xml")
            .Select(t => Path.Combine("DialectXmls", t))
            .Select(t => new CustomAdditionalText(t))
            .Cast<AdditionalText>()
            .ToList();
        
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator)
            .AddAdditionalTexts(ImmutableArray.CreateRange(files));
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
            out var diagnostics);

        return (outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString() ?? "",
            outputCompilation.GetDiagnostics().Concat(diagnostics));
    }

    private static Compilation CreateCompilation(string[] source)
        => CSharpCompilation.Create("compilation",
            source.Select(t => CSharpSyntaxTree.ParseText(t, path: typeof(GeneratorTestHelper<>).Assembly.Location))
                .ToArray(),
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .Cast<MetadataReference>(),
            new CSharpCompilationOptions(OutputKind.NetModule));
}