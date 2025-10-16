# 📡 API Documentation - Logic Fallacy Quiz

REST API Dokumentation für das Logic Fallacy Quiz Backend.

## 📑 Inhaltsverzeichnis

1. [Übersicht](#übersicht)
2. [Base URL](#base-url)
3. [Endpoints](#endpoints)
4. [Data Models](#data-models)
5. [Error Handling](#error-handling)
6. [Examples](#examples)

---

## 🎯 Übersicht

Die Logic Fallacy Quiz API ist eine RESTful API, die das Quiz-Spiel Backend bereitstellt.

### API Charakteristiken

- **Protocol**: HTTP/HTTPS
- **Format**: JSON
- **Authentication**: None (currently)
- **Rate Limiting**: None (currently)
- **Versioning**: None (v1 implicit)

### Base URL

```
Development: http://localhost:5000/api
Production:  https://api.yourdomain.com/api
```

---

## 🛣 Endpoints

### 1. Start Game

Startet eine neue Quizsession.

**Endpoint:** `POST /quiz/start`

**Request Body:**

```json
{
  "playerName": "string",
  "difficulty": 1 | 2 | 3
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| playerName | string | Yes | Name des Spielers (max. 100 Zeichen) |
| difficulty | integer | Yes | Schwierigkeitsgrad: 1 (Easy), 2 (Medium), 3 (Hard) |

**Response:** `200 OK`

```json
{
  "sessionId": 123,
  "playerName": "Max Mustermann",
  "difficulty": 1,
  "questions": [
    {
      "id": 5,
      "statement": "Du bist doch selbst nicht perfekt, also kannst du mich nicht kritisieren!",
      "correctFallacyTypeId": 7
    },
    {
      "id": 12,
      "statement": "Jeder glaubt das, also muss es wahr sein.",
      "correctFallacyTypeId": 4
    },
    {
      "id": 3,
      "statement": "Entweder du bist für uns oder gegen uns.",
      "correctFallacyTypeId": 3
    }
  ],
  "availableFallacies": [
    {
      "id": 1,
      "name": "Ad Hominem",
      "description": "Angriff auf die Person statt auf das Argument",
      "difficulty": 1
    },
    {
      "id": 3,
      "name": "Falsches Dilemma",
      "description": "Präsentation von nur zwei Optionen, obwohl mehr existieren",
      "difficulty": 1
    },
    {
      "id": 7,
      "name": "Tu Quoque",
      "description": "Ablenkung durch Hinweis auf Heuchelei des Gegners",
      "difficulty": 3
    }
  ],
  "startTime": "2025-10-16T14:30:00Z"
}
```

**Error Responses:**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Ungültige Eingabedaten (z.B. leerer PlayerName, ungültige Difficulty) |
| 500 Internal Server Error | Server-Fehler |

**Example Request:**

```bash
curl -X POST http://localhost:5000/api/quiz/start \
  -H "Content-Type: application/json" \
  -d '{
    "playerName": "Max Mustermann",
    "difficulty": 1
  }'
```

---

### 2. Submit Answer

Sendet eine Antwort für eine Frage.

**Endpoint:** `POST /quiz/answer`

**Request Body:**

```json
{
  "sessionId": 123,
  "questionId": 5,
  "selectedFallacyTypeId": 7
}
```

**Parameters:**

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| sessionId | integer | Yes | ID der Spielsession |
| questionId | integer | Yes | ID der Frage |
| selectedFallacyTypeId | integer | Yes | ID des ausgewählten Fehlschlusses |

**Response:** `200 OK`

```json
{
  "isCorrect": true,
  "correctFallacyName": "Tu Quoque"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| isCorrect | boolean | true wenn Antwort korrekt, false sonst |
| correctFallacyName | string | Name des korrekten Fehlschlusses |

**Error Responses:**

| Status Code | Description |
|-------------|-------------|
| 400 Bad Request | Ungültige Session-ID oder Question-ID |
| 404 Not Found | Session oder Question nicht gefunden |
| 500 Internal Server Error | Server-Fehler |

**Example Request:**

```bash
curl -X POST http://localhost:5000/api/quiz/answer \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": 123,
    "questionId": 5,
    "selectedFallacyTypeId": 7
  }'
```

---

### 3. Get Game Result

Holt das Ergebnis einer Spielsession.

**Endpoint:** `GET /quiz/result/{sessionId}`

**Path Parameters:**

| Parameter | Type | Required | Description |
|-----------|------|----------|-------------|
| sessionId | integer | Yes | ID der Spielsession |

**Response:** `200 OK`

```json
{
  "sessionId": 123,
  "playerName": "Max Mustermann",
  "difficulty": 1,
  "correctAnswers": 2,
  "totalQuestions": 3,
  "score": 950,
  "timeInSeconds": 45,
  "startTime": "2025-10-16T14:30:00Z",
  "endTime": "2025-10-16T14:30:45Z"
}
```

**Response Fields:**

| Field | Type | Description |
|-------|------|-------------|
| sessionId | integer | ID der Session |
| playerName | string | Name des Spielers |
| difficulty | integer | Gewählte Schwierigkeit |
| correctAnswers | integer | Anzahl richtiger Antworten |
| totalQuestions | integer | Gesamtzahl der Fragen |
| score | integer | Erreichte Punktzahl |
| timeInSeconds | integer | Benötigte Zeit in Sekunden |
| startTime | string (ISO 8601) | Startzeitpunkt |
| endTime | string (ISO 8601) | Endzeitpunkt |

**Score Berechnung:**

```
BasisPunkte = correctAnswers × 100
ZeitBonus = max(0, 500 - (timeInSeconds ÷ 2))
Multiplikator = difficulty switch {
  1 => 1.0,
  2 => 1.5,
  3 => 2.0
}
Score = (BasisPunkte + ZeitBonus) × Multiplikator
```

**Error Responses:**

| Status Code | Description |
|-------------|-------------|
| 404 Not Found | Session nicht gefunden |
| 500 Internal Server Error | Server-Fehler |

**Example Request:**

```bash
curl -X GET http://localhost:5000/api/quiz/result/123
```

---

## 📊 Data Models

### StartGameRequest

```typescript
{
  playerName: string,     // Max. 100 Zeichen
  difficulty: 1 | 2 | 3   // 1=Easy, 2=Medium, 3=Hard
}
```

### GameStateDto

```typescript
{
  sessionId: number,
  playerName: string,
  difficulty: number,
  questions: QuestionDto[],
  availableFallacies: FallacyTypeDto[],
  startTime: string  // ISO 8601 DateTime
}
```

### QuestionDto

```typescript
{
  id: number,
  statement: string,
  correctFallacyTypeId: number
}
```

### FallacyTypeDto

```typescript
{
  id: number,
  name: string,
  description: string,
  difficulty: number
}
```

### SubmitAnswerRequest

```typescript
{
  sessionId: number,
  questionId: number,
  selectedFallacyTypeId: number
}
```

### SubmitAnswerResponse

```typescript
{
  isCorrect: boolean,
  correctFallacyName: string
}
```

### GameResultDto

```typescript
{
  sessionId: number,
  playerName: string,
  difficulty: number,
  correctAnswers: number,
  totalQuestions: number,
  score: number,
  timeInSeconds: number,
  startTime: string,  // ISO 8601 DateTime
  endTime: string     // ISO 8601 DateTime
}
```

---

## ❌ Error Handling

### Error Response Format

```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "errors": {
    "PlayerName": ["The PlayerName field is required."]
  }
}
```

### Common HTTP Status Codes

| Code | Meaning | Description |
|------|---------|-------------|
| 200 | OK | Erfolgreiche Anfrage |
| 400 | Bad Request | Ungültige Eingabedaten |
| 404 | Not Found | Ressource nicht gefunden |
| 500 | Internal Server Error | Server-Fehler |

### Error Scenarios

#### 1. Invalid Player Name

**Request:**
```json
{
  "playerName": "",
  "difficulty": 1
}
```

**Response:** `400 Bad Request`
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Player name is required"
}
```

#### 2. Invalid Difficulty

**Request:**
```json
{
  "playerName": "Max",
  "difficulty": 5
}
```

**Response:** `400 Bad Request`
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Difficulty must be 1 (Easy), 2 (Medium), or 3 (Hard)"
}
```

#### 3. Session Not Found

**Request:**
```
GET /quiz/result/99999
```

**Response:** `404 Not Found`
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Game session not found"
}
```

---

## 📝 Examples

### Complete Game Flow

#### 1. Start a New Game

**Request:**
```bash
curl -X POST http://localhost:5000/api/quiz/start \
  -H "Content-Type: application/json" \
  -d '{
    "playerName": "Alice",
    "difficulty": 2
  }'
```

**Response:**
```json
{
  "sessionId": 456,
  "playerName": "Alice",
  "difficulty": 2,
  "questions": [
    {"id": 7, "statement": "...", "correctFallacyTypeId": 4},
    {"id": 11, "statement": "...", "correctFallacyTypeId": 2},
    {"id": 15, "statement": "...", "correctFallacyTypeId": 5},
    {"id": 19, "statement": "...", "correctFallacyTypeId": 6},
    {"id": 23, "statement": "...", "correctFallacyTypeId": 1}
  ],
  "availableFallacies": [
    {"id": 1, "name": "Ad Hominem", "description": "...", "difficulty": 1},
    {"id": 2, "name": "Strohmann", "description": "...", "difficulty": 1},
    {"id": 4, "name": "Slippery Slope", "description": "...", "difficulty": 2},
    {"id": 5, "name": "Hasty Generalization", "description": "...", "difficulty": 2},
    {"id": 6, "name": "Post Hoc", "description": "...", "difficulty": 2}
  ],
  "startTime": "2025-10-16T15:00:00Z"
}
```

#### 2. Submit Answers

**First Answer (Correct):**
```bash
curl -X POST http://localhost:5000/api/quiz/answer \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": 456,
    "questionId": 7,
    "selectedFallacyTypeId": 4
  }'
```

**Response:**
```json
{
  "isCorrect": true,
  "correctFallacyName": "Slippery Slope"
}
```

**Second Answer (Incorrect):**
```bash
curl -X POST http://localhost:5000/api/quiz/answer \
  -H "Content-Type: application/json" \
  -d '{
    "sessionId": 456,
    "questionId": 11,
    "selectedFallacyTypeId": 1
  }'
```

**Response:**
```json
{
  "isCorrect": false,
  "correctFallacyName": "Strohmann"
}
```

#### 3. Get Final Result

```bash
curl -X GET http://localhost:5000/api/quiz/result/456
```

**Response:**
```json
{
  "sessionId": 456,
  "playerName": "Alice",
  "difficulty": 2,
  "correctAnswers": 3,
  "totalQuestions": 5,
  "score": 825,
  "timeInSeconds": 90,
  "startTime": "2025-10-16T15:00:00Z",
  "endTime": "2025-10-16T15:01:30Z"
}
```

---

## 🔧 API Testing

### Using Postman

1. **Import Collection**
   - Erstelle eine neue Collection "Logic Fallacy Quiz"
   - Füge Requests für alle Endpoints hinzu

2. **Environment Variables**
   ```json
   {
     "baseUrl": "http://localhost:5000/api",
     "sessionId": ""
   }
   ```

3. **Test Scripts**
   ```javascript
   // Nach StartGame: Session ID speichern
   pm.test("Status is 200", function () {
       pm.response.to.have.status(200);
   });

   var jsonData = pm.response.json();
   pm.environment.set("sessionId", jsonData.sessionId);
   ```

### Using curl

**Complete Test Script:**

```bash
#!/bin/bash

# Start Game
RESPONSE=$(curl -s -X POST http://localhost:5000/api/quiz/start \
  -H "Content-Type: application/json" \
  -d '{"playerName": "TestUser", "difficulty": 1}')

echo "Start Game Response:"
echo $RESPONSE | jq .

# Extract Session ID
SESSION_ID=$(echo $RESPONSE | jq -r '.sessionId')
QUESTION_ID=$(echo $RESPONSE | jq -r '.questions[0].id')
FALLACY_ID=$(echo $RESPONSE | jq -r '.questions[0].correctFallacyTypeId')

echo "\nSession ID: $SESSION_ID"
echo "Question ID: $QUESTION_ID"
echo "Fallacy ID: $FALLACY_ID"

# Submit Answer
echo "\nSubmitting Answer..."
curl -s -X POST http://localhost:5000/api/quiz/answer \
  -H "Content-Type: application/json" \
  -d "{
    \"sessionId\": $SESSION_ID,
    \"questionId\": $QUESTION_ID,
    \"selectedFallacyTypeId\": $FALLACY_ID
  }" | jq .

