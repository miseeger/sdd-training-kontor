# Implementation Plan: Zeit erfassen

**Branch**: `001-zeit-erfassen` | **Date**: 2026-10-18 (aktualisiert nach Clarifications 2026-10-18, Rundung, und Constitution 1.1.0) | **Spec**: [spec.md](spec.md)

**Input**: Feature specification from `/specs/001-zeit-erfassen/spec.md`

**Note**: This template is filled in by the `/speckit-plan` command; its definition describes the execution workflow.

## Summary

Mitarbeitende buchen Arbeitszeit mit Datum, Beginn, Ende, Projekt und optionaler Notiz.
Kontor speichert Beginn und Ende so, wie sie erfasst wurden, rundet die Dauer jeder Buchung
kaufmännisch auf 15 Minuten und prüft Überschneidung, Mindestpause, Tageshöchstarbeitszeit und
Buchungen über Mitternacht mit den erfassten Zeiten, bevor gespeichert wird; gespeicherte
Buchungen sind unveränderlich. Die Tagessumme ist die Summe der gerundeten Dauern. Technisch: Alle Regeln liegen als reine, ohne Datenbank
testbare Klassen in `Kontor.Core`; `Kontor.Infrastructure` speichert mit EF Core in SQL Server
(LocalDB); `Kontor.Web` ist eine Blazor Web App mit globalem Render-Modus
`InteractiveServer` und einer Seite zum Erfassen mit der Tagesliste.

## Technical Context

**Language/Version**: C# 14 / .NET 10 (LTS)

**Primary Dependencies**: ASP.NET Core Blazor Web App (Interactive Server), EF Core 10
(`Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.EntityFrameworkCore.Design`),
`dotnet-ef` als lokales Tool

**Storage**: SQL Server LocalDB, Datenbank `KontorDb`; Schema über EF-Core-Migrationen

**Testing**: MSTest über das Paket `MSTest` (Version zentral in `Directory.Packages.props`), ohne Mock-Bibliotheken

**Target Platform**: Windows-Entwicklungsrechner mit LocalDB, Browser

**Project Type**: Webanwendung (Blazor Server UI) mit Class Libraries für Fachlogik und Persistenz

**Performance Goals**: Erfassen einer Buchung in unter 30 Sekunden für die Nutzerin (SC-001);
Prüfen und Speichern spürbar sofort (unter 1 Sekunde)

**Constraints**: Alle Fachregeln ohne Datenbank testbar; keine Änderung oder Löschung
gespeicherter Buchungen; keine Datenmigration bestehender Buchungen; Paketversionen fest und
zentral

**Scale/Scope**: rund 40 Mitarbeitende, etwa 20.000 Buchungen pro Jahr, eine Seite

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

| Prinzip | Prüfung vor Phase 0 | Prüfung nach Phase 1 |
|---|---|---|
| I. Fachlogik-First | ✅ Rundung der Dauer, Überschneidung, Pause, Höchstarbeitszeit und Mitternacht liegen in `Kontor.Core`; `Kontor.Web` und `Kontor.Infrastructure` rufen nur auf | ✅ `data-model.md` und Struktur: Regeln in `Kontor.Core/Rules`, `TimeEntryService` orchestriert; Web und Infrastructure ohne Regelcode |
| II. Test-First (NON-NEGOTIABLE) | ✅ Jede Regel bekommt MSTest-Tests vor der Implementierung | ✅ `tests/Kontor.Core.Tests`; Test-Double `InMemoryTimeEntryRepository` selbst geschrieben, keine Mock-Bibliothek |
| III. Nachvollziehbarkeit | ✅ Spec verbietet Ändern und Löschen (FR-010) | ✅ Repository bietet nur Hinzufügen und Lesen; keine Bearbeiten- oder Löschen-Funktion in der Oberfläche |
| IV. Englische Bezeichner | ✅ | ✅ `TimeEntry`, `Employee`, `Project`, `TimeRounding`, `BreakRule` usw. (siehe `data-model.md`) |
| V. Feste Paketversionen (1.1.0) | ✅ NuGet-Pakete mit festen Versionen in `Directory.Packages.props`; SDK-Version fest in `global.json`; `dotnet-ef` fest im Tool-Manifest | ✅ Alle NuGet-Pakete einschließlich `MSTest` fest in `Directory.Packages.props`; `global.json` mit fester SDK-Version; `dotnet-ef` 10.0.13 fest in `.config/dotnet-tools.json`; nirgends Platzhalter |

