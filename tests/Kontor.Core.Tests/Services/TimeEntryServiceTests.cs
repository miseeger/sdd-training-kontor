using Kontor.Core.Rules;
using Kontor.Core.Services;
using Kontor.Core.Tests.Fakes;

namespace Kontor.Core.Tests.Services;

[TestClass]
public sealed class TimeEntryServiceTests
{
    private const int JunaSato = 3;
    private const int NslAlb01 = 1;
    private const int KbaTg304 = 2;
    private static readonly DateOnly November2 = new(2026, 11, 2);

    private InMemoryTimeEntryRepository _repository = null!;
    private TimeEntryService _service = null!;

    [TestInitialize]
    public void Setup()
    {
        _repository = new InMemoryTimeEntryRepository();
        _service = new TimeEntryService(_repository);
    }

    // User Story 1, scenario 1
    [TestMethod]
    public async Task BookAsync_KeepsRecordedTimes_AndRoundsDuration()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 7, 12, 53));

        Assert.IsTrue(result.Succeeded);
        Assert.AreEqual(new TimeOnly(8, 7), result.Entry!.Start);
        Assert.AreEqual(new TimeOnly(12, 53), result.Entry.End);
        Assert.AreEqual(new TimeSpan(4, 45, 0), result.Entry.RoundedDuration);
        var day = await _service.GetDayAsync(JunaSato, November2);
        Assert.AreEqual(new TimeSpan(4, 45, 0), day.Total);
    }

    // Edge case: rounded duration 0 (FR-005)
    [TestMethod]
    public async Task BookAsync_FiveMinutes_IsRejected()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 5, 8, 10));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(MidnightRule.Name, result.ViolatedRule);
    }

    // FR-013: overlap is checked with the recorded times, not the rounded ones
    [TestMethod]
    public async Task BookAsync_OverlapWithinRoundingWindow_IsRejected()
    {
        await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 12, 52));

        var result = await _service.BookAsync(Request(JunaSato, KbaTg304, November2, 12, 50, 14, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(OverlapRule.Name, result.ViolatedRule);
    }

    // FR-014, SC-003: the daily total is the sum of the rounded durations
    [TestMethod]
    public async Task GetDayAsync_TotalIsSumOfRoundedDurations()
    {
        await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 7, 12, 53));
        await _service.BookAsync(Request(JunaSato, KbaTg304, November2, 13, 30, 14, 38));

        var day = await _service.GetDayAsync(JunaSato, November2);

        Assert.AreEqual(new TimeSpan(5, 54, 0), day.Entries.Aggregate(TimeSpan.Zero, (sum, e) => sum + e.Duration));
        Assert.AreEqual(new TimeSpan(6, 0, 0), day.Total);
    }

    // User Story 1, scenario 2
    [TestMethod]
    public async Task BookAsync_Overlapping_IsRejectedAndNamesRule()
    {
        await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 13, 0));

        var result = await _service.BookAsync(Request(JunaSato, KbaTg304, November2, 12, 0, 14, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(OverlapRule.Name, result.ViolatedRule);
        Assert.HasCount(1, _repository.Entries);
    }

    // User Story 1, scenario 3
    [TestMethod]
    public async Task BookAsync_Touching_IsSaved()
    {
        await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 13, 0));

        var result = await _service.BookAsync(Request(JunaSato, KbaTg304, November2, 13, 0, 14, 0));

        Assert.IsTrue(result.Succeeded);
        Assert.HasCount(2, _repository.Entries);
    }

    private const int RikHalvorsen = 4;
    private const int HflPer02 = 3;
    private static readonly DateOnly November3 = new(2026, 11, 3);

    // User Story 2, scenario 1
    [TestMethod]
    public async Task BookAsync_SevenHoursWithoutBreak_IsRejected()
    {
        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 7, 0, 14, 0));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(BreakRule.Name, result.ViolatedRule);
    }

    // User Story 2, scenario 2
    [TestMethod]
    public async Task BookAsync_WithThirtyMinuteBreak_IsSaved()
    {
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 7, 0, 11, 0));

        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 11, 30, 14, 0));

        Assert.IsTrue(result.Succeeded);
    }

    // User Story 2, scenario 3: the new booking is rejected, existing ones stay unchanged
    // FR-013: the maximum applies to the recorded working time
    [TestMethod]
    public async Task BookAsync_TenRecordedHours_IsSavedAlthoughRoundedTotalIsHigher()
    {
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 6, 0, 9, 23));
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 10, 0, 13, 23));

        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 14, 0, 17, 14));

        Assert.IsTrue(result.Succeeded);
        var day = await _service.GetDayAsync(RikHalvorsen, November3);
        Assert.AreEqual(new TimeSpan(10, 15, 0), day.Total);
    }

    [TestMethod]
    public async Task BookAsync_ExceedingTenHours_IsRejectedAndExistingEntriesStay()
    {
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 7, 0, 12, 0));
        await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 12, 45, 16, 45));

        var result = await _service.BookAsync(Request(RikHalvorsen, HflPer02, November3, 17, 0, 18, 30));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(MaxDailyHoursRule.Name, result.ViolatedRule);
        Assert.HasCount(2, _repository.Entries);
    }

    // data-model TimeEntry.Note: optional, max. 500 Zeichen
    [TestMethod]
    public async Task BookAsync_NoteLongerThan500Characters_IsRejected()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 12, 0, new string('x', 501)));

        Assert.IsFalse(result.Succeeded);
        Assert.AreEqual(TimeEntryService.NoteRuleName, result.ViolatedRule);
        Assert.HasCount(0, _repository.Entries);
    }

    [TestMethod]
    public async Task BookAsync_NoteWith500Characters_IsSaved()
    {
        var result = await _service.BookAsync(Request(JunaSato, NslAlb01, November2, 8, 0, 12, 0, new string('x', 500)));

        Assert.IsTrue(result.Succeeded);
    }

    internal static BookingRequest Request(int employeeId, int projectId, DateOnly date, int startHour, int startMinute, int endHour, int endMinute, string? note = null) =>
        new(employeeId, projectId, date, new TimeOnly(startHour, startMinute), new TimeOnly(endHour, endMinute), note);
}
