# Implementation Plan: Wochenübersicht

**Branch**: `002-wochenuebersicht` | **Date**: 2026-10-18 | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/002-wochenuebersicht/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Eine neue, rein lesende Seite zeigt für eine gewählte Person die Buchungen einer Woche (Montag bis
Sonntag), je Tag mit Tagessumme, dazu Wochensumme und Summe je Projekt; Blättern zur vorigen und
nächsten Woche. Technisch: Die Woche und die Summen entstehen in `Kontor.Core` in einem neuen
`WeekOverviewService`, der die bestehende Schnittstelle `ITimeEntryRepository` für jeden Tag der
Woche liest und die gerundete Dauer aus Feature 001 (`TimeEntry.RoundedDuration`) verwendet.
`Kontor.Web` bekommt eine Seite `WeekOverview.razor`, erreichbar von der Erfassungsseite und aus
der Navigation. Keine Änderung am Datenbankschema, keine Migration, keine neuen Pakete.

## Technical Context

**Language/Version**: C# 14 / .NET 10 (LTS)

**Primary Dependencies**: wie Feature 001 (Blazor Web App Interactive Server, EF Core 10); keine neuen

**Storage**: SQL Server LocalDB `KontorDb`, nur lesend; Schema unverändert

**Testing**: MSTest (Paket `MSTest`, Version zentral), Test-Double `InMemoryTimeEntryRepository` aus Feature 001

**Target Platform**: Windows-Entwicklungsrechner mit LocalDB, Browser

**Project Type**: Webanwendung (Blazor Server UI) mit Class Libraries, wie Feature 001

**Performance Goals**: Wochenübersicht spürbar sofort (unter 1 Sekunde) bei rund 40 Mitarbeitenden

**Constraints**: nur lesen (FR-006); gleiche Rundung wie die Erfassung (FR-007); bestehende Projekte und Schnittstellen weiterverwenden

**Scale/Scope**: rund 40 Mitarbeitende, höchstens einige Dutzend Buchungen pro Person und Woche, eine neue Seite

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Prinzip | Prüfung vor Phase 0 | Prüfung nach Phase 1 |
|---|---|---|
| I. Fachlogik-First | ✅ Wochengrenzen und Summen liegen in `Kontor.Core` | ✅ `Week`, `WeekOverviewService` in `Kontor.Core`; die Seite stellt nur dar |
| II. Test-First (NON-NEGOTIABLE) | ✅ Tests vor der Implementierung für Wochengrenzen und Summen | ✅ `WeekTests`, `WeekOverviewServiceTests` mit dem selbst geschriebenen Test-Double |
| III. Nachvollziehbarkeit | ✅ Feature liest nur | ✅ kein Schreibzugriff, keine Bearbeiten- oder Löschen-Funktion |
| IV. Englische Bezeichner | ✅ | ✅ `Week`, `WeekOverview`, `ProjectTotal`, `WeekOverviewService` |
| V. Feste Paketversionen (1.1.0) | ✅ keine neuen Pakete | ✅ unverändert |

Ergebnis: bestanden, keine Abweichung.

## Project Structure

### Documentation (this feature)

```text
specs/002-wochenuebersicht/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

Kein `contracts/`: keine Schnittstelle für andere Systeme.

### Source Code (repository root)

```text
src/
├── Kontor.Core/
│   ├── Rules/Week.cs                         # neu: Montag bis Sonntag, vorige/nächste Woche
│   └── Services/
│       ├── WeekOverview.cs                   # neu: Tage, Wochensumme, Summen je Projekt
│       └── WeekOverviewService.cs            # neu: liest ITimeEntryRepository je Tag
├── Kontor.Infrastructure/                    # unverändert
└── Kontor.Web/
    ├── Components/Pages/WeekOverview.razor   # neu, Route /woche
    ├── Components/Pages/Booking.razor        # Link zur Wochenübersicht
    ├── Components/Layout/NavMenu.razor       # Eintrag Wochenübersicht
    └── Program.cs                            # WeekOverviewService registrieren

tests/
└── Kontor.Core.Tests/
    ├── Rules/WeekTests.cs                    # neu
    └── Services/WeekOverviewServiceTests.cs  # neu
```

**Structure Decision**: Keine neuen Projekte; das Feature erweitert `Kontor.Core` und
`Kontor.Web` der bestehenden Struktur aus Feature 001. `Kontor.Infrastructure` bleibt unverändert.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| – | – | – |

Keine Abweichungen.
