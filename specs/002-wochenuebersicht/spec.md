# Feature Specification: Wochenübersicht

**Feature Branch**: `002-wochenuebersicht`

**Created**: 2026-10-18

**Status**: Draft

**Input**: User description: "Wochenübersicht für Kontor, die Zeiterfassung der Galactic Service Company (GSC). Als Schichtleiter möchte ich sehen, was eine Person in einer Woche gebucht hat, damit ich die Woche nachvollziehen kann, ohne jeden Tag einzeln aufzurufen. Die Übersicht zeigt die Buchungen einer Person für eine Woche, je Tag mit Tagessumme, dazu die Wochensumme und die Summe je Projekt. Sie ist nur zum Lesen. Nicht Teil dieses Features: Übersicht für mehrere Personen oder Schichten, Freigabe von Wochen, Warnungen zur Wochenarbeitszeit, Export. Es gibt weiterhin keine Anmeldung. Grundlage sind die Buchungen aus Feature 001 mit der dort geklärten Rundung."

## Clarifications

### Session 2026-10-18

- Q: An welchem Tag beginnt eine Woche in der Übersicht? → A: Montag; die Woche reicht von Montag bis Sonntag.
- Q: Welche Woche zeigt die Übersicht, wenn sie geöffnet wird? → A: Die aktuelle Woche.
- Q: Werden Tage ohne Buchung in der Übersicht angezeigt? → A: Ja, alle sieben Tage erscheinen, Tage ohne Buchung leer.
- Q: Bleibt die gewählte Person erhalten, wenn zur vorigen oder nächsten Woche gewechselt wird? → A: Ja; beim Blättern wechselt nur die Woche.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Die Woche einer Person ansehen (Priority: P1)

Ein Schichtleiter wählt eine Person aus der Liste der Mitarbeitenden und sieht ihre Buchungen
einer Woche, Tag für Tag, jeweils mit der Tagessumme, und darunter die Wochensumme.

**Why this priority**: Das ist der Kern des Wunsches: die Woche nachvollziehen, ohne jeden Tag
einzeln aufzurufen. Diese Story allein liefert bereits den Nutzen.

**Independent Test**: Für eine Person Buchungen an zwei Tagen einer Woche anlegen, die
Wochenübersicht öffnen und prüfen, dass beide Tage mit ihren Buchungen und Tagessummen
erscheinen und die Wochensumme stimmt.

**Acceptance Scenarios**:

1. **Given** Juna Sato hat am Montag, 2026-11-02, die Buchungen 08:07–12:53 auf `NSL-ALB-01`
   und 13:30–16:00 auf `KBA-TG3-04` und am Dienstag, 2026-11-03, die Buchung 07:00–11:00 auf
   `NSL-ALB-01`, **When** die Wochenübersicht für Juna Sato und die Woche vom 2026-11-02
   geöffnet wird, **Then** zeigt sie für Montag beide Buchungen mit den erfassten Uhrzeiten,
   den gerundeten Dauern 4:45 und 2:30 und der Tagessumme 7:15, für Dienstag eine Buchung mit
   4:00 und als Wochensumme 11:15 Stunden.
2. **Given** eine Woche, in der eine Person keine Buchung hat, **When** die Wochenübersicht
   geöffnet wird, **Then** erscheinen alle Tage ohne Buchungen und die Wochensumme ist 0:00.
3. **Given** die Wochenübersicht ist geöffnet, **When** der Schichtleiter sie ansieht,
   **Then** gibt es keine Möglichkeit, eine Buchung anzulegen, zu ändern oder zu löschen.

---

### User Story 2 - Summe je Projekt (Priority: P2)

In der Wochenübersicht sieht der Schichtleiter zusätzlich, wie viel Zeit die Person in dieser
Woche auf welches Projekt gebucht hat.

**Why this priority**: Die Summe je Projekt zeigt, wohin die Woche gegangen ist. Story 1 ist auch
ohne sie nutzbar.

**Independent Test**: Für eine Person Buchungen auf zwei Projekten in einer Woche anlegen und
prüfen, dass die Summe je Projekt angezeigt wird und zusammen die Wochensumme ergibt.

**Acceptance Scenarios**:

1. **Given** die Buchungen von Juna Sato aus User Story 1, **When** die Wochenübersicht geöffnet
   wird, **Then** zeigt sie `NSL-ALB-01` mit 8:45 und `KBA-TG3-04` mit 2:30 Stunden.

