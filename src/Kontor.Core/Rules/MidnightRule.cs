namespace Kontor.Core.Rules;

/// <summary>The end must lie after the start on the same day; no booking across midnight (FR-005, FR-012).</summary>
public static class MidnightRule
{
    public const string Name = "Zeitraum";

    public static RuleViolation? Check(TimeSlot slot) =>
        slot.End > slot.Start
            ? null
            : new RuleViolation(Name, "Das Ende muss nach dem Beginn liegen. Arbeit über Mitternacht bitte als zwei Buchungen erfassen, eine pro Tag.");
}
