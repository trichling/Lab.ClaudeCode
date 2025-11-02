# 🧠 Logic Fallacy Quiz

Ein interaktives Quiz-Spiel zum Training des kritischen Denkvermögens. Spieler müssen logische Fehlschlüsse in verschiedenen Aussagen identifizieren und lernen dabei, argumentative Fehler zu erkennen.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)](https://dotnet.microsoft.com/)
[![Vue.js](https://img.shields.io/badge/Vue.js-3-4FC08D?logo=vue.js)](https://vuejs.org/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?logo=postgresql)](https://www.postgresql.org/)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

## 📋 Inhaltsverzeichnis

- [Überblick](#überblick)
- [Features](#features)
- [Technologie-Stack](#technologie-stack)
- [Schnellstart](#schnellstart)
- [Projektstruktur](#projektstruktur)
- [Dokumentation](#dokumentation)
- [Contributing](#contributing)
- [Lizenz](#lizenz)

## 🎯 Überblick

Logic Fallacy Quiz ist eine moderne Full-Stack-Webanwendung, die Nutzer dabei unterstützt, logische Fehlschlüsse zu erkennen und ihr kritisches Denkvermögen zu trainieren.

### Hauptkomponenten

```
┌─────────────────┐     ┌──────────────────┐     ┌─────────────────┐
│   Vue.js SPA    │────▶│  .NET 9 Web API  │────▶│   PostgreSQL    │
│  (Frontend)     │◀────│   (Backend)      │◀────│   (Database)    │
└─────────────────┘     └──────────────────┘     └─────────────────┘
         │                       │
         └───────────────────────┘
                  │
         ┌────────▼────────┐
         │  .NET Aspire    │
         │  (AppHost)      │
         └─────────────────┘
```

## ✨ Features

### 🎮 Spielmodi

- **Leicht** (Easy): 3 Fragen, 3 Antwortmöglichkeiten
- **Mittel** (Medium): 5 Fragen, 5 Antwortmöglichkeiten  
- **Schwer** (Hard): 10 Fragen, 8 Antwortmöglichkeiten

### 🧩 Spielablauf

1. **Spielstart**: Name eingeben und Schwierigkeitsgrad wählen
2. **Quiz**: Aussagen analysieren und korrekte Fehlschlüsse identifizieren
3. **Auswertung**: Detaillierte Ergebnisse mit Score, Zeit und Genauigkeit

### 🎯 Logische Fehlschlüsse

Das Spiel deckt verschiedene Kategorien von logischen Fehlschlüssen ab:

**Difficulty 1** (Basis)
- Ad Hominem
- Strohmann-Argument
- Falsche Dichotomie

**Difficulty 2** (Fortgeschritten)
- Argumentum ad Populum
- Post Hoc Ergo Propter Hoc
- Slippery Slope
- Hasty Generalization

**Difficulty 3** (Expert)
- Zirkelschluss
- Tu Quoque
- Argumentum ad Ignorantiam
- Appeal to Authority
- Red Herring

### 🔒 Sicherheit

- Entity Framework Core SQL-Injection-Schutz
- Input-Validierung auf Controller-Ebene
- Strukturiertes Logging für Monitoring
- CORS-Konfiguration (konfigurierbar für Production)

## 🛠 Technologie-Stack

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

## 🧪 Testing

Das Projekt enthält umfassende Unit Tests:

```bash
# Alle Tests ausführen
dotnet test

# Mit Detailausgabe
dotnet test --logger "console;verbosity=detailed"

# Tests mit Coverage
dotnet test --collect:"XPlat Code Coverage"
```

**Test-Abdeckung:**
- ✅ 14 Unit Tests
- ✅ Alle Schwierigkeitsgrade getestet
- ✅ Edge Cases abgedeckt
- ✅ Fehlerbehandlung validiert

## 📚 Dokumentation

Für detaillierte Entwicklerdokumentation siehe:
- [**DEVELOPER_GUIDE.md**](docs/DEVELOPER_GUIDE.md) - Umfassende Entwickler-Dokumentation
- [**API_DOCUMENTATION.md**](docs/API_DOCUMENTATION.md) - Detaillierte API-Referenz
- [**ARCHITECTURE.md**](docs/ARCHITECTURE.md) - Architektur-Übersicht

## 🤝 Contributing

Beiträge sind willkommen! Bitte folgen Sie diesen Schritten:

1. Fork das Repository
2. Erstellen Sie einen Feature-Branch (`git checkout -b feature/AmazingFeature`)
3. Committen Sie Ihre Änderungen (`git commit -m 'Add some AmazingFeature'`)
4. Pushen Sie den Branch (`git push origin feature/AmazingFeature`)
5. Öffnen Sie einen Pull Request

### Code-Standards

- Folgen Sie den C# Coding Conventions
- Verwenden Sie die `.editorconfig` Einstellungen
- Schreiben Sie Tests für neue Features
- Dokumentieren Sie öffentliche APIs

## 🐛 Bekannte Probleme & Roadmap

### Bekannte Probleme
- CORS-Konfiguration aktuell zu permissiv (siehe Issue #X)
- Keine Authentifizierung implementiert
- Rate Limiting fehlt

### Geplante Features
- [ ] Benutzer-Authentifizierung und Accounts
- [ ] Highscore-Tabelle
- [ ] Mehrsprachigkeit (i18n)
- [ ] Progressive Web App (PWA)
- [ ] Erweiterte Analytics
- [ ] Admin-Dashboard

## 📝 Lizenz

Dieses Projekt ist unter der MIT-Lizenz lizenziert - siehe [LICENSE](LICENSE) Datei für Details.

## 👥 Autoren

- **Tobias Richling** - *Initial work* - [trichling](https://github.com/trichling)

## 🙏 Danksagungen

- Inspiriert durch verschiedene Logik- und Fallacy-Training-Resources
- Dank an die .NET und Vue.js Communities
- Entity Framework Core und .NET Aspire Teams

## 📞 Support

Bei Fragen oder Problemen:
- Öffnen Sie ein [Issue](https://github.com/trichling/Lab.ClaudeCode/issues)
- Kontaktieren Sie den Maintainer

---

**Happy Coding! 🚀**
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
