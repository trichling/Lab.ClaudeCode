# 📘 Developer Guide - Logic Fallacy Quiz

Willkommen zum Logic Fallacy Quiz Projekt! Diese Dokumentation hilft neuen Entwicklern beim Einstieg in das Projekt.

## 📑 Inhaltsverzeichnis

1. [Projektübersicht](#projektübersicht)
2. [Architektur](#architektur)
3. [Entwicklungsumgebung einrichten](#entwicklungsumgebung-einrichten)
4. [Projektstruktur im Detail](#projektstruktur-im-detail)
5. [Backend-Entwicklung](#backend-entwicklung)
6. [Frontend-Entwicklung](#frontend-entwicklung)
7. [Datenbank](#datenbank)
8. [Testing](#testing)
9. [Deployment](#deployment)
10. [Best Practices](#best-practices)
11. [Troubleshooting](#troubleshooting)

---

## 🎯 Projektübersicht

Logic Fallacy Quiz ist eine Full-Stack-Webanwendung zum Training von kritischem Denken durch Erkennung logischer Fehlschlüsse.

### Technologie-Entscheidungen

| Technologie | Grund für die Wahl |
|------------|-------------------|
| .NET 9 | Moderne, performante Plattform mit hervorragendem Tooling |
| ASP.NET Core | Production-ready Web Framework mit Dependency Injection |
| Entity Framework Core | Type-safe ORM mit Migrations-Support |
| PostgreSQL | Robuste, open-source relationale Datenbank |
| Vue 3 | Reaktives, einfach zu lernendes Frontend-Framework |
| .NET Aspire | Moderne Orchestrierung für Cloud-native Apps |

### Kern-Features

- ✅ Drei Schwierigkeitsgrade (Easy, Medium, Hard)
- ✅ Randomisierte Fragen und Antworten
- ✅ Zeit- und Punktetracking
- ✅ Persistente Speicherung von Spielsessions
- ✅ RESTful API
- ✅ Responsive UI

---

## 🏗 Architektur

### High-Level Architektur

```
┌─────────────────────────────────────────────────────────────┐
│                      Client Browser                          │
│                     (Vue 3 SPA)                              │
└──────────────────────┬──────────────────────────────────────┘
                       │ HTTP/HTTPS
                       │ REST API
┌──────────────────────▼──────────────────────────────────────┐
│                  .NET Aspire AppHost                         │
│  ┌────────────────────────────────────────────────────┐     │
│  │         LogicQuiz.Api (.NET 9 Web API)             │     │
│  │  ┌──────────────┐  ┌──────────────┐               │     │
│  │  │ Controllers  │──│   Services   │               │     │
│  │  └──────┬───────┘  └──────┬───────┘               │     │
│  │         │                  │                        │     │
│  │  ┌──────▼──────────────────▼───────┐               │     │
│  │  │   Entity Framework Core         │               │     │
│  │  └──────┬──────────────────────────┘               │     │
│  └─────────┼──────────────────────────────────────────┘     │
│            │                                                 │
│  ┌─────────▼─────────────────────────────────────┐          │
│  │         PostgreSQL Container                  │          │
│  │  ┌─────────────────────────────────────┐      │          │
│  │  │ FallacyTypes │ Questions │ ...      │      │          │
│  │  └─────────────────────────────────────┘      │          │
│  └───────────────────────────────────────────────┘          │
└─────────────────────────────────────────────────────────────┘
```

### Komponenten-Übersicht

#### 1. **LogicQuiz.Api** (Backend)
- **Verantwortung**: Business-Logik, Datenzugriff, API-Endpoints
- **Pattern**: 
  - Controller → Service → Repository (via EF Core)
  - Dependency Injection
  - DTO-basierte API-Kommunikation

#### 2. **LogicQuiz.Web** (Frontend)
- **Verantwortung**: Benutzeroberfläche, User Experience
- **Pattern**:
  - Vue 3 Composition API
  - Vue Router für Navigation
  - Centralized API Service

#### 3. **LogicQuiz.AppHost** (.NET Aspire)
- **Verantwortung**: Service-Orchestrierung, Container-Management
- **Features**:
  - PostgreSQL Container
  - Service Discovery
  - Health Checks
  - Dashboard

#### 4. **LogicQuiz.ServiceDefaults**
- **Verantwortung**: Gemeinsame Konfiguration für alle Services
- **Inhalt**:
  - Service Discovery
  - Health Checks
  - OpenTelemetry

---

## 🛠 Entwicklungsumgebung einrichten

### Voraussetzungen

1. **.NET 9 SDK**
   ```bash
   # Prüfen der Installation
   dotnet --version
   # Sollte 9.0.x ausgeben
   ```

2. **.NET Aspire Workload**
   ```bash
   # Installation
   dotnet workload install aspire
   
   # Prüfen
   dotnet workload list
   ```

3. **Node.js 18+**
   ```bash
   # Prüfen
   node --version
   npm --version
   ```

4. **Docker Desktop**
   - Läuft und ist bereit (grünes Symbol in der Taskleiste)
   - Mindestens 4GB RAM zugewiesen

5. **IDE/Editor** (Empfohlen)
   - Visual Studio 2022 (17.9+) mit .NET Aspire Support
   - Visual Studio Code mit Extensions:
     - C# Dev Kit
     - Vue Language Features (Volar)
     - .NET Aspire

### Schritt-für-Schritt Setup

```bash
# 1. Repository klonen
git clone https://github.com/trichling/Lab.ClaudeCode.git
cd Lab.ClaudeCode

# 2. In src-Verzeichnis wechseln
cd src

# 3. Frontend Dependencies installieren
cd LogicQuiz.Web
npm install
cd ..

# 4. .NET Dependencies wiederherstellen
dotnet restore

# 5. Projekt starten (aus src-Verzeichnis)
cd LogicQuiz.AppHost
dotnet run
```

### Erste Schritte nach dem Start

1. **Aspire Dashboard öffnet sich automatisch** (z.B. `http://localhost:15000`)
2. **Im Dashboard sehen Sie**:
   - 🟢 logicquiz-api (Backend)
   - 🟢 logicquiz-web (Frontend)
   - 🟢 quizdb (PostgreSQL)
   - 🟢 quizdb-pgadmin (PgAdmin)

3. **Frontend aufrufen**: Klicken Sie auf den Link bei `logicquiz-web`
4. **API testen**: Verwenden Sie den OpenAPI-Link bei `logicquiz-api`

---

## 📂 Projektstruktur im Detail

### Backend (LogicQuiz.Api)

```
LogicQuiz.Api/
├── Controllers/              # API Endpoints
│   └── QuizController.cs    # Alle Quiz-bezogenen Endpoints
│
├── Data/                     # Datenzugriff
│   ├── QuizDbContext.cs     # EF Core DbContext
│   └── DbSeeder.cs          # Initiale Daten
│
├── DTOs/                     # Data Transfer Objects
│   ├── GameStateDto.cs      # Spielzustand für Client
│   ├── GameResultDto.cs     # Spielergebnis
│   ├── StartGameRequest.cs  # Request für Spielstart
│   ├── SubmitAnswerRequest.cs
│   ├── SubmitAnswerResponse.cs
│   ├── QuestionDto.cs
│   └── FallacyTypeDto.cs
│
├── Models/                   # Domain-Modelle
│   ├── FallacyType.cs       # Fehlschluss-Typ
│   ├── Question.cs          # Frage
│   ├── GameSession.cs       # Spielsession
│   └── GameAnswer.cs        # Antwort
│
├── Services/                 # Business-Logik
│   ├── IQuizService.cs      # Interface
│   └── QuizService.cs       # Implementierung
│
├── Migrations/               # EF Core Migrations
│   └── ...
│
├── Program.cs               # App-Startup
├── appsettings.json
└── LogicQuiz.Api.csproj
```

### Frontend (LogicQuiz.Web)

```
LogicQuiz.Web/
├── src/
│   ├── assets/              # Statische Ressourcen
│   │   └── vue.svg
│   │
│   ├── components/          # Vue Komponenten
│   │   └── HelloWorld.vue  # Demo-Komponente
│   │
│   ├── router/              # Routing
│   │   └── index.js        # Route-Definitionen
│   │
│   ├── services/            # API-Kommunikation
│   │   └── api.js          # Axios-basierter API-Client
│   │
│   ├── views/               # Seiten-Komponenten
│   │   ├── Home.vue        # Startseite
│   │   ├── Game.vue        # Quiz-Spiel
│   │   └── Result.vue      # Ergebnis-Seite
│   │
│   ├── App.vue              # Root-Komponente
│   ├── main.js              # App-Initialisierung
│   └── style.css            # Globale Styles
│
├── public/
│   └── vite.svg
│
├── index.html               # HTML Entry Point
├── package.json
├── vite.config.js           # Vite-Konfiguration
└── README.md
```

### Tests (LogicQuiz.Api.Tests)

```
LogicQuiz.Api.Tests/
├── Services/
│   └── QuizServiceTests.cs  # Unit Tests für QuizService
│
└── LogicQuiz.Api.Tests.csproj
```

---

## 🔧 Backend-Entwicklung

### Controller-Pattern

```csharp
[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;
    private readonly ILogger<QuizController> _logger;

    public QuizController(IQuizService quizService, ILogger<QuizController> logger)
    {
        _quizService = quizService;
        _logger = logger;
    }

    [HttpPost("start")]
    public async Task<ActionResult<GameStateDto>> StartGame([FromBody] StartGameRequest request)
    {
        // Validierung
        // Service-Aufruf
        // Fehlerbehandlung
    }
}
```

**Best Practices:**
- ✅ Dependency Injection verwenden
- ✅ DTOs statt Domain-Modelle zurückgeben
- ✅ Exceptions in ActionResults übersetzen
- ✅ Logging für wichtige Operationen

### Service-Layer

```csharp
public interface IQuizService
{
    Task<GameStateDto> StartGameAsync(string playerName, int difficulty);
    Task<SubmitAnswerResponse> SubmitAnswerAsync(int sessionId, int questionId, int selectedFallacyTypeId);
    Task<GameResultDto> GetGameResultAsync(int sessionId);
}

public class QuizService : IQuizService
{
    private readonly QuizDbContext _context;
    
    public QuizService(QuizDbContext context)
    {
        _context = context;
    }
    
    // Implementierung...
}
```

**Verantwortlichkeiten:**
- ✅ Business-Logik implementieren
- ✅ Datenbank-Operationen durchführen
- ✅ Domain-Modelle in DTOs konvertieren
- ✅ Validierung durchführen

### Entity Framework Core

#### DbContext

```csharp
public class QuizDbContext : DbContext
{
    public DbSet<FallacyType> FallacyTypes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<GameAnswer> GameAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Konfiguration von Relationships, Constraints, etc.
    }
}
```

#### Migration erstellen

```bash
cd src/LogicQuiz.Api

# Neue Migration erstellen
dotnet ef migrations add MigrationName

# Migration anwenden
dotnet ef database update

# Letzte Migration rückgängig machen
dotnet ef migrations remove
```

#### Daten seeden

Die `DbSeeder.cs` fügt initiale Daten hinzu:

```csharp
public static class DbSeeder
{
    public static async Task SeedAsync(QuizDbContext context)
    {
        if (!context.FallacyTypes.Any())
        {
            // Fallacy Types hinzufügen
        }
        
        if (!context.Questions.Any())
        {
            // Questions hinzufügen
        }
    }
}
```

### API-Endpunkte erweitern

**Beispiel: Neuen Endpoint hinzufügen**

1. **DTO erstellen** (z.B. `GetLeaderboardResponse.cs`)
   ```csharp
   public record GetLeaderboardResponse(List<LeaderboardEntry> Entries);
   public record LeaderboardEntry(string PlayerName, int Score, DateTime PlayedAt);
   ```

2. **Interface erweitern** (`IQuizService.cs`)
   ```csharp
   Task<GetLeaderboardResponse> GetLeaderboardAsync(int topCount = 10);
   ```

3. **Service implementieren** (`QuizService.cs`)
   ```csharp
   public async Task<GetLeaderboardResponse> GetLeaderboardAsync(int topCount = 10)
   {
       var topSessions = await _context.GameSessions
           .Where(s => s.EndTime != null)
           .OrderByDescending(s => s.Score)
           .Take(topCount)
           .Select(s => new LeaderboardEntry(s.PlayerName, s.Score, s.EndTime.Value))
           .ToListAsync();
           
       return new GetLeaderboardResponse(topSessions);
   }
   ```

4. **Controller-Endpoint hinzufügen** (`QuizController.cs`)
   ```csharp
   [HttpGet("leaderboard")]
   public async Task<ActionResult<GetLeaderboardResponse>> GetLeaderboard([FromQuery] int top = 10)
   {
       try
       {
           var leaderboard = await _quizService.GetLeaderboardAsync(top);
           return Ok(leaderboard);
       }
       catch (Exception ex)
       {
           _logger.LogError(ex, "Error getting leaderboard");
           return StatusCode(500, "An error occurred");
       }
   }
   ```

---

## 💻 Frontend-Entwicklung

### Vue 3 Composition API

**Beispiel-Komponente:**

```vue
<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const playerName = ref('')
const difficulty = ref(1)
const loading = ref(false)

async function startGame() {
  loading.value = true
  try {
    const response = await api.startGame(playerName.value, difficulty.value)
    // Weiter zu Game-Seite
  } catch (error) {
    console.error('Error starting game:', error)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="container">
    <h1>Start Game</h1>
    <input v-model="playerName" placeholder="Your name" />
    <select v-model="difficulty">
      <option :value="1">Easy</option>
      <option :value="2">Medium</option>
      <option :value="3">Hard</option>
    </select>
    <button @click="startGame" :disabled="loading">
      {{ loading ? 'Loading...' : 'Start' }}
    </button>
  </div>
</template>
```

### API Service

**services/api.js:**

```javascript
import axios from 'axios'

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

const apiClient = axios.create({
  baseURL: `${API_BASE_URL}/api`,
  headers: {
    'Content-Type': 'application/json'
  }
})

export default {
  startGame(playerName, difficulty) {
    return apiClient.post('/quiz/start', { playerName, difficulty })
  },
  
  submitAnswer(sessionId, questionId, selectedFallacyTypeId) {
    return apiClient.post('/quiz/answer', {
      sessionId,
      questionId,
      selectedFallacyTypeId
    })
  },
  
  getResult(sessionId) {
    return apiClient.get(`/quiz/result/${sessionId}`)
  }
}
```

### Routing

**router/index.js:**

```javascript
import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Game from '../views/Game.vue'
import Result from '../views/Result.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    { path: '/', name: 'Home', component: Home },
    { path: '/game/:sessionId', name: 'Game', component: Game },
    { path: '/result/:sessionId', name: 'Result', component: Result }
  ]
})

export default router
```

### State Management

Aktuell wird **kein globaler State Store** verwendet (Pinia/Vuex).
State wird über:
- Route-Parameter
- Lokale component refs
- API-Calls

Für größere Apps empfohlen: **Pinia**

---

## 💾 Datenbank

### Schema-Übersicht

```sql
-- FallacyTypes
CREATE TABLE "FallacyTypes" (
    "Id" SERIAL PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "Difficulty" INTEGER NOT NULL
);

-- Questions
CREATE TABLE "Questions" (
    "Id" SERIAL PRIMARY KEY,
    "Statement" TEXT NOT NULL,
    "CorrectFallacyTypeId" INTEGER NOT NULL,
    FOREIGN KEY ("CorrectFallacyTypeId") REFERENCES "FallacyTypes"("Id")
);

-- GameSessions
CREATE TABLE "GameSessions" (
    "Id" SERIAL PRIMARY KEY,
    "PlayerName" TEXT NOT NULL,
    "Difficulty" INTEGER NOT NULL,
    "StartTime" TIMESTAMP NOT NULL,
    "EndTime" TIMESTAMP,
    "CorrectAnswers" INTEGER NOT NULL DEFAULT 0,
    "TotalQuestions" INTEGER NOT NULL,
    "Score" INTEGER NOT NULL DEFAULT 0
);

-- GameAnswers
CREATE TABLE "GameAnswers" (
    "Id" SERIAL PRIMARY KEY,
    "GameSessionId" INTEGER NOT NULL,
    "QuestionId" INTEGER NOT NULL,
    "SelectedFallacyTypeId" INTEGER NOT NULL,
    "IsCorrect" BOOLEAN NOT NULL,
    "AnsweredAt" TIMESTAMP NOT NULL,
    FOREIGN KEY ("GameSessionId") REFERENCES "GameSessions"("Id"),
    FOREIGN KEY ("QuestionId") REFERENCES "Questions"("Id"),
    FOREIGN KEY ("SelectedFallacyTypeId") REFERENCES "FallacyTypes"("Id")
);
```

### Datenbank-Zugriff mit PgAdmin

1. Aspire Dashboard öffnen
2. Auf **quizdb-pgadmin** klicken
3. Login mit:
   - Email: admin@admin.com (Standard)
   - Password: (im Aspire Dashboard angezeigt)
4. Server hinzufügen:
   - Host: quizdb (oder localhost)
   - Port: 5432
   - Database: quizdb
   - Username: postgres
   - Password: (im Aspire Dashboard angezeigt)

### Manuelle Queries

```sql
-- Alle Fallacy Types anzeigen
SELECT * FROM "FallacyTypes" ORDER BY "Difficulty", "Id";

-- Top 10 Highscores
SELECT "PlayerName", "Score", "Difficulty", "EndTime"
FROM "GameSessions"
WHERE "EndTime" IS NOT NULL
ORDER BY "Score" DESC
LIMIT 10;

-- Fragen mit korrekten Antworten
SELECT q."Id", q."Statement", f."Name" as "CorrectFallacy"
FROM "Questions" q
JOIN "FallacyTypes" f ON q."CorrectFallacyTypeId" = f."Id";
```

---

## 🧪 Testing

### Unit Tests ausführen

```bash
# Alle Tests
cd src/LogicQuiz.Api.Tests
dotnet test

# Mit detaillierter Ausgabe
dotnet test --logger "console;verbosity=detailed"

# Mit Coverage
dotnet test --collect:"XPlat Code Coverage"
```

### Test-Struktur

**Naming Convention:**
```
MethodName_Scenario_ExpectedBehavior
```

**Beispiel:**
```csharp
[Fact]
public async Task StartGameAsync_Easy_ReturnsGameWithCorrectAnswersIncluded()
{
    // Arrange
    var context = await SeedTestData(Guid.NewGuid().ToString());
    var service = new QuizService(context);

    // Act
    var result = await service.StartGameAsync("TestPlayer", 1);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(3, result.Questions.Count);
    foreach (var question in result.Questions)
    {
        Assert.Contains(result.AvailableFallacies, 
            ft => ft.Id == question.CorrectFallacyTypeId);
    }
}
```

### In-Memory Database für Tests

```csharp
private QuizDbContext CreateInMemoryContext(string databaseName)
{
    var options = new DbContextOptionsBuilder<QuizDbContext>()
        .UseInMemoryDatabase(databaseName: databaseName)
        .Options;

    return new QuizDbContext(options);
}
```

**Vorteile:**
- ✅ Schnell
- ✅ Isoliert
- ✅ Keine echte DB nötig

### Neue Tests hinzufügen

1. Test-Klasse erstellen (falls nicht vorhanden)
2. Test-Methode mit `[Fact]` Attribut
3. Arrange-Act-Assert Pattern
4. Aussagekräftigen Namen wählen

---

## 🚀 Deployment

### Docker Container Build

```bash
# Backend
cd src/LogicQuiz.Api
dotnet publish -c Release

# Frontend
cd src/LogicQuiz.Web
npm run build
```

### Environment Variables

**Backend (appsettings.json / Environment):**
```json
{
  "ConnectionStrings": {
    "quizdb": "Host=localhost;Database=quizdb;Username=postgres;Password=..."
  },
  "AllowedOrigins": ["https://yourdomain.com"]
}
```

**Frontend (.env.production):**
```
VITE_API_URL=https://api.yourdomain.com
```

### Production Checklist

- [ ] CORS auf spezifische Origins einschränken
- [ ] HTTPS erzwingen
- [ ] Secrets in Umgebungsvariablen
- [ ] Logging konfigurieren
- [ ] Health Checks aktiviert
- [ ] Rate Limiting hinzufügen
- [ ] Error Handling überprüfen
- [ ] Input-Validierung verschärfen
- [ ] Datenbank-Backups einrichten
- [ ] Monitoring einrichten

---

## 📋 Best Practices

### Code-Stil

- **C#**: Microsoft C# Coding Conventions
- **JavaScript**: ESLint + Prettier (empfohlen)
- **Commits**: Conventional Commits
  - `feat: Add leaderboard endpoint`
  - `fix: Correct answer not always included`
  - `docs: Update API documentation`
  - `test: Add tests for score calculation`

### Git Workflow

```bash
# Feature-Branch erstellen
git checkout -b feature/leaderboard

# Änderungen committen
git add .
git commit -m "feat: add leaderboard functionality"

# Pushen
git push origin feature/leaderboard

# Pull Request erstellen
gh pr create
```

### Sicherheit

- ✅ **Input-Validierung**: Immer validieren
- ✅ **SQL-Injection**: EF Core nutzen
- ✅ **XSS**: Vue escapet automatisch
- ✅ **CORS**: Restriktiv konfigurieren
- ✅ **Secrets**: Nie im Code!
- ✅ **Logging**: Keine sensiblen Daten

### Performance

- ✅ **Async/Await**: Immer verwenden
- ✅ **DB-Queries**: N+1 vermeiden (`.Include()`)
- ✅ **Caching**: Für statische Daten
- ✅ **Pagination**: Für große Listen

---

## 🔍 Troubleshooting

### Häufige Probleme

#### 1. **Docker Container startet nicht**

```bash
# Docker Status prüfen
docker ps

# Logs ansehen
docker logs <container-id>

# Container neu starten
docker restart <container-id>
```

#### 2. **Migration schlägt fehl**

```bash
# Datenbank droppen und neu erstellen
dotnet ef database drop --force
dotnet ef database update
```

#### 3. **Frontend verbindet nicht zum Backend**

- Prüfe CORS-Konfiguration
- Prüfe API-URL in `services/api.js`
- Prüfe Aspire Dashboard für Service-URLs

#### 4. **Tests schlagen fehl**

```bash
# Clean und Rebuild
dotnet clean
dotnet build
dotnet test
```

#### 5. **"Aspire Workload not found"**

```bash
dotnet workload install aspire
```

### Debug-Tipps

#### Backend debuggen

**Visual Studio:**
- F5 drücken
- Breakpoints setzen

**VS Code:**
- `.vscode/launch.json` konfigurieren
- F5 drücken

#### Frontend debuggen

**Browser DevTools:**
- F12 öffnen
- Network-Tab für API-Calls
- Console für JavaScript-Errors

**Vue DevTools:**
- Browser Extension installieren
- Component-Inspektion

### Logs

**Backend:**
```bash
# Logs im Terminal ansehen
# Oder im Aspire Dashboard → Logs-Tab
```

**Frontend:**
```bash
# Vite Dev Server Logs
npm run dev

# Browser Console
F12 → Console
```

---

## 📚 Weiterführende Ressourcen

### Offizielle Dokumentation

- [.NET Documentation](https://docs.microsoft.com/dotnet/)
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core/)
- [Entity Framework Core](https://docs.microsoft.com/ef/core/)
- [.NET Aspire](https://learn.microsoft.com/dotnet/aspire/)
- [Vue 3](https://vuejs.org/)
- [Vite](https://vitejs.dev/)

### Tutorials

- [ASP.NET Core Web API Tutorial](https://docs.microsoft.com/aspnet/core/tutorials/first-web-api)
- [Vue 3 Composition API](https://vuejs.org/guide/introduction.html)
- [EF Core Migrations](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)

### Community

- [.NET Discord](https://discord.gg/dotnet)
- [Vue.js Discord](https://discord.com/invite/vue)
- [Stack Overflow](https://stackoverflow.com/questions/tagged/asp.net-core)

---

## 🎓 Nächste Schritte

1. **Code durchgehen**: Starte mit `Program.cs` und folge dem Flow
2. **Eigenen Branch erstellen**: Experimentiere mit dem Code
3. **Tests ansehen**: Lerne aus den vorhandenen Tests
4. **Feature hinzufügen**: Z.B. Leaderboard
5. **Pull Request erstellen**: Teile deinen Code

**Happy Coding! 🚀**

Für Fragen: [Issue erstellen](https://github.com/trichling/Lab.ClaudeCode/issues)
