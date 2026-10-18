# Data Model: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-17, aktualisiert 2026-10-18 (Rundung der Dauer)

## Employee (Mitarbeitende)

| Field | Type | Constraints |
|---|---|---|
| `Id` | `int` | Primärschlüssel |
| `Name` | `string` | required, max. 100 Zeichen |

Fest hinterlegt (Seed): Ilka Brandt, Tomás Okoye, Juna Sato, Rik Halvorsen.

## Project (Projekt)

| Field | Type | Constraints |
|---|---|---|
| `Id` | `int` | Primärschlüssel |
| `Key` | `string` | required, max. 20 Zeichen, eindeutig |
| `Name` | `string` | required, max. 200 Zeichen |

Fest hinterlegt (Seed): `NSL-ALB-01` Antriebswartung Albatros, `KBA-TG3-04`
Schildgenerator-Kalibrierung Tiefgrund III, `HFL-PER-02` Lebenserhaltung prüfen Perihel.

## TimeEntry (Buchung)

| Field | Type | Constraints |
|---|---|---|
| `Id` | `int` | Primärschlüssel |
| `EmployeeId` | `int` | required, Fremdschlüssel auf `Employee` |
| `ProjectId` | `int` | required, Fremdschlüssel auf `Project` |
| `Date` | `DateOnly` | required |
| `Start` | `TimeOnly` | required, wie erfasst (minutengenau) |
| `End` | `TimeOnly` | required, wie erfasst, nach `Start` |
| `Note` | `string?` | optional, max. 500 Zeichen |
| `Duration` | `TimeSpan` | berechnet, nicht gespeichert: `End - Start` |
| `RoundedDuration` | `TimeSpan` | berechnet, nicht gespeichert: `Duration` kaufmännisch auf 15 Minuten (FR-004) |

**Unveränderlich**: kein Update, kein Delete (Prinzip III, FR-010). Index auf
(`EmployeeId`, `Date`) für das Laden der Tagesbuchungen.

**Keine Schemaänderung:** Die Spalten bleiben, wie sie sind. Buchungen, die vor dem
2026-10-18 gespeichert wurden, tragen gerundete Uhrzeiten; ihre `RoundedDuration` ist gleich
ihrer `Duration` (Assumptions der Spec).

## Regeln (Kontor.Core/Rules)

| Rule | Prüft | Quelle |
|---|---|---|
| `TimeRounding` | Dauer kaufmännisch auf 15 Minuten: Rest unter 8 Minuten ab, ab 8 Minuten auf | FR-004 |
| `MidnightRule` | Ende nach Beginn, gerundete Dauer größer 0, kein Ende nach Mitternacht | FR-005, FR-012 |
| `OverlapRule` | keine Überschneidung mit Buchungen derselben Person am Tag (erfasste Zeiten); Berührung erlaubt | FR-006, FR-013 |
| `BreakRule` | > 6 h → Pause ≥ 30 min, > 9 h → Pause ≥ 45 min; nur Lücken ≥ 15 min zählen (erfasste Zeiten) | FR-007, FR-013 |
| `MaxDailyHoursRule` | Tagesarbeitszeit ≤ 10 h (erfasste Zeiten) | FR-008, FR-013 |

## BookingResult

| Field | Type | Bedeutung |
|---|---|---|
| `Succeeded` | `bool` | gespeichert ja/nein |
| `ViolatedRule` | `string?` | Name der verletzten Regel bei Ablehnung (FR-009) |
| `Message` | `string?` | Hinweis für die Nutzerin |
| `Entry` | `TimeEntry?` | die gespeicherte Buchung (erfasste Zeiten, gerundete Dauer) |

## DayOverview (Tagesliste)

| Field | Type | Bedeutung |
|---|---|---|
| `Entries` | Liste von `TimeEntry` | Buchungen des Tages mit erfassten Uhrzeiten und `RoundedDuration` (FR-014) |
| `Total` | `TimeSpan` | Summe der `RoundedDuration` (FR-014, SC-003) |
