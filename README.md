# Logic Fallacy Quiz

Ein interaktives Quiz-Spiel zum Training des kritischen Denkvermögens. Spieler müssen logische Fehlschlüsse in verschiedenen Aussagen identifizieren.

## Projektübersicht

Das Projekt besteht aus drei Hauptkomponenten:

- **Backend (LogicQuiz.Api)**: C# WebAPI mit .NET 9
- **Frontend (LogicQuiz.Web)**: Vue 3 Single Page Application
- **Orchestrierung (LogicQuiz.AppHost)**: .NET Aspire zur Verwaltung aller Services
- **Datenbank**: PostgreSQL

## Funktionen

### Spielablauf

1. **Startseite**: Spieler geben ihren Namen ein und wählen einen Schwierigkeitsgrad:
   - **Leicht**: 3 Antwortmöglichkeiten
   - **Mittel**: 5 Antwortmöglichkeiten
   - **Schwer**: 8 Antwortmöglichkeiten

2. **Quiz**: Spieler erhalten 10 zufällige Fragen mit verschiedenen Aussagen, die logische Fehlschlüsse enthalten. Sie müssen den richtigen Fehlschluss aus den verfügbaren Optionen auswählen.

3. **Auswertung**: Nach Abschluss aller Fragen wird eine detaillierte Auswertung angezeigt:
   - Anzahl richtiger/falscher Antworten
   - Benötigte Zeit
   - Gesamtpunktzahl (basierend auf Richtigkeit, Zeit und Schwierigkeit)
   - Prozentualer Genauigkeitswert

### Logische Fehlschlüsse

Das Spiel enthält mindestens 3 Beispiele für jeden der folgenden Fehlschlüsse:

#### Schwierigkeit 1 (Leicht)
- **Ad Hominem**: Angriff auf die Person statt auf das Argument
- **Strohmann-Argument**: Verzerren der gegnerischen Position
- **Falsche Dichotomie**: Präsentation von nur zwei Optionen

#### Schwierigkeit 2 (Mittel)
- **Argumentum ad Populum**: Etwas ist wahr, weil viele es glauben
- **Post Hoc Ergo Propter Hoc**: Falsche Kausalität

#### Schwierigkeit 3 (Schwer)
- **Zirkelschluss**: Schlussfolgerung bereits in Prämissen enthalten
- **Tu Quoque**: Ablenkung durch Hinweis auf Heuchelei
- **Argumentum ad Ignorantiam**: Etwas ist wahr, weil es nicht widerlegt wurde

## Technologie-Stack

### Backend
- .NET 9
- ASP.NET Core WebAPI
- Entity Framework Core 9
- PostgreSQL mit Npgsql
- .NET Aspire für Service Orchestrierung

### Frontend
- Vue 3 (Composition API)
- Vite
- Vue Router
- Vanilla CSS

### Infrastruktur
- .NET Aspire AppHost
- PostgreSQL
- PgAdmin (über Aspire)

## Projektstruktur

```
Lab.ClaudeCode/
├── src/
│   ├── LogicQuiz.Api/              # Backend WebAPI
│   │   ├── Controllers/            # API Endpoints
│   │   ├── Data/                   # DbContext und Seeding
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Models/                 # Datenmodelle
│   │   ├── Services/               # Business Logik
│   │   └── Program.cs
│   ├── LogicQuiz.Web/              # Frontend Vue3 SPA
│   │   ├── src/
│   │   │   ├── router/             # Vue Router Konfiguration
│   │   │   ├── services/           # API Client
│   │   │   ├── views/              # Vue Komponenten
│   │   │   ├── App.vue
│   │   │   └── main.js
│   │   └── package.json
│   ├── LogicQuiz.AppHost/          # Aspire Orchestrierung
│   │   └── AppHost.cs
│   └── LogicQuiz.ServiceDefaults/  # Gemeinsame Aspire Konfiguration
└── LogicQuiz.sln
```

## Voraussetzungen

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (für PostgreSQL Container)
- [.NET Aspire Workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling)

### Installation der .NET Aspire Workload

```bash
dotnet workload install aspire
```

## Installation und Ausführung

### 1. Repository klonen

```bash
cd src
```

### 2. Frontend Dependencies installieren

```bash
cd LogicQuiz.Web
npm install
cd ..
```

### 3. Projekt mit Aspire starten

```bash
cd LogicQuiz.AppHost
dotnet run
```

Dies startet:
- PostgreSQL Container
- PgAdmin Container
- Backend API
- Frontend Dev Server
- Aspire Dashboard

