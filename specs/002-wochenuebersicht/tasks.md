---

description: "Task list for feature 002 Wochenübersicht"
---

# Tasks: Wochenübersicht

**Input**: Design documents from `/specs/002-wochenuebersicht/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md

**Tests**: Enthalten. Die Constitution verlangt Test-First für alle Fachregeln in `Kontor.Core` (Prinzip II, NON-NEGOTIABLE); Test-Doubles werden selbst geschrieben.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2)
- Include exact file paths in descriptions

## Path Conventions

- Bestehende Struktur aus Feature 001: Projekte unter `src/`, Tests unter `tests/` (siehe plan.md, Project Structure)

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Ausgangslage prüfen: `dotnet build` und `dotnet test` auf dem Stand von Feature 001 grün; keine neuen Projekte und keine neuen Pakete (plan.md, Structure Decision)

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [ ] T002 `WeekTests` in `tests/Kontor.Core.Tests/Rules/WeekTests.cs`: `Containing` liefert für Montag 2026-11-02, Mittwoch 2026-11-04 und Sonntag 2026-11-08 die Woche mit `Monday` 2026-11-02 und `Sunday` 2026-11-08; `Days` hat sieben Tage; `Next` beginnt am 2026-11-09, `Previous` am 2026-10-26 (FR-005, FR-008)
- [ ] T003 `Week` in `src/Kontor.Core/Rules/Week.cs`: `Monday`, `Sunday`, `Days`, `Containing(DateOnly)`, `Previous`, `Next` (macht T002 grün)

**Checkpoint**: Foundation ready - user story implementation can now begin

---

## Phase 3: User Story 1 - Die Woche einer Person ansehen (Priority: P1) 🎯 MVP

**Goal**: Für eine gewählte Person die Buchungen einer Woche, je Tag mit Tagessumme, und die Wochensumme anzeigen; nur lesen.

**Independent Test**: Juna Sato: 2026-11-02 08:07–12:53 und 13:30–16:00, 2026-11-03 07:00–11:00 → Montag 4:45 und 2:30, Tagessumme 7:15; Dienstag 4:00; übrige Tage leer; Wochensumme 11:15.

### Tests for User Story 1 ⚠️

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T004 [US1] `WeekOverviewServiceTests` in `tests/Kontor.Core.Tests/Services/WeekOverviewServiceTests.cs`: Akzeptanzszenario 1 von User Story 1 (Tage, gerundete Dauern, Tagessummen 7:15 und 4:00, Wochensumme 11:15); Szenario 2 (Woche ohne Buchung: sieben leere Tage, Wochensumme 0:00); Buchungen anderer Personen und anderer Wochen zählen nicht; Tagessumme gleich `TimeEntryService.GetDayAsync` (FR-002, FR-003, FR-007); nutzt `InMemoryTimeEntryRepository`

### Implementation for User Story 1

- [ ] T005 [P] [US1] `WeekOverview` in `src/Kontor.Core/Services/WeekOverview.cs`: `Week`, `Days` (sieben `DayOverview`), `Total` (data-model.md)
- [ ] T006 [US1] `WeekOverviewService` in `src/Kontor.Core/Services/WeekOverviewService.cs`: liest `ITimeEntryRepository.GetForEmployeeAndDateAsync` für jeden Tag der Woche, Tagessummen und Wochensumme aus `RoundedDuration` (research.md R2, R3; macht T004 grün; depends on T003, T005)
- [ ] T007 [US1] `WeekOverviewService` in `src/Kontor.Web/Program.cs` registrieren
- [ ] T008 [US1] Seite `src/Kontor.Web/Components/Pages/WeekOverview.razor`, Route `/woche`: Person aus Liste wählen, beim Öffnen die aktuelle Woche, je Tag Datum und Wochentag, Buchungen mit erfassten Uhrzeiten, gerundeter Dauer, Projekt und Notiz, Tagessumme, Wochensumme; keine Funktion zum Anlegen, Ändern oder Löschen (FR-001, FR-002, FR-003, FR-006, FR-009)
- [ ] T009 [US1] Navigation: Eintrag „Wochenübersicht“ in `src/Kontor.Web/Components/Layout/NavMenu.razor` und Link auf der Erfassungsseite `src/Kontor.Web/Components/Pages/Booking.razor` (Assumption „von der Erfassungsseite erreichbar“)

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Summe je Projekt (Priority: P2)

**Goal**: Die Wochenübersicht zeigt je Projekt die Summe der gerundeten Dauern der Woche.

**Independent Test**: Buchungen von Juna Sato aus User Story 1 → `KBA-TG3-04` 2:30, `NSL-ALB-01` 8:45; zusammen 11:15.

### Tests for User Story 2 ⚠️

- [ ] T010 [US2] `WeekOverviewServiceTests` (User Story 2) in `tests/Kontor.Core.Tests/Services/WeekOverviewServiceTests.cs`: Summen je Projekt nur für Projekte mit Buchungen, sortiert nach Schlüssel, mit Bezeichnung; Summe der Projektsummen = Wochensumme (FR-004, SC-003); Test-Double `InMemoryProjectRepository` in `tests/Kontor.Core.Tests/Fakes/InMemoryProjectRepository.cs`

### Implementation for User Story 2

- [ ] T011 [P] [US2] `ProjectTotal` in `src/Kontor.Core/Services/ProjectTotal.cs`: `ProjectKey`, `ProjectName`, `Total` (data-model.md)
- [ ] T012 [US2] `ProjectTotals` in `WeekOverview` ergänzen und im `WeekOverviewService` über `IProjectRepository.GetAllAsync` bilden in `src/Kontor.Core/Services/` (research.md R4; macht T010 grün)
- [ ] T013 [US2] Summen je Projekt in `src/Kontor.Web/Components/Pages/WeekOverview.razor` anzeigen

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - Zwischen Wochen blättern (Priority: P3)

**Goal**: Zur vorigen und nächsten Woche wechseln; die gewählte Person bleibt erhalten.

**Independent Test**: Woche vom 2026-11-02 → nächste Woche zeigt 2026-11-09 bis 2026-11-15, Juna Sato bleibt gewählt; zurück → wieder die Woche vom 2026-11-02.

### Tests for User Story 3 ⚠️

- [ ] T014 [US3] Blättern in `tests/Kontor.Core.Tests/Rules/WeekTests.cs` absichern: `Next` und `Previous` über einen Jahreswechsel (Woche vom 2026-12-28 → 2027-01-04) und hin und zurück wieder dieselbe Woche (FR-005)

### Implementation for User Story 3

- [ ] T015 [US3] Schaltflächen „vorige Woche“ und „nächste Woche“ in `src/Kontor.Web/Components/Pages/WeekOverview.razor`; die gewählte Person bleibt im Komponentenzustand (FR-005; macht T014 nutzbar)

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T016 [P] `README.md`: Wochenübersicht in der Kurzbeschreibung ergänzen, Verweis auf `specs/002-wochenuebersicht/quickstart.md`
- [ ] T017 Prüfszenarien aus `specs/002-wochenuebersicht/quickstart.md` durchgehen

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: keine Abhängigkeiten
- **Foundational (Phase 2)**: hängt von Setup ab, blockiert alle User Stories
- **User Story 1 (Phase 3)**: hängt von Foundational ab
- **User Story 2 (Phase 4)**: hängt von User Story 1 ab (erweitert `WeekOverview` und den Service)
- **User Story 3 (Phase 5)**: hängt von Foundational ab; T015 erweitert die Seite aus T008
- **Polish (Phase 6)**: nach den gewünschten User Stories

### Within Each User Story

- Tests zuerst, sie müssen vor der Implementierung fehlschlagen
- Modell vor Service, Service vor Seite

### Parallel Opportunities

- User Story 1: T005 parallel zu T004
- User Story 2: T011 parallel zu T010
- Polish: T016

---

## Parallel Example: User Story 1

```text
Task: "WeekOverviewServiceTests in tests/Kontor.Core.Tests/Services/WeekOverviewServiceTests.cs"
Task: "WeekOverview in src/Kontor.Core/Services/WeekOverview.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Phase 1: Setup
2. Phase 2: Foundational
3. Phase 3: User Story 1
4. **STOP and VALIDATE**: User Story 1 unabhängig prüfen (`dotnet test`, App starten, Woche ansehen)

### Incremental Delivery

1. Setup + Foundational → Woche berechnet
2. User Story 1 → prüfen → vorzeigbar (MVP)
3. User Story 2 → Summen je Projekt
4. User Story 3 → Blättern

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Verify tests fail before implementing
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
