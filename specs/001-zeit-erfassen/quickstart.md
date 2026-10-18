# Quickstart: Zeit erfassen

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-17, aktualisiert 2026-10-18 (Rundung der Dauer)

## Voraussetzungen

- .NET 10 SDK
- SQL Server LocalDB (`sqllocaldb info` zeigt `MSSQLLocalDB`)
- PowerShell 7

## Bauen und testen

```powershell
dotnet tool restore
dotnet build
dotnet test
```

Erwartet: Build ohne Fehler, alle Tests in `tests/Kontor.Core.Tests` grün.

## Starten

```powershell
dotnet run --project src/Kontor.Web
```

Beim ersten Start in Development legt die App die Datenbank `KontorDb` in LocalDB an
(Migrationen, siehe [research.md](research.md), R6). Die Adresse steht in der Konsolenausgabe.

## Prüfszenarien

| # | Schritt | Erwartet | Bezug |
|---|---|---|---|
| 1 | Juna Sato wählen, 08:07–12:53 auf `NSL-ALB-01` buchen | gespeichert mit 08:07–12:53, Dauer 4:45, Tagessumme 4:45 | US1, FR-004, FR-014 |
| 2 | Juna Sato, gleicher Tag, 12:00–14:00 buchen | abgelehnt, Überschneidung genannt | FR-006, FR-009 |
| 3 | Rik Halvorsen, neuer Tag, 07:00–14:00 buchen | abgelehnt, Pausenregel genannt | US2, FR-007 |
| 4 | Rik Halvorsen, gleicher Tag, 07:00–11:00 und 11:30–14:00 buchen | beide gespeichert | US2 |
| 5 | Eine Buchung 22:00–00:30 versuchen | abgelehnt (Mitternacht) | FR-012 |
| 6 | Tagesliste ansehen | keine Bearbeiten- oder Löschen-Funktion | FR-010 |
| 7 | Tomás Okoye, neuer Tag, 08:05–08:10 buchen | abgelehnt, gerundete Dauer 0 | FR-005 |
| 8 | Tomás Okoye, gleicher Tag, 08:05–08:13 buchen | gespeichert, Dauer 0:15 | FR-004 |

Die Datenmodell-Details stehen in [data-model.md](data-model.md).
