# Data Model: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-17

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
| `Start` | `TimeOnly` | required, auf Viertelstunde gerundet |
| `End` | `TimeOnly` | required, auf Viertelstunde gerundet, nach `Start` |
| `Note` | `string?` | optional, max. 500 Zeichen |

**Unveränderlich**: kein Update, kein Delete (Prinzip III, FR-010). Index auf
(`EmployeeId`, `Date`) für das Laden der Tagesbuchungen.

## Regeln (Kontor.Core/Rules)

| Rule | Prüft | Quelle |
|---|---|---|
| `TimeRounding` | Beginn und Ende einzeln auf die nächste Viertelstunde | FR-004 |
| `MidnightRule` | gerundetes Ende nach gerundetem Beginn, kein Ende nach Mitternacht | FR-005, FR-012 |
| `OverlapRule` | keine Überschneidung mit Buchungen derselben Person am Tag; Berührung erlaubt | FR-006 |
| `BreakRule` | > 6 h → Pause ≥ 30 min, > 9 h → Pause ≥ 45 min; nur Lücken ≥ 15 min zählen | FR-007 |
| `MaxDailyHoursRule` | Tagesarbeitszeit ≤ 10 h | FR-008 |

## BookingResult

| Field | Type | Bedeutung |
|---|---|---|
| `Succeeded` | `bool` | gespeichert ja/nein |
| `ViolatedRule` | `string?` | Name der verletzten Regel bei Ablehnung (FR-009) |
| `Message` | `string?` | Hinweis für die Nutzerin |
| `Entry` | `TimeEntry?` | die gespeicherte, gerundete Buchung |
