# 🏛 Architecture Documentation - Logic Fallacy Quiz

Diese Dokumentation beschreibt die Architektur und Design-Entscheidungen des Logic Fallacy Quiz Projekts.

## 📑 Inhaltsverzeichnis

1. [Architektur-Übersicht](#architektur-übersicht)
2. [Design-Prinzipien](#design-prinzipien)
3. [Komponenten-Details](#komponenten-details)
4. [Daten-Flow](#daten-flow)
5. [Sicherheits-Architektur](#sicherheits-architektur)
6. [Skalierbarkeit](#skalierbarkeit)
7. [Design-Entscheidungen](#design-entscheidungen)

---

## 🎯 Architektur-Übersicht

### System-Kontext

```
┌──────────────┐
│   Browser    │
│   (Client)   │
└──────┬───────┘
       │
       │ HTTPS/REST
       │
┌──────▼────────────────────────────────────────────┐
│         .NET Aspire Platform                      │
│  ┌──────────────────────────────────────────┐    │
│  │        LogicQuiz.Web (Vue 3 SPA)         │    │
│  │  - Vite Dev Server                       │    │
│  │  - Static Assets                         │    │
│  └──────────────┬───────────────────────────┘    │
│                 │                                 │
│  ┌──────────────▼───────────────────────────┐    │
│  │    LogicQuiz.Api (ASP.NET Core)          │    │
│  │  ┌────────────────────────────────────┐  │    │
│  │  │  Presentation Layer                │  │    │
│  │  │  - Controllers (API Endpoints)     │  │    │
│  │  │  - DTOs                            │  │    │
│  │  └────────────────────────────────────┘  │    │
│  │  ┌────────────────────────────────────┐  │    │
│  │  │  Business Layer                    │  │    │
│  │  │  - Services (Business Logic)       │  │    │
│  │  │  - Validation                      │  │    │
│  │  └────────────────────────────────────┘  │    │
│  │  ┌────────────────────────────────────┐  │    │
│  │  │  Data Access Layer                 │  │    │
│  │  │  - DbContext (EF Core)             │  │    │
│  │  │  - Repositories (implizit via EF)  │  │    │
│  │  └────────────────────────────────────┘  │    │
│  └──────────────┬───────────────────────────┘    │
│                 │                                 │
│  ┌──────────────▼───────────────────────────┐    │
│  │         PostgreSQL Database              │    │
│  │  - Tables                                │    │
│  │  - Relationships                         │    │
│  │  - Constraints                           │    │
│  └──────────────────────────────────────────┘    │
└───────────────────────────────────────────────────┘
```

### Architektur-Stil

**Layered Architecture (3-Schichten-Architektur)**

1. **Presentation Layer**: Controllers, DTOs
2. **Business Layer**: Services, Domain Logic
3. **Data Access Layer**: EF Core, Database

**Vorteile:**
- ✅ Klare Trennung der Verantwortlichkeiten
- ✅ Testbarkeit durch Abstraktion
- ✅ Wartbarkeit durch Modularität
- ✅ Skalierbarkeit durch lose Kopplung

---

## 🎨 Design-Prinzipien

### SOLID Principles

#### 1. **Single Responsibility Principle (SRP)**

Jede Klasse hat eine einzige Verantwortung:

```csharp
// ❌ Schlecht: Controller macht zu viel
public class QuizController
{
    public void StartGame()
    {
        // DB-Zugriff
        // Business-Logik
        // Validation
        // Response-Erstellung
    }
}

// ✅ Gut: Aufgeteilt in verschiedene Klassen
public class QuizController  // Nur HTTP-Handling
{
    private readonly IQuizService _service;  // Delegiert Business-Logik
}

public class QuizService  // Nur Business-Logik
{
    private readonly QuizDbContext _context;  // Delegiert DB-Zugriff
}
```

#### 2. **Open/Closed Principle (OCP)**

Offen für Erweiterung, geschlossen für Modifikation:

```csharp
// Interface für zukünftige Erweiterungen
public interface IQuizService
{
    Task<GameStateDto> StartGameAsync(string playerName, int difficulty);
    // Neue Methoden können hinzugefügt werden ohne bestehende zu ändern
}

// Verschiedene Implementierungen möglich
public class QuizService : IQuizService { }
public class CachedQuizService : IQuizService { }  // Zukünftig
```

#### 3. **Liskov Substitution Principle (LSP)**

Implementierungen sind austauschbar:

```csharp
// Jede IQuizService-Implementierung kann verwendet werden
IQuizService service = useCache 
    ? new CachedQuizService(context, cache)
    : new QuizService(context);
```

#### 4. **Interface Segregation Principle (ISP)**

Interfaces sind spezifisch und klein:

```csharp
// ✅ Kleine, fokussierte Interfaces
public interface IQuizService { }
public interface ILeaderboardService { }  // Separat

// ❌ Nicht: Ein großes Interface mit allem
// public interface IQuizAndLeaderboardService { }
```

#### 5. **Dependency Inversion Principle (DIP)**

Abhängig von Abstraktionen, nicht von Konkretem:

```csharp
// ✅ Abhängig von Interface
public class QuizController
{
    private readonly IQuizService _service;  // Interface, nicht konkrete Klasse
}

// DI Container löst auf
builder.Services.AddScoped<IQuizService, QuizService>();
```

### Clean Code Principles

- **Meaningful Names**: Sprechende Variablen- und Methodennamen
- **Small Functions**: Methoden tun eine Sache
- **DRY (Don't Repeat Yourself)**: Keine Duplikation
- **Comments When Necessary**: Code sollte selbsterklärend sein

---

## 🧩 Komponenten-Details

### Backend-Komponenten

#### 1. Controllers (Presentation Layer)

**Verantwortung:**
- HTTP-Requests entgegennehmen
- Input-Validierung
- Service-Aufrufe
- HTTP-Responses erstellen
- Exception-Handling

**Beispiel:**

```csharp
[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    // Dependency Injection
    private readonly IQuizService _quizService;
    private readonly ILogger<QuizController> _logger;

    // HTTP Endpoint
    [HttpPost("start")]
    public async Task<ActionResult<GameStateDto>> StartGame(
        [FromBody] StartGameRequest request)
    {
        // Validierung
        if (string.IsNullOrWhiteSpace(request.PlayerName))
            return BadRequest("Player name is required");

        try
        {
            // Service-Aufruf
            var gameState = await _quizService.StartGameAsync(
                request.PlayerName, 
                request.Difficulty);
            
            // Erfolgreiche Response
            return Ok(gameState);
        }
        catch (Exception ex)
        {
            // Fehler-Handling
            _logger.LogError(ex, "Error starting game");
            return StatusCode(500, "An error occurred");
        }
    }
}
```

#### 2. Services (Business Layer)

**Verantwortung:**
- Business-Logik implementieren
- Daten-Transformationen
- Orchestrierung von DB-Operationen
- Domain-Validierung

**Beispiel:**

```csharp
public class QuizService : IQuizService
{
    private readonly QuizDbContext _context;

    public async Task<GameStateDto> StartGameAsync(
        string playerName, 
        int difficulty)
    {
        // 1. Validierung
        if (difficulty < 1 || difficulty > 3)
            throw new ArgumentException("Invalid difficulty");

        // 2. Daten abrufen
        var questions = await GetRandomQuestionsAsync(difficulty);
        var fallacies = await GetFallacyTypesAsync(difficulty, questions);

        // 3. Game Session erstellen
        var session = new GameSession
        {
            PlayerName = playerName,
            Difficulty = difficulty,
            StartTime = DateTime.UtcNow,
            TotalQuestions = questions.Count
        };
        _context.GameSessions.Add(session);
        await _context.SaveChangesAsync();

        // 4. DTO zurückgeben
        return MapToDto(session, questions, fallacies);
    }
}
```

#### 3. Data Access (EF Core)

**Verantwortung:**
- Datenbank-Zugriff
- OR-Mapping
- Query-Optimierung
- Relationship-Management

**Beispiel:**

```csharp
public class QuizDbContext : DbContext
{
    public DbSet<FallacyType> FallacyTypes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<GameSession> GameSessions { get; set; }
    public DbSet<GameAnswer> GameAnswers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Relationships konfigurieren
        modelBuilder.Entity<Question>()
            .HasOne(q => q.CorrectFallacyType)
            .WithMany()
            .HasForeignKey(q => q.CorrectFallacyTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Constraints
        modelBuilder.Entity<GameSession>()
            .Property(s => s.PlayerName)
            .IsRequired()
            .HasMaxLength(100);
    }
}
```

### Frontend-Komponenten

#### 1. Views (Pages)

**Verantwortung:**
- Seiten-Layout
- User-Interaktion
- Navigation
- State-Management

**Beispiel:**

```vue
<script setup>
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import api from '../services/api'

const router = useRouter()
const gameState = ref(null)
const currentQuestionIndex = ref(0)

onMounted(async () => {
  const sessionId = router.currentRoute.value.params.sessionId
  // Lade Spielzustand...
})

async function submitAnswer(fallacyId) {
  // API-Call
  // Nächste Frage oder Ergebnis
}
</script>
```

#### 2. API Service

**Verantwortung:**
- HTTP-Kommunikation
- Error-Handling
- Request/Response-Transformation

**Beispiel:**

```javascript
import axios from 'axios'

const apiClient = axios.create({
  baseURL: `${import.meta.env.VITE_API_URL}/api`,
  headers: { 'Content-Type': 'application/json' }
})

// Interceptor für Error-Handling
apiClient.interceptors.response.use(
  response => response,
  error => {
    console.error('API Error:', error)
    return Promise.reject(error)
  }
)

export default {
  async startGame(playerName, difficulty) {
    const response = await apiClient.post('/quiz/start', {
      playerName,
      difficulty
    })
    return response.data
  }
}
```

---

## 🔄 Daten-Flow

### Request-Flow (Happy Path)

```
1. User klickt "Start Game"
   ↓
2. Vue Component (Home.vue)
   - Sammelt Input (Name, Difficulty)
   ↓
3. API Service (api.js)
   - POST /api/quiz/start
   ↓
4. Controller (QuizController.cs)
   - Validiert Input
   - Ruft Service auf
   ↓
5. Service (QuizService.cs)
   - Lädt Questions aus DB
   - Lädt FallacyTypes aus DB
   - Erstellt GameSession
   - Speichert in DB
   ↓
6. Controller
   - Konvertiert zu DTO
   - Gibt HTTP 200 + JSON zurück
   ↓
7. API Service
   - Parsed Response
   ↓
8. Vue Component
   - Navigiert zu Game-Seite
   - Zeigt Questions an
```

### Data Transformation

```
Domain Model (DB) → DTO → JSON → Frontend Object

GameSession (DB):
{
  Id: 1,
  PlayerName: "Max",
  Difficulty: 1,
  StartTime: DateTime,
  // ...mehr interne Felder
}

↓ MapToDto()

GameStateDto (API):
{
  SessionId: 1,
  PlayerName: "Max",
  Difficulty: 1,
  Questions: [...],
  AvailableFallacies: [...],
  StartTime: "2025-10-13T18:00:00Z"
}

↓ JSON Serialization

JSON Response:
{
  "sessionId": 1,
  "playerName": "Max",
  "difficulty": 1,
  ...
}

↓ Frontend Deserialization

Vue Reactive Object:
{
  sessionId: 1,
  playerName: "Max",
  difficulty: 1,
  ...
}
```

---

## 🔒 Sicherheits-Architektur

### Threat Model

```
┌─────────────────────────────────────────┐
│         Threat Actors                    │
├─────────────────────────────────────────┤
│ - Malicious Users                       │
│ - Script Kiddies                        │
│ - Automated Bots                        │
└─────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│      Attack Vectors (OWASP Top 10)      │
├─────────────────────────────────────────┤
│ 1. Broken Access Control                │
│ 2. Cryptographic Failures               │
│ 3. Injection                             │
│ 4. Insecure Design                      │
│ 5. Security Misconfiguration            │
│ 6. Vulnerable Components                │
│ 7. Authentication Failures              │
│ 8. Data Integrity Failures              │
│ 9. Logging/Monitoring Failures          │
│ 10. SSRF                                 │
└─────────────────────────────────────────┘
           ↓
┌─────────────────────────────────────────┐
│      Defense Mechanisms                  │
├─────────────────────────────────────────┤
│ ✅ Input Validation (Controllers)       │
│ ✅ Parameterized Queries (EF Core)      │
│ ✅ HTTPS Enforcement                     │
│ ✅ CORS Configuration                    │
│ ✅ Error Handling                        │
│ ✅ Structured Logging                    │
│ ⚠️  Rate Limiting (TODO)                │
│ ⚠️  Authentication (TODO)               │
│ ⚠️  Authorization (TODO)                │
└─────────────────────────────────────────┘
```

### Security Layers

```
┌──────────────────────────────────┐
│     Network Layer                │
│  - HTTPS/TLS                     │
│  - CORS                          │
└────────────┬─────────────────────┘
             ↓
┌────────────▼─────────────────────┐
│     Application Layer            │
│  - Input Validation              │
│  - Rate Limiting                 │
│  - Authentication                │
│  - Authorization                 │
└────────────┬─────────────────────┘
             ↓
┌────────────▼─────────────────────┐
│     Business Layer               │
│  - Business Logic Validation     │
│  - Domain Rules Enforcement      │
└────────────┬─────────────────────┘
             ↓
┌────────────▼─────────────────────┐
│     Data Layer                   │
│  - Parameterized Queries         │
│  - Data Encryption (at rest)     │
│  - Access Control                │
└──────────────────────────────────┘
```

---

## 📈 Skalierbarkeit

### Current Architecture (Single Instance)

```
┌──────────────┐
│   Browser    │
└──────┬───────┘
       ↓
┌──────▼───────┐    ┌──────────────┐
│   Web API    │───▶│  PostgreSQL  │
└──────────────┘    └──────────────┘
```

**Limits:**
- Vertical Scaling only
- Single Point of Failure
- Session-State im Memory

### Scalable Architecture (Future)

```
┌──────────────┐
│   Browser    │
└──────┬───────┘
       ↓
┌──────▼────────────────────┐
│   Load Balancer / CDN     │
└──────┬────────────────────┘
       ↓
┌──────▼───────┐  ┌─────────────┐  ┌─────────────┐
│   Web API 1  │  │ Web API 2   │  │ Web API N   │
└──────┬───────┘  └─────┬───────┘  └─────┬───────┘
       └──────────┬─────┘                │
                  ↓                       ↓
         ┌────────▼────────┐    ┌────────▼────────┐
         │   Redis Cache   │    │  PostgreSQL     │
         └─────────────────┘    │  (Primary +     │
                                │   Replicas)     │
                                └─────────────────┘
```

**Improvements:**
- ✅ Horizontal Scaling
- ✅ High Availability
- ✅ Distributed Caching
- ✅ Database Replication

### Scaling Strategies

#### 1. **Stateless API**

Aktuell ist die API stateless (gut für Skalierung):

```csharp
// ✅ Gut: Kein Server-seitiger State
public async Task<GameStateDto> StartGameAsync(...)
{
    // State wird in DB gespeichert, nicht im Memory
    var session = new GameSession { ... };
    _context.GameSessions.Add(session);
    await _context.SaveChangesAsync();
    
    return new GameStateDto(session.Id, ...);
}
```

#### 2. **Database Optimization**

```sql
-- Indexes für häufige Queries
CREATE INDEX idx_gamesession_playerName 
ON "GameSessions" ("PlayerName");

CREATE INDEX idx_gamesession_score 
ON "GameSessions" ("Score" DESC);

-- Partitioning für große Tabellen (zukünftig)
-- View Materialization für Analytics
```

#### 3. **Caching Strategy**

```csharp
// Zukünftig: Caching für statische Daten
public class CachedQuizService : IQuizService
{
    private readonly IMemoryCache _cache;
    private readonly QuizService _innerService;

    public async Task<List<FallacyType>> GetFallacyTypesAsync()
    {
        return await _cache.GetOrCreateAsync("fallacyTypes", 
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
                return await _innerService.GetFallacyTypesAsync();
            });
    }
}
```

---

## 🤔 Design-Entscheidungen

### Warum .NET Aspire?

**Pros:**
- ✅ Einfache lokale Entwicklung
- ✅ Service Discovery
- ✅ Integriertes Dashboard
- ✅ Container-Management
- ✅ Cloud-ready

**Cons:**
- ⚠️ Noch relativ neu
- ⚠️ Dokumentation teilweise unvollständig

### Warum Entity Framework Core?

**Pros:**
- ✅ Type-safe
- ✅ LINQ-Support
- ✅ Migrations
- ✅ Change Tracking

**Cons:**
- ⚠️ Performance-Overhead
- ⚠️ Learning Curve

**Alternative:** Dapper (für Performance-kritische Queries)

### Warum Vue 3 statt React?

**Pros:**
- ✅ Einfacher zu lernen
- ✅ Weniger Boilerplate
- ✅ Gute Dokumentation
- ✅ Single File Components

**Cons:**
- ⚠️ Kleineres Ökosystem als React
- ⚠️ Weniger Jobs

### Warum keine Authentication?

**Aktuell:** MVP/Demo-Projekt

**Zukünftig:** 
- JWT-basierte Auth
- Identity Framework
- OAuth 2.0 / OpenID Connect

### Warum DTOs?

**Separation of Concerns:**

```csharp
// Domain Model (intern)
public class GameSession
{
    public int Id { get; set; }
    public string PlayerName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int Score { get; set; }
    
    // Navigation Properties
    public List<GameAnswer> Answers { get; set; }
    
    // Internal Fields
    internal bool IsCompleted => EndTime.HasValue;
}

// DTO (extern)
public record GameStateDto(
    int SessionId,
    string PlayerName,
    int Difficulty,
    List<QuestionDto> Questions,
    List<FallacyTypeDto> AvailableFallacies,
    DateTime StartTime
);
```

**Vorteile:**
- ✅ API-Stabilität
- ✅ Security (keine internen Details)
- ✅ Versionierung möglich
- ✅ Klare Contracts

---

## 📊 Performance-Überlegungen

### Database Query Optimization

```csharp
// ❌ Schlecht: N+1 Problem
var sessions = await _context.GameSessions.ToListAsync();
foreach (var session in sessions)
{
    var answers = await _context.GameAnswers
        .Where(a => a.GameSessionId == session.Id)
        .ToListAsync();  // N zusätzliche Queries!
}

// ✅ Gut: Eager Loading
var sessions = await _context.GameSessions
    .Include(s => s.Answers)
    .ToListAsync();  // Nur 1 Query
```

### Frontend Performance

```javascript
// ✅ Lazy Loading von Routes
const routes = [
  {
    path: '/game/:sessionId',
    component: () => import('../views/Game.vue')  // Lazy loaded
  }
]

// ✅ Computed statt Methods für Performance
const filteredQuestions = computed(() => {
  return questions.value.filter(q => q.difficulty === selectedDifficulty.value)
})
```

---

## 🔮 Zukünftige Architekt-Änderungen

### Microservices (Optional)

```
┌────────────────┐
│  API Gateway   │
└────────┬───────┘
         │
    ┌────┴─────┬───────────┬──────────┐
    ↓          ↓           ↓          ↓
┌───▼───┐  ┌──▼──┐  ┌────▼─────┐  ┌─▼────┐
│ Quiz  │  │Auth │  │Leaderboard│  │Stats │
│Service│  │Svc  │  │  Service  │  │ Svc  │
└───────┘  └─────┘  └───────────┘  └──────┘
```

### Event-Driven Architecture

```
Game Started → Event Bus → Analytics Service
                        → Notification Service
                        → Leaderboard Service
```

### CQRS (Command Query Responsibility Segregation)

```
Write Model:                    Read Model:
┌──────────────┐               ┌──────────────┐
│  Commands    │               │   Queries    │
│  - StartGame │               │  - GetResult │
│  - Submit    │               │  - GetStats  │
└──────┬───────┘               └──────┬───────┘
       ↓                              ↓
┌──────▼───────┐               ┌─────▼────────┐
│  Write DB    │──Events──────▶│   Read DB    │
│ (normalized) │               │ (denormalized)│
└──────────────┘               └──────────────┘
```

---

## 📝 Zusammenfassung

Die Architektur des Logic Fallacy Quiz folgt bewährten Patterns und Prinzipien:

✅ **Layered Architecture** für Separation of Concerns
✅ **SOLID Principles** für wartbaren Code
✅ **Dependency Injection** für Testbarkeit
✅ **DTOs** für API-Stabilität
✅ **RESTful API** für Interoperabilität
✅ **Modern Stack** (.NET 9, Vue 3, PostgreSQL)

Die Architektur ist:
- **Wartbar**: Klare Strukturen und Verantwortlichkeiten
- **Testbar**: Durch Abstraktion und DI
- **Skalierbar**: Stateless Design, DB-optimiert
- **Sicher**: Defense in Depth, Input-Validierung
- **Erweiterbar**: Offene Interfaces, modulares Design

**Nächste Schritte:**
1. Authentication/Authorization hinzufügen
2. Caching implementieren
3. Rate Limiting einführen
4. Monitoring/Telemetry ausbauen
5. Performance-Optimierungen

---

Für Fragen: [Issue erstellen](https://github.com/trichling/Lab.ClaudeCode/issues)
