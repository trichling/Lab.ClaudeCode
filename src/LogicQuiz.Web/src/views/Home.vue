<template>
  <div class="home-container">
    <div class="home-card">
      <h1>Logic Fallacy Quiz</h1>
      <p class="subtitle">Trainiere dein kritisches Denkvermögen</p>

      <div v-if="error" class="error-message">
        {{ error }}
      </div>

      <form @submit.prevent="startGame" class="game-form">
        <div class="form-group">
          <label for="playerName">Dein Name:</label>
          <input
            id="playerName"
            v-model="playerName"
            type="text"
            placeholder="Namen eingeben..."
            required
            maxlength="100"
            class="input-field"
          />
        </div>

        <div class="form-group">
          <label>Schwierigkeitsgrad:</label>
          <div class="difficulty-options">
            <label class="radio-option" :class="{ selected: difficulty === 1 }">
              <input
                v-model="difficulty"
                type="radio"
                name="difficulty"
                :value="1"
              />
              <div class="radio-content">
                <strong>Leicht</strong>
                <span>3 Antwortmöglichkeiten</span>
              </div>
            </label>

            <label class="radio-option" :class="{ selected: difficulty === 2 }">
              <input
                v-model="difficulty"
                type="radio"
                name="difficulty"
                :value="2"
              />
              <div class="radio-content">
                <strong>Mittel</strong>
                <span>5 Antwortmöglichkeiten</span>
              </div>
            </label>

            <label class="radio-option" :class="{ selected: difficulty === 3 }">
              <input
                v-model="difficulty"
                type="radio"
                name="difficulty"
                :value="3"
              />
              <div class="radio-content">
                <strong>Schwer</strong>
                <span>8 Antwortmöglichkeiten</span>
              </div>
            </label>
          </div>
        </div>

        <button type="submit" class="start-button" :disabled="loading">
          {{ loading ? 'Wird geladen...' : 'Spiel starten' }}
        </button>
      </form>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { api } from '../services/api'

const router = useRouter()
const playerName = ref('')
const difficulty = ref(1)
const loading = ref(false)
const error = ref('')

const startGame = async () => {
  if (!playerName.value.trim()) {
    error.value = 'Bitte gib deinen Namen ein'
    return
  }

  loading.value = true
  error.value = ''

  try {
    const gameState = await api.startGame(playerName.value, difficulty.value)

    // Store game state in localStorage
    localStorage.setItem('currentGame', JSON.stringify(gameState))

    // Navigate to game
    router.push('/game')
  } catch (err) {
    error.value = 'Fehler beim Starten des Spiels. Bitte versuche es erneut.'
    console.error(err)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.home-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.home-card {
  background: white;
  border-radius: 1rem;
  padding: 3rem;
  box-shadow: 0 20px 60px rgba(0, 0, 0, 0.3);
  max-width: 500px;
  width: 100%;
}

h1 {
  font-size: 2.5rem;
  color: #333;
  margin-bottom: 0.5rem;
  text-align: center;
}

.subtitle {
  text-align: center;
  color: #666;
  font-size: 1.1rem;
  margin-bottom: 2rem;
}

.error-message {
  background-color: #fee;
  color: #c33;
  padding: 1rem;
  border-radius: 0.5rem;
  margin-bottom: 1.5rem;
  text-align: center;
}

.game-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}

.form-group {
  display: flex;
  flex-direction: column;
  gap: 0.5rem;
}

.form-group label {
  font-weight: 600;
  color: #333;
  font-size: 1rem;
}

.input-field {
  padding: 0.75rem;
  border: 2px solid #ddd;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.3s;
}

.input-field:focus {
  outline: none;
  border-color: #667eea;
}

.difficulty-options {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.radio-option {
  display: flex;
  align-items: center;
  padding: 1rem;
  border: 2px solid #ddd;
  border-radius: 0.5rem;
  cursor: pointer;
  transition: all 0.3s;
}

.radio-option:hover {
  border-color: #667eea;
  background-color: #f8f9ff;
}

.radio-option.selected {
  border-color: #667eea;
  background-color: #f0f3ff;
}

.radio-option input[type='radio'] {
  margin-right: 1rem;
  width: 20px;
  height: 20px;
  cursor: pointer;
}

.radio-content {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.radio-content strong {
  color: #333;
  font-size: 1rem;
}

.radio-content span {
  color: #666;
  font-size: 0.875rem;
}

.start-button {
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 1.1rem;
  font-weight: 600;
  cursor: pointer;
  transition: transform 0.2s, box-shadow 0.2s;
  margin-top: 1rem;
}

.start-button:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
}

.start-button:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}
</style>