Ergebnis: bestanden, keine Abweichung (geprüft gegen Constitution 1.1.0).

## Project Structure

### Documentation (this feature)

```text
specs/001-zeit-erfassen/
├── plan.md              # This file (/speckit-plan command output)
├── research.md          # Phase 0 output (/speckit-plan command)
├── data-model.md        # Phase 1 output (/speckit-plan command)
├── quickstart.md        # Phase 1 output (/speckit-plan command)
└── tasks.md             # Phase 2 output (/speckit-tasks command - NOT created by /speckit-plan)
```

Kein `contracts/`: Kontor bietet in dieser Phase keine Schnittstelle für andere Systeme an
(keine Web-API, keine Kommandozeile).

### Source Code (repository root)

```text
Kontor.slnx
global.json                         # nur die SDK-Version
Directory.Build.props               # net10.0, Nullable, ImplicitUsings, TreatWarningsAsErrors
Directory.Packages.props            # zentrale, feste Paketversionen
.config/dotnet-tools.json           # dotnet-ef als lokales Tool

src/
├── Kontor.Core/                    # Class Library, keine Abhängigkeit zu EF Core oder Blazor
│   ├── Domain/                     # Employee, Project, TimeEntry
│   ├── Rules/                      # TimeRounding, OverlapRule, BreakRule, MaxDailyHoursRule, MidnightRule
│   ├── Abstractions/               # ITimeEntryRepository, IEmployeeRepository, IProjectRepository
│   └── Services/                   # TimeEntryService, BookingResult
├── Kontor.Infrastructure/          # Class Library, EF Core SQL Server
│   ├── KontorDbContext.cs
│   ├── Configurations/             # Entity-Konfigurationen, Seed-Daten
│   ├── Repositories/
│   ├── Migrations/
│   └── DependencyInjection.cs
└── Kontor.Web/                     # Blazor Web App, InteractiveServer global
    ├── Components/Pages/Booking.razor
    ├── Program.cs
    └── appsettings.Development.json

tests/
└── Kontor.Core.Tests/              # MSTest-Paket
    ├── Rules/
    ├── Services/
    └── Fakes/                      # InMemoryTimeEntryRepository u. a.
```

**Structure Decision**: Drei Projekte unter `src/` plus ein Testprojekt unter `tests/`, wie im
Planungsauftrag vorgegeben. `Kontor.Web` referenziert `Kontor.Core` und
`Kontor.Infrastructure` (nur für die Registrierung im DI-Container); `Kontor.Infrastructure`
referenziert `Kontor.Core`; `Kontor.Core` referenziert nichts.

## Complexity Tracking

> **Fill ONLY if Constitution Check has violations that must be justified**

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|-------------------------------------|
| – | – | – |

Keine Abweichungen. Die Abweichung zu `dotnet-ef` im Tool-Manifest (bisher hier begründet, als T041 von `/speckit-converge` gemeldet) ist mit Constitution 1.1.0 entfallen: Prinzip V nimmt lokale Tools ausdrücklich aus und verlangt dort feste Versionen.
Die Abweichung aus dem ersten Planlauf (Version von `MSTest.Sdk` in `global.json`) ist entfallen: Das Testprojekt nutzt das Paket `MSTest` über Central Package Management (siehe research.md, R8).
