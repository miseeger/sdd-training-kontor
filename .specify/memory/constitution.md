# Kontor Constitution

Kontor ist die Zeiterfassung der Galactic Service Company (GSC).

## Core Principles

### I. Fachlogik-First

Alle fachlichen Regeln (Rundung, Pausen, Überlappung, Höchstarbeitszeit) MÜSSEN
ausschließlich in `Kontor.Core` implementiert sein. Oberfläche und Persistenz DÜRFEN
KEINE Fachlogik enthalten; sie orchestrieren und stellen dar.

*Begründung:* Regeln an genau einer Stelle lassen sich ohne Oberfläche und ohne
Datenbank prüfen und ändern.

### II. Test-First für Fachlogik (NON-NEGOTIABLE)

Jede Regel in `Kontor.Core` MUSS MSTest-Tests haben, die vor der Implementierung
geschrieben werden. Mock-Bibliotheken DÜRFEN NICHT verwendet werden; Test-Doubles
werden selbst geschrieben.

*Begründung:* Die Tests beschreiben die Regel, bevor es Code gibt, und bleiben ohne
fremde Bibliotheken lesbar.

### III. Nachvollziehbarkeit

Eine gespeicherte Buchung DARF weder geändert noch gelöscht werden. Korrekturen
MÜSSEN als Gegenbuchung erfasst werden.

*Begründung:* Jede erfasste Arbeitszeit bleibt nachvollziehbar, auch nach einer
Korrektur.

### IV. Englische Bezeichner

Namespaces, Typen, Methoden und Tests MÜSSEN englisch benannt sein, auch wenn der
Fachbegriff deutsch ist.

*Begründung:* Einheitlicher Teamstandard für den gesamten Code.

### V. Feste Paketversionen

Versionen von NuGet-Paketen MÜSSEN ausschließlich zentral in `Directory.Packages.props`
stehen. Ausgenommen sind die SDK-Version in `global.json` und die Versionen lokaler
.NET-Tools im Tool-Manifest `.config/dotnet-tools.json`; auch dort MÜSSEN feste Versionen
stehen. Platzhalter wie `10.*` DÜRFEN an keiner dieser Stellen verwendet werden.

*Begründung:* Jeder Build verwendet dieselben Pakete, Aktualisierungen sind bewusste
Änderungen an einer Stelle. Central Package Management erfasst weder das SDK noch lokale
Tools; `/speckit-analyze` und `/speckit-converge` meldeten das in Feature 001 als CRITICAL
(`MSTest.Sdk`, `dotnet-ef`), obwohl es keine andere Ablage gibt (Änderung 1.1.0).

## Technische Rahmenbedingungen

- Zielplattform: .NET 10 (LTS)
- Oberfläche: Blazor Web App mit Render-Modus `InteractiveServer`
- Persistenz: EF Core mit SQL Server (lokal LocalDB)
- Tests: MSTest

## Governance

- Diese Constitution hat Vorrang vor anderen Konventionen im Repository.
- Abgeschlossene Feature-Verzeichnisse unter `specs/` werden nicht mehr geändert.
  Folgeänderungen bekommen ein neues Verzeichnis mit Querverweis auf das ursprüngliche
  Feature.
- Änderungen an dieser Constitution erfolgen über `/speckit-constitution`. Die Version
  folgt Semantic Versioning: MAJOR für entfernte oder neu definierte Prinzipien, MINOR
  für neue Prinzipien oder Abschnitte, PATCH für Klarstellungen.
- Die Einhaltung wird im Constitution Check jedes Plans und durch `/speckit-analyze`
  geprüft.

**Version**: 1.1.0 | **Ratified**: 2026-10-17 | **Last Amended**: 2026-10-18
