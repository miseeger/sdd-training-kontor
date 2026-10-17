using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class MaxDailyHoursRuleTests
{
    private static TimeSlot Slot(int startHour, int endHour) => new(new TimeOnly(startHour, 0), new TimeOnly(endHour, 0));

    [TestMethod]
    public void Check_MoreThanTenHours_ReturnsViolation()
    {
        var violation = MaxDailyHoursRule.Check(Slot(13, 19), [Slot(7, 12)]);

        Assert.IsNotNull(violation);
        Assert.AreEqual(MaxDailyHoursRule.Name, violation.Rule);
    }

    [TestMethod]
    public void Check_ExactlyTenHours_ReturnsNoViolation()
    {
        var violation = MaxDailyHoursRule.Check(Slot(13, 18), [Slot(7, 12)]);

        Assert.IsNull(violation);
    }
}
