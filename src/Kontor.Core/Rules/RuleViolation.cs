namespace Kontor.Core.Rules;

/// <summary>A violated rule with a message for the user (FR-009).</summary>
public sealed record RuleViolation(string Rule, string Message);
