using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class TimeRoundingTests
{
    [TestMethod]
    [DataRow(4, 46, 4, 45)]
    [DataRow(0, 7, 0, 0)]
    [DataRow(0, 8, 0, 15)]
    [DataRow(0, 15, 0, 15)]
    [DataRow(5, 0, 5, 0)]
    public void RoundDuration_RoundsCommerciallyToQuarterHour(int hours, int minutes, int expectedHours, int expectedMinutes)
    {
        var rounded = TimeRounding.RoundDuration(new TimeSpan(hours, minutes, 0));

        Assert.AreEqual(new TimeSpan(expectedHours, expectedMinutes, 0), rounded);
    }
}