---

### User Story 3 - Zwischen Wochen blättern (Priority: P3)

Der Schichtleiter wechselt in der Übersicht zur vorigen oder nächsten Woche, ohne ein Datum
einzugeben.

**Why this priority**: Bequemer Zugriff auf andere Wochen; ohne Blättern bleibt die Übersicht für
eine gewählte Woche nutzbar.

**Independent Test**: Die Übersicht für eine Woche öffnen, zur nächsten Woche wechseln und prüfen,
dass deren Buchungen erscheinen; zurück zur vorigen wechseln und prüfen, dass wieder die erste
Woche zu sehen ist.

**Acceptance Scenarios**:

1. **Given** die Wochenübersicht für Juna Sato zeigt die Woche vom 2026-11-02, **When** zur
   nächsten Woche gewechselt wird, **Then** zeigt sie die Woche vom 2026-11-09 bis 2026-11-15.

---

### Edge Cases

- Was zeigt die Übersicht, wenn noch keine Person gewählt ist? → Keine Buchungen; sie fordert
  dazu auf, eine Person zu wählen.
- Was passiert mit Buchungen, die vor der Klärung der Rundung in Feature 001 mit gerundeten
  Uhrzeiten gespeichert wurden? → Sie erscheinen mit ihren gespeicherten Uhrzeiten; ihre
  gerundete Dauer ist gleich ihrer Dauer.
- Was zeigt die Übersicht für eine Woche in der Zukunft? → Die Tage, und Buchungen nur, wenn es
  welche gibt.
- Was passiert, wenn die Wochensumme mehr als 48 Stunden beträgt? → Sie wird angezeigt wie jede
  andere Summe; eine Warnung gibt es nicht (nicht Teil dieses Features).

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: Das System MUSS die Person für die Wochenübersicht aus der fest hinterlegten Liste
  der Mitarbeitenden auswählen lassen; eine Anmeldung gibt es nicht.
- **FR-002**: Das System MUSS für die gewählte Person und Woche alle sieben Tage der Woche
  anzeigen, auch Tage ohne Buchung, je Tag mit den Buchungen (erfasste Uhrzeiten, gerundete Dauer, Projekt, Notiz) und der
  Tagessumme.
- **FR-003**: Das System MUSS die Wochensumme als Summe der gerundeten Dauern aller Buchungen der
  Woche anzeigen.
- **FR-004**: Das System MUSS je Projekt die Summe der gerundeten Dauern der Woche anzeigen.
- **FR-005**: Das System MUSS das Wechseln zur vorigen und zur nächsten Woche ermöglichen; die
  gewählte Person bleibt dabei erhalten.
- **FR-006**: Die Wochenübersicht DARF keine Buchung anlegen, ändern oder löschen.
- **FR-007**: Gerundete Dauern, Tagessummen und Wochensumme MÜSSEN mit den Werten der Erfassung
  aus Feature 001 übereinstimmen (gleiche Rundungsregel).
- **FR-008**: Eine Woche MUSS von Montag bis Sonntag reichen.
- **FR-009**: Beim Öffnen MUSS die Übersicht die aktuelle Woche zeigen.

### Key Entities *(include if feature involves data)*

- **Woche**: Montag bis Sonntag; bestimmt, welche Buchungen gezeigt werden.
- **Buchung**: aus Feature 001; Datum, Beginn und Ende (wie erfasst), gerundete Dauer, Projekt,
  optionale Notiz. Wird hier nur gelesen.
- **Mitarbeitende, Projekt**: aus Feature 001, fest hinterlegt.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: Ein Schichtleiter sieht die Woche einer Person mit höchstens zwei Auswahlschritten
  (Person, Woche), ohne Tage einzeln aufzurufen.
- **SC-002**: Die angezeigte Wochensumme entspricht in 100 % der Fälle der Summe der Tagessummen
  derselben Woche, und jede Tagessumme der Tagessumme in der Erfassung.
- **SC-003**: Die Summen je Projekt ergeben zusammen in 100 % der Fälle die Wochensumme.

## Assumptions

- Die Übersicht ist von der Erfassungsseite aus erreichbar.
- Alle Werte kommen aus den bestehenden Buchungen; das Feature speichert nichts Neues.
- Die Rundung folgt Feature 001: gerundet wird die Dauer jeder Buchung kaufmännisch auf
  15 Minuten.
