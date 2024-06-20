using System.Text;
using FluentAssertions;
using Xunit.Abstractions;

namespace MavLink.Serialize.Tests;

public class BitConverterHelperTest
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BitConverterHelperTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public void Foo()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x53, 0x54, 0x41, 0x54, 0x5f, 0x52, 0x55, 0x4e, 0x54, 0x49, 0x4d, 0x45, 0x00, 0x00, 0x00, 0x00, 0x06, 0xc7,
            0x87
        };
        var result = BitConverterHelper.ReadString(16, ref data);
        result.Should().Be("STAT_RUNTIME");
        data.Length.Should().Be(3);
    }
    
    [Fact]
    public void Bar()
    {
        var buffer = new byte[20];
        Span<byte> span = buffer; 
        BitConverterHelper.WriteString("STAT_RUNTIME",16, ref span);
        
        Span<byte> expected = new byte[]
        {
            0x53, 0x54, 0x41, 0x54, 0x5f, 0x52, 0x55, 0x4e, 0x54, 0x49, 0x4d, 0x45, 0x00, 0x00, 0x00, 0x00
        };
        span.Length.Should().Be(4);
        
        Assert.Equal(expected.ToArray(), ((Span<byte>)buffer).Slice(0,16).ToArray());
    }
}