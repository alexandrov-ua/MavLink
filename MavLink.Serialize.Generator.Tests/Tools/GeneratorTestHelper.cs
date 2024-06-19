using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MavLink.Serialize.Generator.Tests.Tools;

public class GeneratorTestHelper<TGenerator> where TGenerator : ISourceGenerator, new()
{
    public (string, IEnumerable<Diagnostic>) GetGeneratedOutput(string source)
    {
        Compilation inputCompilation = CreateCompilation(new[] {source});
        var generator = new TGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
            out var diagnostics);

        return (outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString() ?? "",
            outputCompilation.GetDiagnostics().Concat(diagnostics));
    }

    private static Compilation CreateCompilation(string[] source)
        => CSharpCompilation.Create("compilation",
            source.Select(t => CSharpSyntaxTree.ParseText(t, path: typeof(GeneratorTestHelper<>).Assembly.Location)).ToArray(),
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .Cast<MetadataReference>(),
            new CSharpCompilationOptions(OutputKind.NetModule));
}