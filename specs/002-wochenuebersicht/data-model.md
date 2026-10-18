# Data Model: Wochenübersicht

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-18

Keine neuen gespeicherten Entitäten. `Employee`, `Project` und `TimeEntry` kommen unverändert aus
Feature 001 (`specs/001-zeit-erfassen/data-model.md`); gelesen wird nur.

## Week (Kontor.Core/Rules)

| Field | Type | Bedeutung |
|---|---|---|
| `Monday` | `DateOnly` | erster Tag der Woche |
| `Sunday` | `DateOnly` | `Monday` + 6 Tage |
| `Days` | Liste von `DateOnly` | die sieben Tage, Montag bis Sonntag |

Operationen: `Containing(DateOnly date)` → Woche, die `date` enthält (FR-008); `Previous`,
`Next` → um sieben Tage verschoben (FR-005).

## WeekOverview (Kontor.Core/Services)

| Field | Type | Bedeutung |
|---|---|---|
| `Week` | `Week` | die gezeigte Woche |
| `Days` | Liste von `DayOverview` (aus Feature 001) | sieben Einträge, auch ohne Buchung (FR-002) |
| `Total` | `TimeSpan` | Summe der `RoundedDuration` aller Buchungen der Woche (FR-003) |
| `ProjectTotals` | Liste von `ProjectTotal` | je Projekt mit Buchungen, sortiert nach Schlüssel (FR-004) |

## ProjectTotal (Kontor.Core/Services)

| Field | Type | Bedeutung |
|---|---|---|
| `ProjectKey` | `string` | Projektschlüssel, z. B. `NSL-ALB-01` |
| `ProjectName` | `string` | Bezeichnung |
| `Total` | `TimeSpan` | Summe der `RoundedDuration` dieses Projekts in der Woche |

Invariante (SC-002, SC-003): Summe der Tagessummen = `Total` = Summe der `ProjectTotals`.
