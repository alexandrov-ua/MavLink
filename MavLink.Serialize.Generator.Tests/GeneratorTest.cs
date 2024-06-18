using FluentAssertions;
using MavLink.Serialize.Generator.Tests.Tools;
using Microsoft.CodeAnalysis;
using Xunit.Abstractions;

namespace MavLink.Serialize.Generator.Tests;

public class DialectGeneratorTest
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly GeneratorTestHelper<Serialize.Generator.DialectGenerator> _generatorTestHelper;

    public DialectGeneratorTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _generatorTestHelper = new GeneratorTestHelper<Serialize.Generator.DialectGenerator>();
        //_generatorTestHelper.AddDependenciesSource(DependentSources.List);
    }

    [Fact]
    public void Positive()
    {
        var (output, diagnostics) = _generatorTestHelper.GetGeneratedOutput(
            """
            using MavLink.Serialize.Dialects;
            
            namespace MavLink.Tests;
            
            
            [Dialect("minimal.xml")]
            public partial class MinimalDialect
            {
                
            }  
            """);

        diagnostics.Where(t=>t.Severity > DiagnosticSeverity.Warning).Should().BeEmpty();
        _testOutputHelper.WriteLine(output);
    }
    
    [Fact]
    public void Positive1()
    {
        var (output, diagnostics) = _generatorTestHelper.GetGeneratedOutput(
            """
            using MavLink.Serialize.Dialects;

            namespace MavLink.Tests;


            [Dialect("common.xml")]
            public partial class MinimalDialect
            {
                
            }
            """);

        diagnostics.Where(t=>t.Severity > DiagnosticSeverity.Warning).Should().BeEmpty();
        _testOutputHelper.WriteLine(output);
    }
    
    [Fact]
    public void Negative()
    {
        var (output, diagnostics) = _generatorTestHelper.GetGeneratedOutput(
            """
            namespace MavLink.Tests;


            [Dialect("minimal.xml")]
            public partial class MinimalDialect
            {
                
            }
            """);

        diagnostics.Should().Contain(t => t.Id == "CS0246");
        _testOutputHelper.WriteLine(output);
    }
    
    [Fact]
    public void Negative2()
    {
        var (output, diagnostics) = _generatorTestHelper.GetGeneratedOutput(
            """
            using MavLink.Serialize.Dialects;
            
            namespace MavLink.Tests;


            [Dialect("minimal.xml")]
            public partial class MinimalDialect
            {
                sdfsdfdssd
            }
            """);

        diagnostics.Should().Contain(t => t.Id == "CS1519");
        _testOutputHelper.WriteLine(output);
    }
    
    [Fact]
    public void Negative3()
    {
        var (output, diagnostics) = _generatorTestHelper.GetGeneratedOutput(
            """
            using MavLink.Serialize.Dialects;

            namespace MavLink.Tests;


            [Dialect("fkdjgdfkjghk.xml")]
            public partial class MinimalDialect
            {
                sdfsdfdssd
            }
            """);

        diagnostics.Should().Contain(t => t.Id == "mavlink02");
        _testOutputHelper.WriteLine(output);
    }
}