using Kontor.Core.Rules;

namespace Kontor.Core.Tests.Rules;

[TestClass]
public sealed class OverlapRuleTests
{
    private static readonly TimeSlot Morning = new(new TimeOnly(8, 0), new TimeOnly(13, 0));

    [TestMethod]
    public void Check_Overlapping_ReturnsViolation()
    {
        var violation = OverlapRule.Check(new TimeSlot(new TimeOnly(12, 0), new TimeOnly(14, 0)), [Morning]);

        Assert.IsNotNull(violation);
        Assert.AreEqual(OverlapRule.Name, violation.Rule);
    }

    [TestMethod]
    public void Check_Touching_ReturnsNoViolation()
    {
        var violation = OverlapRule.Check(new TimeSlot(new TimeOnly(13, 0), new TimeOnly(14, 0)), [Morning]);

        Assert.IsNull(violation);
    }

    [TestMethod]
    public void Check_NoExistingEntries_ReturnsNoViolation()
    {
        var violation = OverlapRule.Check(Morning, []);

        Assert.IsNull(violation);
    }

    [TestMethod]
    public void Check_UsesRecordedTimes_OverlapWithinRoundingWindowIsRejected()
    {
        var existing = new[] { new TimeSlot(new TimeOnly(8, 0), new TimeOnly(12, 52)) };

        var violation = OverlapRule.Check(new TimeSlot(new TimeOnly(12, 50), new TimeOnly(14, 0)), existing);

        Assert.IsNotNull(violation);
    }
}
