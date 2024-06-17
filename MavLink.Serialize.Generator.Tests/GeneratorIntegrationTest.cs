using FluentAssertions;
using MavLink.Serialize.Dialects;


namespace MavLink.Serialize.Generator.Tests;

[Dialect("minimal.xml")]
public partial class MinimalDialect
{
    
}

public class GeneratorIntegrationTest
{
    [Fact]
    public void Foo()
    {
        MinimalDialect.Default.Should().NotBeNull();
    }
}