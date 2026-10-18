namespace Kontor.Core.Rules;

/// <summary>
/// The end must lie after the start on the same day, and the rounded duration must be more than zero;
/// no booking across midnight (FR-005, FR-012).
/// </summary>
public static class MidnightRule
{
    public const string Name = "Zeitraum";

    public static RuleViolation? Check(TimeSlot slot)
    {
        if (slot.End <= slot.Start)
        {
            return new RuleViolation(Name, "Das Ende muss nach dem Beginn liegen. Arbeit über Mitternacht bitte als zwei Buchungen erfassen, eine pro Tag.");
        }

        return slot.RoundedDuration > TimeSpan.Zero
            ? null
            : new RuleViolation(Name, "Die Buchung dauert gerundet 0 Minuten. Buchungen unter 8 Minuten werden nicht gespeichert.");
    }
}