### 4. Zugriff auf die Anwendung

Nach dem Start öffnet sich automatisch das Aspire Dashboard. Von dort aus können Sie auf die verschiedenen Services zugreifen:

- **Aspire Dashboard**: `http://localhost:15xxx` (Port wird im Terminal angezeigt)
- **Frontend**: `http://localhost:5173` (oder der im Dashboard angezeigte Port)
- **Backend API**: `http://localhost:5000` (oder der im Dashboard angezeigte Port)
- **PgAdmin**: Über das Aspire Dashboard erreichbar

## API Endpunkte

### POST /api/quiz/start
Startet ein neues Spiel

**Request Body:**
```json
{
  "playerName": "Max Mustermann",
  "difficulty": 1
}
```

**Response:**
```json
{
  "sessionId": 1,
  "playerName": "Max Mustermann",
  "difficulty": 1,
  "questions": [...],
  "availableFallacies": [...],
  "startTime": "2025-10-13T18:00:00Z"
}
```

### POST /api/quiz/answer
Sendet eine Antwort

**Request Body:**
```json
{
  "sessionId": 1,
  "questionId": 5,
  "selectedFallacyTypeId": 2
}
```

**Response:**
```json
{
  "isCorrect": true,
  "correctFallacyName": "Ad Hominem"
}
```

### GET /api/quiz/result/{sessionId}
Holt die Spielergebnisse

**Response:**
```json
{
  "sessionId": 1,
  "playerName": "Max Mustermann",
  "difficulty": 1,
  "correctAnswers": 8,
  "totalQuestions": 10,
  "score": 1200,
  "timeInSeconds": 120,
  "startTime": "2025-10-13T18:00:00Z",
  "endTime": "2025-10-13T18:02:00Z"
}
```

## Entwicklung

### Backend entwickeln

```bash
cd src/LogicQuiz.Api
dotnet watch run
```

### Frontend entwickeln

```bash
cd src/LogicQuiz.Web
npm run dev
```

### Neue Migration erstellen

```bash
cd src/LogicQuiz.Api
dotnet ef migrations add MigrationName
```

### Datenbank zurücksetzen

```bash
cd src/LogicQuiz.Api
dotnet ef database drop
dotnet ef database update
```

## Scoring-System

Die Punktzahl wird wie folgt berechnet:

1. **Basis-Punkte**: 100 Punkte pro richtiger Antwort
2. **Zeit-Bonus**: Maximaler Bonus von 500 Punkten (sinkt mit der Zeit)
3. **Schwierigkeits-Multiplikator**:
   - Leicht: 1.0x
   - Mittel: 1.5x
   - Schwer: 2.0x

**Formel**: `Score = (BasisPunkte + ZeitBonus) × Multiplikator`

## Datenbankschema

### FallacyTypes
- Id (int, PK)
- Name (string)
- Description (string)
- Difficulty (int)

### Questions
- Id (int, PK)
- Statement (string)
- CorrectFallacyTypeId (int, FK)

### GameSessions
- Id (int, PK)
- PlayerName (string)
- Difficulty (int)
- StartTime (DateTime)
- EndTime (DateTime, nullable)
- CorrectAnswers (int)
- TotalQuestions (int)
- Score (int)

### GameAnswers
- Id (int, PK)
- GameSessionId (int, FK)
- QuestionId (int, FK)
- SelectedFallacyTypeId (int, FK)
- IsCorrect (bool)
- AnsweredAt (DateTime)

## Aspire Features

Das Projekt nutzt .NET Aspire für:

- **Service Discovery**: Automatische Kommunikation zwischen Services
- **Health Checks**: Überwachung der Service-Gesundheit
- **Logging**: Zentralisierte Protokollierung
- **Metrics**: Performance-Monitoring
- **Database Orchestration**: Automatisches Setup von PostgreSQL

## Troubleshooting

### Docker läuft nicht
Stelle sicher, dass Docker Desktop gestartet ist.

### Port bereits in Verwendung
Aspire weist automatisch verfügbare Ports zu. Prüfe das Aspire Dashboard für aktuelle Ports.

### Datenbankverbindung schlägt fehl
Warte, bis der PostgreSQL Container vollständig gestartet ist (ca. 10-20 Sekunden).

### Frontend kann API nicht erreichen
Prüfe die `.env` Datei im Frontend-Projekt und stelle sicher, dass die richtige API-URL konfiguriert ist.

## Lizenz

Dieses Projekt wurde zu Bildungszwecken erstellt.
