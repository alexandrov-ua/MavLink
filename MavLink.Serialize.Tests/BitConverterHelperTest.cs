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
    public void Read_Byte()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x53, 0x54, 0x41, 0x54, 0x5f
        };
        var value = BitConverterHelper.Read<byte>(ref data);
        value.Should().Be(83);
        data.Length.Should().Be(4);
    }

    [Fact]
    public void Read_Short()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x53, 0x54, 0x41, 0x54, 0x5f
        };
        var value = BitConverterHelper.Read<short>(ref data);
        value.Should().Be(21587);
        data.Length.Should().Be(3);
    }

    [Fact]
    public void Read_Int()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x53, 0x54, 0x41, 0x00, 0x5f
        };
        var value = BitConverterHelper.Read<int>(ref data);
        value.Should().Be(4281427);
        data.Length.Should().Be(1);
    }

    [Fact]
    public void Read_Int_WithTrimmedZeroBytes()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x53, 0x54, 0x41,
        };
        var value = BitConverterHelper.Read<int>(ref data);
        value.Should().Be(4281427);
        data.Length.Should().Be(0);
    }

    [Fact]
    public void Read_Int_WithTrimmedZeroBytes_3()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x53,
        };
        var value = BitConverterHelper.Read<int>(ref data);
        value.Should().Be(83);
        data.Length.Should().Be(0);
    }

    [Fact]
    public void Read_Int_FromEmptyArray()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
        };
        var value = BitConverterHelper.Read<int>(ref data);
        value.Should().Be(0);
        data.Length.Should().Be(0);
    }

    [Fact]
    public void Read_Float()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0x79, 0xE9, 0xF6, 0x42,
        };
        var value = BitConverterHelper.Read<float>(ref data);
        value.Should().Be(123.456f);
        data.Length.Should().Be(0);
    }

    [Fact]
    public void Write_Byte()
    {
        Span<byte> buffer = new byte[5];
        var tmpBuffer = buffer;
        BitConverterHelper.Write<byte>(83, ref tmpBuffer);
        tmpBuffer.Length.Should().Be(4);
        Assert.Equal(new byte[] {0x53, 0x00, 0x00, 0x00, 0x00}, buffer.ToArray());
    }

    [Fact]
    public void Write_Int()
    {
        Span<byte> buffer = new byte[5];
        var tmpBuffer = buffer;
        BitConverterHelper.Write<int>(4281427, ref tmpBuffer);
        tmpBuffer.Length.Should().Be(1);
        Assert.Equal(new byte[] {0x53, 0x54, 0x41, 0x00, 0x00}, buffer.ToArray());
    }

    [Fact]
    public void Write_Float()
    {
        Span<byte> buffer = new byte[5];
        var tmpBuffer = buffer;
        BitConverterHelper.Write<float>(123.456f, ref tmpBuffer);
        tmpBuffer.Length.Should().Be(1);
        Assert.Equal(new byte[] {0x79, 0xE9, 0xF6, 0x42, 0x00}, buffer.ToArray());
    }


    [Fact]
    public void ReadArray_Int()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0xd2, 0x02, 0x96, 0x49, 0x53, 0x54, 0x41, 0x00, 0x5f
        };
        var values = BitConverterHelper.ReadArray(new int[2], ref data);
        data.Length.Should().Be(1);
        Assert.Equal(new int[] {1234567890, 4281427}, values);
    }

    [Fact]
    public void ReadArray_WithTrimmedZeroBytes()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0xd2, 0x02, 0x96, 0x49, 0x53, 0x54, 0x41
        };
        var values = BitConverterHelper.ReadArray(new int[2], ref data);
        data.Length.Should().Be(0);
        Assert.Equal(new int[] {1234567890, 4281427}, values);
    }

    [Fact]
    public void ReadArray_WithTrimmedZeroBytes2()
    {
        ReadOnlySpan<byte> data = new byte[]
        {
            0xd2, 0x02, 0x96, 0x49,
        };
        var values = BitConverterHelper.ReadArray(new int[2], ref data);
        data.Length.Should().Be(0);
        Assert.Equal(new int[] {1234567890, 0}, values);
    }

    [Fact]
    public void WriteArray_Int()
    {
        Span<byte> buffer = new byte[10];
        var tmpBuffer = buffer;
        BitConverterHelper.WriteArray(new int[] {1234567890, 4281427}, ref tmpBuffer);
        tmpBuffer.Length.Should().Be(2);
        Assert.Equal(new byte[] {0xd2, 0x02, 0x96, 0x49, 0x53, 0x54, 0x41, 0x00, 0x00, 0x00},
            buffer.ToArray());
    }

    [Fact]
    public void ReadString_Test()
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
    public void WriteString_Test()
    {
        var buffer = new byte[20];
        Span<byte> span = buffer;
        BitConverterHelper.WriteString("STAT_RUNTIME", 16, ref span);

        Span<byte> expected = new byte[]
        {
            0x53, 0x54, 0x41, 0x54, 0x5f, 0x52, 0x55, 0x4e, 0x54, 0x49, 0x4d, 0x45, 0x00, 0x00, 0x00, 0x00
        };
        span.Length.Should().Be(4);

        Assert.Equal(expected.ToArray(), ((Span<byte>) buffer).Slice(0, 16).ToArray());
    }
}