namespace Kontor.Core.Rules;

/// <summary>Rounds start and end separately to the nearest quarter hour (FR-004).</summary>
public static class TimeRounding
{
    private const int QuarterHourMinutes = 15;
    private const int MinutesPerDay = 24 * 60;

    public static TimeOnly RoundToQuarterHour(TimeOnly time)
    {
        var minutes = time.Ticks / (double)TimeSpan.TicksPerMinute;
        var quarters = Math.Round(minutes / QuarterHourMinutes, MidpointRounding.AwayFromZero);
        var roundedMinutes = (int)quarters * QuarterHourMinutes % MinutesPerDay;
        return TimeOnly.FromTimeSpan(TimeSpan.FromMinutes(roundedMinutes));
    }

    public static TimeSlot Round(TimeSlot slot) =>
        new(RoundToQuarterHour(slot.Start), RoundToQuarterHour(slot.End));
}
