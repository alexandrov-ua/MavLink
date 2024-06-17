using System.Reflection;
using MavLink.Serialize.Dialects;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace MavLink.Serialize.Generator.Tests;

public class GeneratorTestHelper<TGenerator> where TGenerator : ISourceGenerator, new()
{
    private List<string> _sources = new List<string>();

    public void AddDependenciesSource(string source)
    {
        _sources.Add(source);
    }
    
    public void AddDependenciesSource(IEnumerable<string> sources)
    {
        _sources.AddRange(sources);
    }

    public (string, IEnumerable<Diagnostic>) GetGeneratedOutput(string source)
    {
        Compilation inputCompilation = CreateCompilation(_sources.Concat(new []{ source }).ToArray());
        var generator = new TGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation,
            out var diagnostics);

        return (outputCompilation.SyntaxTrees.Skip(1).LastOrDefault()?.ToString() ?? "", outputCompilation.GetDiagnostics().Concat(diagnostics));
    }
    
    private static Compilation CreateCompilation(string[] source)
        => CSharpCompilation.Create("compilation",
            source.Select(t=>CSharpSyntaxTree.ParseText(t)).ToArray(),
            // new[]
            // {
            //     MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location), 
            //     MetadataReference.CreateFromFile(typeof(IDialect).GetTypeInfo().Assembly.Location), 
            //     MetadataReference.CreateFromFile(typeof(ReadOnlySpan<>).GetTypeInfo().Assembly.Location)
            // },
            AppDomain.CurrentDomain.GetAssemblies()
                .Where(assembly => !assembly.IsDynamic)
                .Select(assembly => MetadataReference.CreateFromFile(assembly.Location))
                .Cast<MetadataReference>(),
            new CSharpCompilationOptions(OutputKind.NetModule));
}