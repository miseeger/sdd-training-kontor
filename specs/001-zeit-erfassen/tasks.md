---

description: "Task list for feature 001 Zeit erfassen"
---

# Tasks: Zeit erfassen

**Input**: Design documents from `/specs/001-zeit-erfassen/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md

**Tests**: Enthalten. Die Constitution verlangt Test-First für alle Fachregeln in `Kontor.Core` (Prinzip II, NON-NEGOTIABLE); Test-Doubles werden selbst geschrieben.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Path Conventions

- Projekte unter `src/`, Tests unter `tests/`, Lösungsdateien im Repository-Wurzelverzeichnis (siehe plan.md, Project Structure)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [X] T001 `global.json` anlegen: nur die .NET-10-SDK-Version mit `rollForward: latestFeature`
- [X] T002 `Kontor.slnx` und die Ordner `src/` und `tests/` anlegen
- [X] T003 [P] `Directory.Build.props` anlegen: `net10.0`, `Nullable` enable, `ImplicitUsings` enable, `TreatWarningsAsErrors` true
- [X] T004 [P] `Directory.Packages.props` anlegen: `ManagePackageVersionsCentrally` true, `Microsoft.EntityFrameworkCore.SqlServer` 10.0.13, `Microsoft.EntityFrameworkCore.Design` 10.0.13, `MSTest` 4.5.1, keine Platzhalterversionen
- [X] T005 [P] Lokales Tool-Manifest `.config/dotnet-tools.json` mit `dotnet-ef` 10.0.13 anlegen
- [X] T006 Class Library `src/Kontor.Core/Kontor.Core.csproj` anlegen, ohne Paket- und Projektreferenzen
- [X] T007 Class Library `src/Kontor.Infrastructure/Kontor.Infrastructure.csproj` anlegen, mit Referenz auf `Kontor.Core` und den EF-Core-Paketen aus T004
- [X] T008 Blazor Web App `src/Kontor.Web/Kontor.Web.csproj` anlegen (`--interactivity Server --all-interactive`), mit Referenzen auf `Kontor.Core` und `Kontor.Infrastructure`
- [X] T009 Testprojekt `tests/Kontor.Core.Tests/Kontor.Core.Tests.csproj` als SDK-Projekt mit `PackageReference` auf `MSTest` (Version aus `Directory.Packages.props`) anlegen, mit Referenz auf `Kontor.Core`
- [X] T010 Alle Projekte in `Kontor.slnx` aufnehmen und `.gitignore` für .NET anlegen

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T011 [P] Entität `Employee` in `src/Kontor.Core/Domain/Employee.cs`: `Id`, `Name` (required, max. 100 Zeichen)
- [X] T012 [P] Entität `Project` in `src/Kontor.Core/Domain/Project.cs`: `Id`, `Key` (required, max. 20 Zeichen, eindeutig), `Name` (required, max. 200 Zeichen)
- [X] T013 [P] Entität `TimeEntry` in `src/Kontor.Core/Domain/TimeEntry.cs`: `Id`, `EmployeeId` (required), `ProjectId` (required), `Date` (`DateOnly`, required), `Start` und `End` (`TimeOnly`, required), `Note` (optional, max. 500 Zeichen); keine öffentlichen Setter (Prinzip III)
- [X] T014 [P] Schnittstellen `ITimeEntryRepository` (nur `AddAsync` und `GetForEmployeeAndDateAsync`), `IEmployeeRepository`, `IProjectRepository` in `src/Kontor.Core/Abstractions/`
- [X] T015 `KontorDbContext` und Entity-Konfigurationen mit den Constraints aus data-model.md und Index auf (`EmployeeId`, `Date`) in `src/Kontor.Infrastructure/KontorDbContext.cs` und `src/Kontor.Infrastructure/Configurations/`
- [X] T016 Seed-Daten per `HasData` in `src/Kontor.Infrastructure/Configurations/`: Mitarbeitende Ilka Brandt, Tomás Okoye, Juna Sato, Rik Halvorsen; Projekte `NSL-ALB-01` „Antriebswartung Albatros“, `KBA-TG3-04` „Schildgenerator-Kalibrierung Tiefgrund III“, `HFL-PER-02` „Lebenserhaltung prüfen Perihel“
- [X] T017 Repository-Implementierungen (nur Hinzufügen und Lesen) in `src/Kontor.Infrastructure/Repositories/`
- [X] T018 Registrierung `AddKontorInfrastructure(connectionString)` in `src/Kontor.Infrastructure/DependencyInjection.cs`
- [X] T019 `src/Kontor.Web/Program.cs`: Infrastruktur registrieren, Connection String `KontorDb` für LocalDB in `src/Kontor.Web/appsettings.Development.json`, ausstehende Migrationen in Development beim Start anwenden
- [X] T020 Erste Migration `InitialCreate` mit dem lokalen Tool erzeugen (`dotnet tool restore`, `dotnet ef migrations add InitialCreate`) in `src/Kontor.Infrastructure/Migrations/`
- [X] T021 Test-Double `InMemoryTimeEntryRepository` in `tests/Kontor.Core.Tests/Fakes/InMemoryTimeEntryRepository.cs`

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Arbeitszeit auf ein Projekt buchen (Priority: P1) 🎯 MVP

**Goal**: Eine Person bucht Arbeitszeit auf ein Projekt; Kontor rundet, verhindert Überschneidungen und zeigt die Tagesliste mit Tagessumme.

**Independent Test**: Person wählen, 08:07–12:53 auf `NSL-ALB-01` buchen → 08:00–13:00 in der Tagesliste, Tagessumme 5:00; 12:00–14:00 am selben Tag → abgelehnt wegen Überschneidung.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [X] T022 [P] [US1] `TimeRoundingTests` in `tests/Kontor.Core.Tests/Rules/TimeRoundingTests.cs`: 08:07 → 08:00, 12:53 → 13:00, 08:00 bleibt 08:00 (FR-004)
- [X] T023 [P] [US1] `MidnightRuleTests` in `tests/Kontor.Core.Tests/Rules/MidnightRuleTests.cs`: Ende vor oder gleich Beginn abgelehnt, Dauer 0 nach Rundung (08:05–08:10) abgelehnt (FR-005, FR-012)
- [X] T024 [P] [US1] `OverlapRuleTests` in `tests/Kontor.Core.Tests/Rules/OverlapRuleTests.cs`: Überschneidung abgelehnt, Berührung (Ende = Beginn) erlaubt, Buchungen anderer Personen zählen nicht (FR-006)
- [X] T025 [US1] `TimeEntryServiceTests` (User Story 1) in `tests/Kontor.Core.Tests/Services/TimeEntryServiceTests.cs`: die Akzeptanzszenarien 1–3 von User Story 1; bei Ablehnung ist die verletzte Regel benannt (FR-009); nutzt T021

### Implementation for User Story 1

- [X] T026 [P] [US1] `TimeRounding` in `src/Kontor.Core/Rules/TimeRounding.cs`: Beginn und Ende einzeln auf die nächste Viertelstunde (macht T022 grün)
- [X] T027 [P] [US1] `MidnightRule` in `src/Kontor.Core/Rules/MidnightRule.cs` (macht T023 grün)
- [X] T028 [P] [US1] `OverlapRule` in `src/Kontor.Core/Rules/OverlapRule.cs` (macht T024 grün)
- [X] T029 [US1] `BookingResult` und `TimeEntryService` in `src/Kontor.Core/Services/`: runden, `MidnightRule`, `OverlapRule`, speichern; Tagesliste und Tagessumme liefern (macht T025 grün; depends on T026–T028)
- [X] T030 [US1] `TimeEntryService` in `src/Kontor.Web/Program.cs` registrieren
- [X] T031 [US1] Seite `src/Kontor.Web/Components/Pages/Booking.razor`: Person aus Liste wählen, Datum, Beginn, Ende, Projekt, Notiz (optional, max. 500 Zeichen), Speichern; Hinweis mit verletzter Regel bei Ablehnung; Tagesliste mit Tagessumme; keine Bearbeiten- oder Löschen-Funktion (FR-001, FR-002, FR-003, FR-009, FR-010, FR-011)
- [X] T032 [US1] `Booking.razor` als Startseite in die Navigation aufnehmen, Beispielseiten der Vorlage (Counter, Weather) entfernen in `src/Kontor.Web/Components/`

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Arbeitszeitregeln bei der Erfassung einhalten (Priority: P2)

**Goal**: Kontor lehnt Buchungen ab, die die Mindestpause oder die Tageshöchstarbeitszeit verletzen würden, und nennt die Regel.

**Independent Test**: Rik Halvorsen bucht 07:00–14:00 → abgelehnt (Pause); 07:00–11:00 und 11:30–14:00 → beide gespeichert; Tag mit 9:00 Stunden plus 1:30 → abgelehnt (Höchstarbeitszeit).

### Tests for User Story 2 ⚠️

- [X] T033 [P] [US2] `BreakRuleTests` in `tests/Kontor.Core.Tests/Rules/BreakRuleTests.cs`: mehr als 6 h mit weniger als 30 min Pause abgelehnt, 07:00–11:00 und 11:30–14:00 erlaubt, Lücke unter 15 min zählt nicht, mehr als 9 h verlangt 45 min (FR-007)
- [X] T034 [P] [US2] `MaxDailyHoursRuleTests` in `tests/Kontor.Core.Tests/Rules/MaxDailyHoursRuleTests.cs`: mehr als 10 h abgelehnt, genau 10 h erlaubt (FR-008)
- [X] T035 [US2] `TimeEntryServiceTests` (User Story 2) in `tests/Kontor.Core.Tests/Services/TimeEntryServiceTests.cs`: die Akzeptanzszenarien von User Story 2; abgelehnt wird die neue Buchung, bestehende bleiben unverändert (FR-009)

### Implementation for User Story 2

- [X] T036 [P] [US2] `BreakRule` in `src/Kontor.Core/Rules/BreakRule.cs` (macht T033 grün)
- [X] T037 [P] [US2] `MaxDailyHoursRule` in `src/Kontor.Core/Rules/MaxDailyHoursRule.cs` (macht T034 grün)
- [X] T038 [US2] `BreakRule` und `MaxDailyHoursRule` nach der `OverlapRule` in `TimeEntryService` einbinden in `src/Kontor.Core/Services/TimeEntryService.cs` (macht T035 grün)

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [X] T039 [P] `README.md` im Wurzelverzeichnis: Zweck, Voraussetzungen, Verweis auf `specs/001-zeit-erfassen/quickstart.md`
- [X] T040 Prüfszenarien aus `specs/001-zeit-erfassen/quickstart.md` durchgehen

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: keine Abhängigkeiten
- **Foundational (Phase 2)**: hängt von Setup ab, blockiert alle User Stories
- **User Story 1 (Phase 3)**: hängt von Foundational ab
- **User Story 2 (Phase 4)**: hängt von Foundational ab; T038 erweitert den `TimeEntryService` aus T029
- **Polish (Phase 5)**: nach den gewünschten User Stories

### Within Each User Story

- Tests zuerst, sie müssen vor der Implementierung fehlschlagen
- Regeln vor Service, Service vor Seite

### Parallel Opportunities

- Setup: T003, T004, T005
- Foundational: T011, T012, T013, T014
- User Story 1: Tests T022, T023, T024; danach Regeln T026, T027, T028
- User Story 2: Tests T033, T034; danach Regeln T036, T037

---

## Parallel Example: User Story 1

```text
Task: "TimeRoundingTests in tests/Kontor.Core.Tests/Rules/TimeRoundingTests.cs"
Task: "MidnightRuleTests in tests/Kontor.Core.Tests/Rules/MidnightRuleTests.cs"
Task: "OverlapRuleTests in tests/Kontor.Core.Tests/Rules/OverlapRuleTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1: Setup
2. Phase 2: Foundational
3. Phase 3: User Story 1
4. **STOP and VALIDATE**: User Story 1 unabhängig prüfen (`dotnet test`, App starten, buchen)

### Incremental Delivery

1. Setup + Foundational → Grundlage steht
2. User Story 1 → prüfen → vorzeigbar (MVP)
3. User Story 2 → prüfen → Arbeitszeitregeln aktiv

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently

---

## Phase 6: Convergence

- [ ] T041 CRITICAL: Die Version von `dotnet-ef` (10.0.13) steht in `.config/dotnet-tools.json` statt in `Directory.Packages.props`; mit Prinzip V in Einklang bringen per Constitution V (contradicts)
- [ ] T042 Notizen über 500 Zeichen in `Kontor.Core` ablehnen (Test zuerst), statt sie erst an der Datenbank scheitern zu lassen, in `src/Kontor.Core/Services/TimeEntryService.cs` per data-model TimeEntry.Note (partial)
