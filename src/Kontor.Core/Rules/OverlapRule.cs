namespace Kontor.Core.Rules;

/// <summary>A booking must not overlap another booking of the same person; touching is allowed (FR-006).</summary>
public static class OverlapRule
{
    public const string Name = "Überschneidung";

    public static RuleViolation? Check(TimeSlot candidate, IReadOnlyCollection<TimeSlot> existing)
    {
        foreach (var slot in existing)
        {
            if (candidate.Start < slot.End && slot.Start < candidate.End)
            {
                return new RuleViolation(Name, $"Die Buchung überschneidet sich mit {slot.Start:HH\\:mm}–{slot.End:HH\\:mm}.");
            }
        }

        return null;
    }
}
