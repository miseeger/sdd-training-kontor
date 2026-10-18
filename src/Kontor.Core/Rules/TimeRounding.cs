namespace Kontor.Core.Rules;

/// <summary>
/// Rounds the duration of a booking commercially to the quarter hour (FR-004).
/// Start and end stay as recorded. With whole minutes a remainder below 8 minutes
/// rounds down, from 8 minutes up; exactly 7.5 minutes cannot occur.
/// </summary>
public static class TimeRounding
{
    private const int QuarterHourMinutes = 15;

    public static TimeSpan RoundDuration(TimeSpan duration)
    {
        var quarters = Math.Round(duration.TotalMinutes / QuarterHourMinutes, MidpointRounding.AwayFromZero);
        return TimeSpan.FromMinutes(quarters * QuarterHourMinutes);
    }
}
