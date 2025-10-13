<template>
  <div class="game-container">
    <div v-if="loading" class="loading">Lädt...</div>

    <div v-else-if="gameState" class="game-card">
      <div class="game-header">
        <div class="player-info">
          <h2>{{ gameState.playerName }}</h2>
          <span class="difficulty-badge">
            {{ getDifficultyText(gameState.difficulty) }}
          </span>
        </div>
        <div class="progress-info">
          <span class="question-counter">
            Frage {{ currentQuestionIndex + 1 }} / {{ gameState.questions.length }}
          </span>
          <div class="time-display">
            Zeit: {{ elapsedTime }}s
          </div>
        </div>
      </div>

      <div v-if="!gameFinished" class="question-section">
        <div class="question-card">
          <p class="question-text">{{ currentQuestion.statement }}</p>
        </div>

        <div v-if="!answerSubmitted" class="answers-section">
          <h3>Welcher logische Fehlschluss liegt vor?</h3>
          <div class="answer-options">
            <button
              v-for="fallacy in gameState.availableFallacies"
              :key="fallacy.id"
              @click="selectAnswer(fallacy.id)"
              class="answer-button"
              :class="{ selected: selectedFallacyId === fallacy.id }"
            >
              <strong>{{ fallacy.name }}</strong>
              <span>{{ fallacy.description }}</span>
            </button>
          </div>
          <button
            @click="submitAnswer"
            class="submit-button"
            :disabled="!selectedFallacyId || submitting"
          >
            {{ submitting ? 'Wird gesendet...' : 'Antwort bestätigen' }}
          </button>
        </div>

        <div v-else class="answer-feedback">
          <div
            class="feedback-card"
            :class="{ correct: lastAnswerCorrect, incorrect: !lastAnswerCorrect }"
          >
            <h3>{{ lastAnswerCorrect ? '✓ Richtig!' : '✗ Falsch' }}</h3>
            <p v-if="!lastAnswerCorrect">
              Die richtige Antwort wäre: <strong>{{ correctFallacyName }}</strong>
            </p>
          </div>
          <button @click="nextQuestion" class="next-button">
            Nächste Frage
          </button>
        </div>
      </div>
    </div>

    <div v-else class="error-message">
      Kein Spiel gefunden. Bitte starte ein neues Spiel.
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../services/api'

const router = useRouter()
const gameState = ref(null)
const loading = ref(true)
const currentQuestionIndex = ref(0)
const selectedFallacyId = ref(null)
const answerSubmitted = ref(false)
const lastAnswerCorrect = ref(false)
const correctFallacyName = ref('')
const submitting = ref(false)
const gameFinished = ref(false)
const startTime = ref(null)
const elapsedTime = ref(0)
let timerInterval = null

const currentQuestion = computed(() => {
  if (!gameState.value || !gameState.value.questions) return null
  return gameState.value.questions[currentQuestionIndex.value]
})

const getDifficultyText = (difficulty) => {
  switch (difficulty) {
    case 1:
      return 'Leicht'
    case 2:
      return 'Mittel'
    case 3:
      return 'Schwer'
    default:
      return ''
  }
}

const selectAnswer = (fallacyId) => {
  if (!answerSubmitted.value) {
    selectedFallacyId.value = fallacyId
  }
}

const submitAnswer = async () => {
  if (!selectedFallacyId.value || submitting.value) return

  submitting.value = true

  try {
    const response = await api.submitAnswer(
      gameState.value.sessionId,
      currentQuestion.value.id,
      selectedFallacyId.value
    )

    lastAnswerCorrect.value = response.isCorrect
    correctFallacyName.value = response.correctFallacyName
    answerSubmitted.value = true
  } catch (err) {
    console.error('Error submitting answer:', err)
    alert('Fehler beim Senden der Antwort')
  } finally {
    submitting.value = false
  }
}

