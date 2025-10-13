<template>
  <div class="result-container">
    <div v-if="loading" class="loading">Lädt Ergebnisse...</div>

    <div v-else-if="result" class="result-card">
      <div class="result-header">
        <h1>Spiel beendet!</h1>
        <div class="trophy">🏆</div>
      </div>

      <div class="player-name">{{ result.playerName }}</div>

      <div class="stats-grid">
        <div class="stat-card">
          <div class="stat-label">Schwierigkeit</div>
          <div class="stat-value">{{ getDifficultyText(result.difficulty) }}</div>
        </div>

        <div class="stat-card">
          <div class="stat-label">Richtige Antworten</div>
          <div class="stat-value correct">
            {{ result.correctAnswers }} / {{ result.totalQuestions }}
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-label">Falsche Antworten</div>
          <div class="stat-value incorrect">
            {{ result.totalQuestions - result.correctAnswers }}
          </div>
        </div>

        <div class="stat-card">
          <div class="stat-label">Zeit</div>
          <div class="stat-value">{{ formatTime(result.timeInSeconds) }}</div>
        </div>
      </div>

      <div class="score-section">
        <div class="score-label">Gesamtpunktzahl</div>
        <div class="score-value">{{ result.score }}</div>
        <div class="score-description">
          Basierend auf Richtigkeit, Zeit und Schwierigkeit
        </div>
      </div>

      <div class="accuracy-bar">
        <div class="accuracy-label">
          Genauigkeit: {{ accuracyPercentage }}%
        </div>
        <div class="progress-bar">
          <div
            class="progress-fill"
            :style="{ width: accuracyPercentage + '%' }"
            :class="getAccuracyClass()"
          ></div>
        </div>
      </div>

      <div class="performance-message">
        <p>{{ getPerformanceMessage() }}</p>
      </div>

      <button @click="goHome" class="home-button">
        Zurück zur Startseite
      </button>
    </div>

    <div v-else class="error-message">
      Fehler beim Laden der Ergebnisse.
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../services/api'

const props = defineProps({
  sessionId: {
    type: String,
    required: true,
  },
})

const router = useRouter()
const result = ref(null)
const loading = ref(true)

const accuracyPercentage = computed(() => {
  if (!result.value) return 0
  return Math.round((result.value.correctAnswers / result.value.totalQuestions) * 100)
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

const formatTime = (seconds) => {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return mins > 0 ? `${mins}m ${secs}s` : `${secs}s`
}

const getAccuracyClass = () => {
  const accuracy = accuracyPercentage.value
  if (accuracy >= 80) return 'excellent'
  if (accuracy >= 60) return 'good'
  if (accuracy >= 40) return 'average'
  return 'poor'
}

const getPerformanceMessage = () => {
  const accuracy = accuracyPercentage.value

  if (accuracy >= 90) {
    return 'Hervorragend! Du hast ein ausgezeichnetes kritisches Denkvermögen!'
  } else if (accuracy >= 70) {
    return 'Sehr gut! Du erkennst die meisten logischen Fehlschlüsse.'
  } else if (accuracy >= 50) {
    return 'Gut gemacht! Mit etwas Übung wirst du noch besser.'
  } else {
    return 'Nicht aufgeben! Übung macht den Meister.'
  }
}

const goHome = () => {
  router.push('/')
}

onMounted(async () => {
  try {
    result.value = await api.getGameResult(props.sessionId)
  } catch (err) {
    console.error('Error loading result:', err)
  } finally {
    loading.value = false
  }
})
</script>

<style scoped>
.result-container {
  min-height: 100vh;
  padding: 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  display: flex;
  align-items: center;
  justify-content: center;
}

.loading,
.error-message {
  text-align: center;
  color: white;
  font-size: 1.5rem;
  padding: 2rem;
}

.result-card {
  max-width: 700px;
  width: 100%;
  background: white;
  border-radius: 1rem;
  padding: 3rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
}

.result-header {
  text-align: center;
  margin-bottom: 1.5rem;
}

.result-header h1 {
  margin: 0 0 1rem 0;
  color: #333;
  font-size: 2.5rem;
}

.trophy {
  font-size: 4rem;
  animation: bounce 1s ease infinite;
}

@keyframes bounce {
  0%,
  100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-10px);
  }
}

.player-name {
  text-align: center;
  font-size: 1.5rem;
  color: #666;
  margin-bottom: 2rem;
  font-weight: 600;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(140px, 1fr));
  gap: 1rem;
  margin-bottom: 2rem;
}

.stat-card {
  background: #f8f9ff;
  padding: 1.5rem;
  border-radius: 0.75rem;
  text-align: center;
  border: 2px solid #eee;
}

.stat-label {
  font-size: 0.875rem;
  color: #666;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: #333;
}

.stat-value.correct {
  color: #28a745;
}

.stat-value.incorrect {
  color: #dc3545;
}

.score-section {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
  border-radius: 1rem;
  text-align: center;
  margin-bottom: 2rem;
}

.score-label {
  color: rgba(255, 255, 255, 0.9);
  font-size: 1rem;
  margin-bottom: 0.5rem;
}

.score-value {
  color: white;
  font-size: 3.5rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
}

.score-description {
  color: rgba(255, 255, 255, 0.8);
  font-size: 0.875rem;
}

.accuracy-bar {
  margin-bottom: 2rem;
}

.accuracy-label {
  font-size: 1rem;
  color: #333;
  margin-bottom: 0.5rem;
  font-weight: 600;
}

.progress-bar {
  height: 30px;
  background: #eee;
  border-radius: 15px;
  overflow: hidden;
}

.progress-fill {
  height: 100%;
  transition: width 1s ease-out;
  border-radius: 15px;
  display: flex;
  align-items: center;
  justify-content: center;
  font-weight: 600;
  color: white;
}

.progress-fill.excellent {
  background: linear-gradient(90deg, #28a745 0%, #20c997 100%);
}

.progress-fill.good {
  background: linear-gradient(90deg, #17a2b8 0%, #138496 100%);
}

.progress-fill.average {
  background: linear-gradient(90deg, #ffc107 0%, #ff9800 100%);
}

.progress-fill.poor {
  background: linear-gradient(90deg, #dc3545 0%, #c82333 100%);
}

.performance-message {
  background: #f8f9ff;
  padding: 1.5rem;
  border-radius: 0.75rem;
  text-align: center;
  margin-bottom: 2rem;
  border-left: 4px solid #667eea;
}

.performance-message p {
  margin: 0;
  font-size: 1.1rem;
  color: #333;
  font-weight: 500;
}

.home-button {
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

.home-button:hover {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
}

@media (max-width: 768px) {
  .result-card {
    padding: 2rem;
  }

  .result-header h1 {
    font-size: 2rem;
  }

  .score-value {
    font-size: 2.5rem;
  }
}
</style>
