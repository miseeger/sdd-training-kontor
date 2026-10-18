# Quickstart: Wochenübersicht

**Feature**: [spec.md](spec.md) | **Date**: 2026-10-18

Voraussetzungen, Bauen, Testen und Starten wie in [Feature 001](../001-zeit-erfassen/quickstart.md).

## Prüfszenarien

Vorbereitung auf der Erfassungsseite: Juna Sato bucht am 2026-11-02 08:07–12:53 auf `NSL-ALB-01`
und 13:30–16:00 auf `KBA-TG3-04`, am 2026-11-03 07:00–11:00 auf `NSL-ALB-01`.

| # | Schritt | Erwartet | Bezug |
|---|---|---|---|
| 1 | Wochenübersicht öffnen | aktuelle Woche, Montag bis Sonntag | FR-008, FR-009 |
| 2 | Juna Sato wählen, zur Woche vom 2026-11-02 blättern | Montag: zwei Buchungen, 4:45 und 2:30, Tagessumme 7:15; Dienstag: 4:00; übrige Tage leer; Wochensumme 11:15 | US1, FR-002, FR-003 |
| 3 | Summen je Projekt ansehen | `KBA-TG3-04` 2:30, `NSL-ALB-01` 8:45 | US2, FR-004 |
| 4 | Nächste Woche | 2026-11-09 bis 2026-11-15, Juna Sato bleibt gewählt, Wochensumme 0:00 | US3, FR-005 |
| 5 | Seite ansehen | keine Funktion zum Anlegen, Ändern oder Löschen | FR-006 |
| 6 | Erfassungsseite, 2026-11-02 | Tagessumme 7:15 wie in der Übersicht | FR-007 |