# Get Result
echo "\nGetting Result..."
curl -s -X GET http://localhost:5000/api/quiz/result/$SESSION_ID | jq .
```

---

## 📚 Additional Information

### CORS Configuration

Aktuell erlaubt die API alle Origins (nur Development):

```
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, POST, OPTIONS
Access-Control-Allow-Headers: Content-Type
```

### Rate Limiting

**Current:** Keine Rate Limits

**Planned:** 
- 100 requests per minute per IP
- Burst: 20 requests

### Versioning

**Current:** Keine Versioning (implizit v1)

**Future:** URL-basiert
```
/api/v1/quiz/start
/api/v2/quiz/start
```

### OpenAPI/Swagger

Verfügbar unter (Development):
```
http://localhost:5000/openapi/v1.json
```

---

## 🔄 Changelog

### Version 1.0 (Current)

- Initial API release
- Basic CRUD operations for Quiz game
- Score calculation
- Session management

### Planned Features

- [ ] Authentication/Authorization
- [ ] Leaderboard endpoint
- [ ] Statistics endpoint
- [ ] Rate limiting
- [ ] API versioning
- [ ] Pagination for results
- [ ] Filtering and sorting

---

## 📞 Support

Bei Fragen oder Problemen:
- [GitHub Issues](https://github.com/trichling/Lab.ClaudeCode/issues)
- [Developer Guide](DEVELOPER_GUIDE.md)

---

**Last Updated:** October 16, 2025
