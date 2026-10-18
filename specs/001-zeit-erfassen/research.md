# Research: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Plan**: [plan.md](plan.md) | **Date**: 2026-10-17, aktualisiert 2026-10-18 (R2, R3, R4 nach Clarifications zur Rundung; R10–R12 neu)

Der Technical Context enthielt keine `NEEDS CLARIFICATION`. Die folgenden Entscheidungen
klären Abhängigkeiten und Muster.

## R1 — Datentypen für Datum und Uhrzeit

- **Decision**: `DateOnly` für das Datum, `TimeOnly` für Beginn und Ende.
- **Rationale**: Eine Buchung liegt laut Spec innerhalb eines Tages (FR-012), Zeitzonen spielen
  keine Rolle (Assumptions). EF Core 10 bildet `DateOnly` und `TimeOnly` mit SQL Server auf
  `date` und `time` ab.
- **Alternatives considered**: `DateTime` für Beginn und Ende (Mitternacht und Datum wären
  doppelt ausdrückbar); `DateTimeOffset` (Zeitzonen sind nicht gefordert).

## R2 — Rundung

- **Decision**: Beginn und Ende werden unverändert gespeichert. Gerundet wird nur die Dauer
  einer Buchung, kaufmännisch auf 15 Minuten (Rest unter 8 Minuten ab, ab 8 Minuten auf), als
  berechneter Wert von `TimeEntry` (`RoundedDuration`), nicht als eigene Spalte. Eine
  gerundete Dauer von 0 führt zur Ablehnung (FR-005).
- **Rationale**: FR-004, FR-014 und Clarifications vom 2026-10-18. Ein berechneter Wert braucht
  keine Schemaänderung und keine Migration; bestehende Buchungen mit gerundeten Uhrzeiten
  haben schon eine Dauer in Vielfachen von 15 Minuten und ändern sich nicht.
- **Alternatives considered**: Beginn und Ende einzeln runden (bisherige Entscheidung nach der
  Annahme der Spec; von der Fachseite am 2026-10-18 verworfen); gerundete Dauer als Spalte
  speichern (Migration und Befüllen bestehender Zeilen, zweite Wahrheit neben Beginn und Ende);
  nur die Tagessumme runden (in Clarifications verworfen).

## R3 — Wo die Regeln geprüft werden

- **Decision**: `TimeEntryService` in `Kontor.Core` lädt die Buchungen der Person für den Tag
  über `ITimeEntryRepository` und prüft die neue Buchung mit den erfassten Zeiten nacheinander mit
  `MidnightRule`, `OverlapRule`, `BreakRule` und `MaxDailyHoursRule`. Das Ergebnis ist ein
  `BookingResult` (gespeichert oder abgelehnt mit verletzter Regel, FR-009).
- **Rationale**: Prinzip I und II: Alle Regeln sind reine Funktionen über eine Liste von
  Zeitspannen und ohne Datenbank testbar.
- **Alternatives considered**: Prüfung in der Blazor-Komponente oder in Datenbank-Constraints
  (verstößt gegen Prinzip I).

## R4 — Pause und Tagesarbeitszeit

- **Decision**: Tagesarbeitszeit = Summe der **erfassten, ungerundeten** Dauern aller Buchungen
  des Tages einschließlich der neuen (FR-013). Pause = Summe der Lücken zwischen zeitlich aufeinanderfolgenden Buchungen, wobei nur
  Lücken ab 15 Minuten zählen. Mehr als 6 Stunden verlangen mindestens 30 Minuten, mehr als
  9 Stunden mindestens 45 Minuten Pause; mehr als 10 Stunden sind nie erlaubt.
- **Rationale**: FR-007, FR-008, FR-013 und Clarifications vom 2026-10-17 und 2026-10-18.
  Die gerundete Tagessumme dient der Abrechnung (FR-014), nicht den Arbeitszeitregeln.
- **Alternatives considered**: Pause als eigene Buchungsart (in Clarifications verworfen).

## R5 — Unveränderlichkeit

- **Decision**: `ITimeEntryRepository` bietet nur `AddAsync` und Lesemethoden; `TimeEntry` hat
  keine öffentlichen Setter. Die Oberfläche hat keine Bearbeiten- oder Löschen-Funktion.
- **Rationale**: Prinzip III, FR-010.
- **Alternatives considered**: Soft Delete (wäre eine Löschung; Korrekturen kommen später als
  Gegenbuchung).

## R6 — Datenbankschema und Migrationen

