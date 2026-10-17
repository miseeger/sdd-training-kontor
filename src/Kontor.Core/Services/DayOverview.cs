using Kontor.Core.Domain;

namespace Kontor.Core.Services;

/// <summary>All bookings of a person on one day and the daily total (FR-011).</summary>
public sealed record DayOverview(IReadOnlyList<TimeEntry> Entries, TimeSpan Total);
