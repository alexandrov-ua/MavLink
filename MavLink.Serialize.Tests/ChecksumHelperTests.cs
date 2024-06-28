using FluentAssertions;

namespace MavLink.Serialize.Tests;

public class ChecksumHelperTests
{
    [Fact]
    public void Calculate()
    {
        Span<byte> buffer = new byte[] //TerrainData pocket
        {
            0x27, 0x00, 0x00, 0x7B, 0xFF, 0x7B, 0x86, 0x00, 0x00, 0x0C, 0x00, 0x00, 0x00, 0x17, 0x00, 0x00, 0x00,
            0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x03, 0x00, 0x04, 0x00, 0x05, 0x00, 0x06, 0x00, 0x07, 0x00, 0x08, 0x00,
            0x09, 0x00, 0x0A, 0x00, 0x0B, 0x00, 0x0C, 0x00, 0x0D, 0x00, 0x0E, 0x00, 0x0F
        };

        var result = ChecksumHelper.Calculate(buffer, 229);

        result.Should().Be(21332);
    }
}