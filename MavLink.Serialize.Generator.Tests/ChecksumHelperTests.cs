using FluentAssertions;

namespace MavLink.Serialize.Generator.Tests;

public class ChecksumHelperTests
{
    [Fact]
    public void Accumulate()
    {
        var acc = ChecksumHelper.Accumulate("PARAM_VALUE ", 0xFFFF);
        acc.Should().Be(12575);
        acc = ChecksumHelper.Accumulate("float ", acc);
        acc.Should().Be(11086);
    }
}