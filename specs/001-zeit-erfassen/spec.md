# Feature Specification: Zeit erfassen

**Feature Branch**: `001-zeit-erfassen`

**Created**: 2026-10-17

**Status**: Draft

**Input**: User description: "Kontor ist die neue Zeiterfassung der Galactic Service Company (GSC), eines Wartungsdienstleisters mit rund 40 Technikerinnen und Technikern. Heute führen alle ihre Stunden in Tabellen, und die Produktverantwortliche tippt am Monatsende alles ab. Als Technikerin möchte ich meine Arbeitszeit auf das Projekt buchen, an dem ich gearbeitet habe, damit am Monatsende niemand meine Stunden abtippen muss. Eine Buchung hat Datum, Beginn, Ende, Projekt und eine optionale Notiz. Nach dem Speichern sehe ich meine Buchungen des Tages und die Tagessumme. Fachliche Regeln: Zeiten werden kaufmännisch auf 15 Minuten gerundet. Buchungen derselben Person dürfen sich zeitlich nicht überlappen. Es gilt eine Mindestpause nach gesetzlicher Vorgabe. Es gilt eine Tageshöchstarbeitszeit. Eine gespeicherte Buchung wird weder geändert noch gelöscht. Nicht Teil dieses Features: Wochenübersicht, Monatsabschluss und Korrektur von Buchungen. In dieser Phase gibt es keine Anmeldung; Mitarbeitende und Projekte sind fest hinterlegt. Projekte kommen später aus der Projektverwaltung Dockyard."

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Arbeitszeit auf ein Projekt buchen (Priority: P1)

Eine Technikerin wählt sich aus der Liste der Mitarbeitenden aus und bucht für einen Tag
Beginn und Ende ihrer Arbeit auf ein Projekt, optional mit einer Notiz. Kontor rundet die
Zeiten, prüft, dass sich die Buchung nicht mit einer anderen Buchung derselben Person
überschneidet, und zeigt nach dem Speichern alle Buchungen dieses Tages mit der Tagessumme.

**Why this priority**: Ohne Buchen hat Kontor keinen Wert. Diese Story allein ersetzt
bereits die Tabelle: Die Stunden liegen erfasst vor und müssen nicht mehr abgetippt werden.

**Independent Test**: Eine Person auswählen, eine Buchung anlegen und prüfen, dass sie
gerundet in der Tagesliste erscheint und die Tagessumme stimmt; eine überlappende zweite
Buchung anlegen und prüfen, dass sie abgelehnt wird.

**Acceptance Scenarios**:

1. **Given** Juna Sato hat am 2026-11-02 noch keine Buchung, **When** sie 08:07–12:53 auf
   Projekt `NSL-ALB-01` bucht, **Then** wird die Buchung als 08:00–13:00 gespeichert und
   die Tagessumme zeigt 5:00 Stunden.
2. **Given** Juna Sato hat am 2026-11-02 die Buchung 08:00–13:00, **When** sie
   12:00–14:00 auf Projekt `KBA-TG3-04` bucht, **Then** wird die Buchung abgelehnt mit dem
   Hinweis, dass sie sich mit 08:00–13:00 überschneidet.
3. **Given** Juna Sato hat am 2026-11-02 die Buchung 08:00–13:00, **When** sie
   13:00–15:00 auf Projekt `KBA-TG3-04` bucht, **Then** wird die Buchung gespeichert
   (Ende und Beginn berühren sich, sie überschneiden sich nicht).
4. **Given** eine gespeicherte Buchung, **When** Juna Sato die Tagesliste öffnet,
   **Then** gibt es keine Möglichkeit, die Buchung zu ändern oder zu löschen.

---

### User Story 2 - Arbeitszeitregeln bei der Erfassung einhalten (Priority: P2)

Beim Speichern prüft Kontor zusätzlich die gesetzlichen Arbeitszeitregeln für den ganzen
Tag der Person: die Mindestpause und die Tageshöchstarbeitszeit. Eine Buchung, die eine
Regel verletzen würde, wird nicht gespeichert, und Kontor sagt, welche Regel verletzt ist.

**Why this priority**: Die Regeln schützen die Mitarbeitenden und die GSC. Story 1 ist auch
ohne sie nutzbar, aber erst mit ihnen sind die erfassten Zeiten verlässlich.

**Independent Test**: Für eine Person nacheinander Buchungen anlegen, bis eine Pause fehlt
oder die Höchstarbeitszeit überschritten würde, und prüfen, dass genau diese Buchung mit
Nennung der Regel abgelehnt wird.

**Acceptance Scenarios**:

1. **Given** Rik Halvorsen hat am 2026-11-03 keine Buchung, **When** er 07:00–14:00 auf
   Projekt `HFL-PER-02` bucht, **Then** wird die Buchung abgelehnt, weil bei mehr als
   6 Stunden Arbeitszeit eine Pause von mindestens 30 Minuten fehlt.
