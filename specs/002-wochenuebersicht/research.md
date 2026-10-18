# Research: Wochenübersicht

**Feature**: [spec.md](spec.md) | **Plan**: [plan.md](plan.md) | **Date**: 2026-10-18

Der Technical Context enthielt keine `NEEDS CLARIFICATION`. Die Entscheidungen aus Feature 001
(`specs/001-zeit-erfassen/research.md`, R1–R12) gelten weiter; hier stehen nur die neuen.

## R1 — Wochengrenzen

- **Decision**: Ein Wert `Week` in `Kontor.Core/Rules` mit `Monday` (`DateOnly`) und den sieben
  Tagen; `Week.Containing(date)` liefert die Woche von Montag bis Sonntag, `Previous` und `Next`
  verschieben um sieben Tage.
- **Rationale**: FR-008, Clarifications vom 2026-10-18; ohne Datenbank testbar (Prinzip II).
- **Alternatives considered**: `ISOWeek` aus `System.Globalization` für Kalenderwochen (für die
  Darstellung nicht nötig; die Woche wird über ihren Montag bestimmt).

## R2 — Lesen der Buchungen

- **Decision**: `WeekOverviewService` ruft die bestehende Methode
  `ITimeEntryRepository.GetForEmployeeAndDateAsync` für jeden der sieben Tage auf.
- **Rationale**: Planungsvorgabe „Lesen über die bestehenden Repository-Schnittstellen“;
  `Kontor.Infrastructure` und das Test-Double bleiben unverändert. Sieben kleine Abfragen pro
  Aufruf sind bei rund 40 Mitarbeitenden unkritisch.
- **Alternatives considered**: neue Methode für einen Zeitraum (eine Abfrage statt sieben, ändert
  aber die Schnittstelle und beide Implementierungen; erst sinnvoll, wenn Monatsabschluss oder
  Auswertungen größere Zeiträume brauchen).

## R3 — Summen

- **Decision**: Tagessumme, Wochensumme und Summe je Projekt sind Summen von
  `TimeEntry.RoundedDuration`. Die Tagessumme wird **an genau einer Stelle** gebildet:
  `DayOverview.From(entries)` in `Kontor.Core/Services`. `TimeEntryService.GetDayAsync` (Feature 001)
  und `WeekOverviewService` nutzen beide diese Methode (FR-007).
- **Correction** (2026-10-18, `/speckit-analyze` D1): Die erste Fassung bildete die Tagessumme im
  `WeekOverviewService` ein zweites Mal und sicherte die Übereinstimmung nur über einen Test.
- **Rationale**: FR-003, FR-004, FR-007, SC-002, SC-003. Weil jede gerundete Dauer ein Vielfaches
  von 15 Minuten ist, ist auch jede Summe eines; ein zweites Runden gibt es nicht.
- **Alternatives considered**: `GetDayAsync` siebenmal aufrufen (koppelt die Wochenübersicht an
  den Erfassungs-Service); eigene Summenbildung im `WeekOverviewService` (erste Fassung, zwei
  Stellen für dieselbe Regel, analyze D1).

## R4 — Projektnamen und Personenauswahl

- **Decision**: Projekte über `IProjectRepository.GetAllAsync` für Schlüssel und Bezeichnung der
  Summen je Projekt; Personen über `IEmployeeRepository.GetAllAsync` wie auf der Erfassungsseite.
  Summen je Projekt nur für Projekte mit Buchungen in der Woche, sortiert nach Projektschlüssel.
- **Rationale**: FR-001, FR-004; bestehende Schnittstellen.
- **Alternatives considered**: alle Projekte mit 0:00 anzeigen (bläht die Liste auf, ohne
  Information).

## R5 — Seite und Navigation

- **Decision**: `WeekOverview.razor` mit Route `/woche`, Auswahl der Person, Anzeige der
  aktuellen Woche beim Öffnen (FR-009), Schaltflächen „vorige Woche“ und „nächste Woche“; die
  gewählte Person bleibt im Komponentenzustand (FR-005). Link auf der Erfassungsseite und
  Eintrag im `NavMenu`.
- **Rationale**: FR-001, FR-005, FR-009, Assumption „von der Erfassungsseite erreichbar“.
- **Alternatives considered**: Person und Woche in der URL (nützlich für Lesezeichen, in Phase 1
  nicht gefordert; wie Feature 001, R9).
