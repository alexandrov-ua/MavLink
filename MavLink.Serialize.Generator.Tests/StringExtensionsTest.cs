using FluentAssertions;

namespace MavLink.Serialize.Generator.Tests;

public class StringExtensionsTest
{
    [Fact]
    public void ToPascalCase_ReplaceSpace()
    {
        "hello world"
            .ToPascalCase()
            .Should()
            .Be("HelloWorld");
    }
    
    [Fact]
    public void ToPascalCase_ReplaceDash()
    {
        "hello-world"
            .ToPascalCase()
            .Should()
            .Be("HelloWorld");
    }
    
    [Fact]
    public void ToPascalCase_CrushTest()
    {
        " .><>=/*hello(*&=/*world&*("
            .ToPascalCase()
            .Should()
            .Be("HelloWorld");
    }
    
    [Fact]
    public void ToPascalCase_CrushTest1()
    {
        "!@#$%^&*()_+-=,./<>?;:'\"hi"
            .ToPascalCase()
            .Should()
            .Be("Hi");
    }
    
    [Fact]
    public void ToPascalCase_CrushTest2()
    {
        "!@#$ % ^ &*( ) _+-=, ./<>?;:'\""
            .ToPascalCase()
            .Should()
            .Be("");
    }

    [Fact]
    public void ToPascalCase_CrushTest3()
    {
        "a"
            .ToPascalCase()
            .Should()
            .Be("A");
    }



    [Fact]
    public void ToPascalCase_EmptyString()
    {
        ""
            .ToPascalCase()
            .Should()
            .Be("");
    }
    
    [Fact]
    public void ToCamelCase_EmptyString()
    {
        ""
            .ToCamelCase()
            .Should()
            .Be("");
    }
    
    [Fact]
    public void ToCamelCase_CrushTest()
    {
        " .><>=/*hello(*&=/*world&*("
            .ToCamelCase()
            .Should()
            .Be("helloWorld");
    }
    
    
    [Fact]
    public void ToPascalCaseIdentifier_Smoke()
    {
        "2 two 3 four"
            .ToPascalCaseIdentifier()
            .Should()
            .Be("The2Two3Four");
    }
    
    [Fact]
    public void ToCamelCaseIdentifier_Smoke()
    {
        "2 two 3 four"
            .ToCamelCaseIdentifier()
            .Should()
            .Be("the2Two3Four");
    }

    [Fact]
    public void EscapeKeywords_Smoke()
    {
        "continue"
            .EscapeKeywords()
            .Should()
            .Be("@continue");
    }
    
    [Fact]
    public void EscapeKeywords_EmptyString()
    {
        ""
            .EscapeKeywords()
            .Should()
            .Be("");
    }

}