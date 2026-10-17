using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class TimeRoundingTests
{
    [TestMethod]
    [DataRow(8, 7, 8, 0)]
    [DataRow(12, 53, 13, 0)]
    [DataRow(8, 0, 8, 0)]
    [DataRow(16, 52, 16, 45)]
    public void RoundToQuarterHour_RoundsToNearestQuarter(int hour, int minute, int expectedHour, int expectedMinute)
    {
        var rounded = TimeRounding.RoundToQuarterHour(new TimeOnly(hour, minute));

        Assert.AreEqual(new TimeOnly(expectedHour, expectedMinute), rounded);
    }
}