const nextQuestion = () => {
  if (currentQuestionIndex.value < gameState.value.questions.length - 1) {
    currentQuestionIndex.value++
    selectedFallacyId.value = null
    answerSubmitted.value = false
    lastAnswerCorrect.value = false
    correctFallacyName.value = ''
  } else {
    finishGame()
  }
}

const finishGame = () => {
  gameFinished.value = true
  localStorage.removeItem('currentGame')
  router.push(`/result/${gameState.value.sessionId}`)
}

const updateTimer = () => {
  if (startTime.value) {
    elapsedTime.value = Math.floor((Date.now() - startTime.value) / 1000)
  }
}

onMounted(() => {
  const savedGame = localStorage.getItem('currentGame')

  if (savedGame) {
    gameState.value = JSON.parse(savedGame)
    startTime.value = new Date(gameState.value.startTime).getTime()
    timerInterval = setInterval(updateTimer, 1000)
  } else {
    router.push('/')
  }

  loading.value = false
})

onUnmounted(() => {
  if (timerInterval) {
    clearInterval(timerInterval)
  }
})
</script>

<style scoped>
.game-container {
  min-height: 100vh;
  padding: 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.loading,
.error-message {
  text-align: center;
  color: white;
  font-size: 1.5rem;
  padding: 2rem;
}

.game-card {
  max-width: 900px;
  margin: 0 auto;
  background: white;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.game-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 2rem;
  padding-bottom: 1rem;
  border-bottom: 2px solid #eee;
  flex-wrap: wrap;
  gap: 1rem;
}

.player-info {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.player-info h2 {
  margin: 0;
  color: #333;
  font-size: 1.5rem;
}

.difficulty-badge {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  font-weight: 600;
}

.progress-info {
  display: flex;
  flex-direction: column;
  align-items: flex-end;
  gap: 0.5rem;
}

.question-counter {
  font-weight: 600;
  color: #333;
  font-size: 1.1rem;
}

.time-display {
  color: #666;
  font-size: 1rem;
}

.question-section {
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.question-card {
  background: #f8f9ff;
  padding: 2rem;
  border-radius: 0.75rem;
  border-left: 4px solid #667eea;
}

.question-text {
  font-size: 1.25rem;
  line-height: 1.6;
  color: #333;
  margin: 0;
}

.answers-section h3 {
  color: #333;
  margin-bottom: 1rem;
  font-size: 1.2rem;
}

.answer-options {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 1.5rem;
}

.answer-button {
  display: flex;
  flex-direction: column;
  align-items: flex-start;
  gap: 0.5rem;
  padding: 1.25rem;
  background: white;
  border: 2px solid #ddd;
  border-radius: 0.75rem;
  cursor: pointer;
  transition: all 0.3s;
  text-align: left;
}

.answer-button:hover {
  border-color: #667eea;
  background-color: #f8f9ff;
  transform: translateX(4px);
}

.answer-button.selected {
  border-color: #667eea;
  background-color: #f0f3ff;
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.2);
}

.answer-button strong {
  color: #333;
  font-size: 1.1rem;
}

.answer-button span {
  color: #666;
  font-size: 0.95rem;
}

.submit-button,
.next-button {
  width: 100%;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 0.75rem;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s;
}

.submit-button:hover:not(:disabled),
.next-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
}

.submit-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.answer-feedback {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.feedback-card {
  padding: 2rem;
  border-radius: 0.75rem;
  text-align: center;
}

.feedback-card.correct {
  background: #d4edda;
  border: 2px solid #28a745;
}

.feedback-card.incorrect {
  background: #f8d7da;
  border: 2px solid #dc3545;
}

.feedback-card h3 {
  margin: 0 0 1rem 0;
  font-size: 1.5rem;
}

.feedback-card.correct h3 {
  color: #28a745;
}

.feedback-card.incorrect h3 {
  color: #dc3545;
}

.feedback-card p {
  margin: 0;
  font-size: 1.1rem;
  color: #333;
}

@media (max-width: 768px) {
  .game-header {
    flex-direction: column;
    align-items: flex-start;
  }

  .progress-info {
    align-items: flex-start;
  }
}
</style>