- **Decision**: EF-Core-Migrationen in `Kontor.Infrastructure`, erzeugt mit `dotnet-ef` als
  lokalem Tool (`.config/dotnet-tools.json`, `dotnet tool restore`). In der Umgebung
  Development wendet `Kontor.Web` ausstehende Migrationen beim Start an.
- **Rationale**: Niemand braucht eine globale Installation; die Datenbank entsteht beim ersten
  Start.
- **Alternatives considered**: `EnsureCreated` (verträgt keine späteren Schemaänderungen);
  globales `dotnet-ef` (zusätzliche Installation auf jedem Rechner).

## R7 — Stammdaten

- **Decision**: Seed über `HasData` in den Entity-Konfigurationen: Mitarbeitende Ilka Brandt,
  Tomás Okoye, Juna Sato, Rik Halvorsen; Projekte `NSL-ALB-01` „Antriebswartung Albatros“,
  `KBA-TG3-04` „Schildgenerator-Kalibrierung Tiefgrund III“, `HFL-PER-02` „Lebenserhaltung
  prüfen Perihel“.
- **Rationale**: Laut Spec fest hinterlegt; über Migrationen reproduzierbar.
- **Alternatives considered**: Seed im Startcode (läuft bei jedem Start, schwerer zu prüfen).

## R8 — Paketversionen und Testprojekt

- **Decision**: `Directory.Packages.props` mit `ManagePackageVersionsCentrally` und festen
  Versionen: EF Core 10.0.13 und `MSTest` 4.5.1. Das Testprojekt ist ein normales SDK-Projekt
  mit `PackageReference` auf `MSTest`. `global.json` legt nur die SDK-Version fest.
- **Rationale**: Prinzip V verlangt alle Paketversionen ausschließlich in
  `Directory.Packages.props`. Das Projekt-SDK `MSTest.Sdk` ließe sich nur über `global.json`
  versionieren (Befund C1 aus `/speckit-analyze`). Mit dem Paket `MSTest` gilt Prinzip V
  ohne Ausnahme.
- **Alternatives considered**: `MSTest.Sdk` mit Version in `global.json` (verstößt gegen
  Prinzip V); Prinzip V ändern (eigener Constitution-Schritt, nicht mitten im Feature).

## R9 — Blazor-Render-Modus und Auswahl der Person

- **Decision**: Blazor Web App mit `InteractiveServer` global (`--interactivity Server
  --all-interactive`). Die Person wird auf der Seite aus einer Auswahlliste gewählt und bleibt
  für die Sitzung im Komponentenzustand.
- **Rationale**: Vorgabe der Constitution; ohne Anmeldung (FR-002).
- **Alternatives considered**: Person in der URL (unnötig für Phase 1).

## R10 — DbContext in Blazor Server

- **Decision**: `AddDbContextFactory<KontorDbContext>` in `Kontor.Infrastructure`; die
  Repositories erzeugen pro Operation einen eigenen Kontext über `IDbContextFactory`.
- **Rationale**: Eine Blazor-Server-Verbindung lebt lange; ein gemeinsamer `DbContext` führt bei
  parallelen Aufrufen (Tagesliste laden und Speichern) zu „A second operation was started on
  this context instance“. Beim Implementieren von Feature 001 entschieden, bisher nur im Code.
- **Alternatives considered**: `AddDbContext` mit Scoped-Lebensdauer (siehe Fehler oben).

## R11 — NuGet-Quellen

- **Decision**: `nuget.config` im Repository mit `<clear />` und nur nuget.org.
- **Rationale**: Mit Central Package Management und mehr als einer Paketquelle meldet NuGet
  `NU1507`; mit `TreatWarningsAsErrors` scheitert der Restore. Beim Implementieren von
  Feature 001 entschieden, bisher nur im Code.
- **Alternatives considered**: Package Source Mapping (mehr Konfiguration ohne Nutzen, solange
  nur nuget.org gebraucht wird).

## R12 — Transitive Pinning

- **Decision**: `CentralPackageTransitivePinningEnabled` in `Directory.Packages.props` und eine
  feste Version für `System.Security.Cryptography.Pkcs` (10.0.13).
- **Rationale**: EF Core SqlServer 10.0.13 zieht `System.Security.Cryptography.Pkcs` 9.0.11 mit
  bekannten Sicherheitslücken nach (`NU1902`, `NU1903`), der Build bricht mit
  `TreatWarningsAsErrors`. Beim Implementieren von Feature 001 entschieden, bisher nur im Code.
- **Alternatives considered**: Warnung unterdrücken (verdeckt eine echte Sicherheitslücke).