2. **Given** Rik Halvorsen hat am 2026-11-03 die Buchung 07:00–11:00, **When** er
   11:30–14:00 bucht, **Then** wird die Buchung gespeichert (6:30 Stunden Arbeitszeit,
   30 Minuten Pause).
3. **Given** Rik Halvorsen hat am 2026-11-03 Buchungen über insgesamt 9:00 Stunden mit
   ausreichender Pause, **When** er eine weitere Buchung über 1:30 Stunden anlegt,
   **Then** wird sie abgelehnt, weil die Tageshöchstarbeitszeit von 10 Stunden
   überschritten würde.

---

### Edge Cases

- Was passiert, wenn das Ende vor oder gleich dem Beginn liegt? → Die Buchung wird
  abgelehnt.
- Was passiert, wenn eine Buchung durch die Rundung die Dauer 0 bekommt (z. B. 08:05–08:10)?
  → Die Buchung wird abgelehnt, weil nach der Rundung das Ende nicht nach dem Beginn liegt.
- Was passiert, wenn sich zwei Buchungen nur an einer Grenze berühren (Ende = Beginn)?
  → Das ist keine Überschneidung.
- Was passiert bei einer Buchung über Mitternacht?
- Was passiert, wenn eine Person für einen Tag in der Zukunft bucht?

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Mitarbeitende MÜSSEN eine Buchung mit Datum, Beginn, Ende, Projekt und
  optionaler Notiz erfassen können.
- **FR-002**: Das System MUSS die Person, für die gebucht wird, aus einer fest hinterlegten
  Liste der Mitarbeitenden auswählen lassen; eine Anmeldung gibt es nicht.
- **FR-003**: Das System MUSS als Projekt nur fest hinterlegte Projekte zur Auswahl anbieten.
- **FR-004**: Das System MUSS Beginn und Ende einer Buchung kaufmännisch auf 15 Minuten
  runden, bevor die Buchung geprüft und gespeichert wird.
- **FR-005**: Das System MUSS eine Buchung ablehnen, deren gerundetes Ende nicht nach dem
  gerundeten Beginn liegt.
- **FR-006**: Das System MUSS eine Buchung ablehnen, die sich zeitlich mit einer bestehenden
  Buchung derselben Person überschneidet. Berühren sich zwei Buchungen nur (Ende = Beginn),
  ist das keine Überschneidung.
- **FR-007**: Das System MUSS eine Buchung ablehnen, wenn die Tagesarbeitszeit der Person
  danach mehr als 6 Stunden beträgt und die Pause weniger als 30 Minuten, oder mehr als
  9 Stunden und die Pause weniger als 45 Minuten.
- **FR-008**: Das System MUSS eine Buchung ablehnen, wenn die Tagesarbeitszeit der Person
  danach mehr als 10 Stunden beträgt.
- **FR-009**: Bei jeder Ablehnung MUSS das System die verletzte Regel benennen.
- **FR-010**: Gespeicherte Buchungen DÜRFEN weder geändert noch gelöscht werden können.
- **FR-011**: Nach dem Speichern MUSS das System alle Buchungen der Person für diesen Tag
  und die Tagessumme anzeigen.

### Key Entities *(include if feature involves data)*

- **Mitarbeitende**: eine Person, die Arbeitszeit bucht; Name. In dieser Phase fest
  hinterlegt.
- **Projekt**: ein Wartungsauftrag, auf den gebucht wird; Projektschlüssel und Bezeichnung.
  In dieser Phase fest hinterlegt, später aus Dockyard.
- **Buchung**: gehört zu genau einer Person und einem Projekt; Datum, Beginn, Ende (beide
  gerundet), optionale Notiz. Nach dem Speichern unveränderlich.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Eine Technikerin erfasst eine Buchung in unter 30 Sekunden.
- **SC-002**: 100 % der Buchungen, die eine Regel verletzen (Überschneidung, Pause,
  Höchstarbeitszeit), werden bei der Erfassung abgelehnt.
- **SC-003**: Die angezeigte Tagessumme entspricht in 100 % der Fälle der Summe der
  gerundeten Buchungen des Tages.
- **SC-004**: Keine gespeicherte Buchung lässt sich nachträglich ändern oder löschen.

## Assumptions

- Alle Mitarbeitenden arbeiten in derselben Ortszeit; Zeitzonen spielen keine Rolle.
- Beginn und Ende werden jeweils einzeln gerundet; die Dauer ergibt sich aus den gerundeten
  Zeiten.
- Als Pause gilt die Zeit zwischen zwei Buchungen desselben Tages.
- Eine Notiz ist freier Text mit höchstens 500 Zeichen.
- Mitarbeitende und Projekte werden in dieser Phase nicht über die Oberfläche gepflegt.
- Korrekturen falscher Buchungen sind nicht Teil dieses Features; bis dahin bleibt eine
  gespeicherte Buchung, wie sie ist.
