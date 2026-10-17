using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class MidnightRuleTests
{
    [TestMethod]
    public void Check_EndAfterStart_ReturnsNoViolation()
    {
        var violation = MidnightRule.Check(new TimeSlot(new TimeOnly(8, 0), new TimeOnly(13, 0)));

        Assert.IsNull(violation);
    }

    [TestMethod]
    [DataRow(13, 0, 8, 0)]
    [DataRow(8, 0, 8, 0)]
    [DataRow(22, 0, 0, 30)]
    public void Check_EndNotAfterStart_ReturnsViolation(int startHour, int startMinute, int endHour, int endMinute)
    {
        var violation = MidnightRule.Check(new TimeSlot(new TimeOnly(startHour, startMinute), new TimeOnly(endHour, endMinute)));

        Assert.IsNotNull(violation);
        Assert.AreEqual(MidnightRule.Name, violation.Rule);
    }

    [TestMethod]
    public void Check_DurationZeroAfterRounding_ReturnsViolation()
    {
        var slot = TimeRounding.Round(new TimeSlot(new TimeOnly(8, 1), new TimeOnly(8, 5)));

        var violation = MidnightRule.Check(slot);

        Assert.IsNotNull(violation);
    }
}
