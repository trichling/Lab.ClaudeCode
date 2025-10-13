const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000'

export const api = {
  async startGame(playerName, difficulty) {
    const response = await fetch(`${API_BASE_URL}/api/quiz/start`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ playerName, difficulty }),
    })

    if (!response.ok) {
      throw new Error('Failed to start game')
    }

    return response.json()
  },

  async submitAnswer(sessionId, questionId, selectedFallacyTypeId) {
    const response = await fetch(`${API_BASE_URL}/api/quiz/answer`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ sessionId, questionId, selectedFallacyTypeId }),
    })

    if (!response.ok) {
      throw new Error('Failed to submit answer')
    }

    return response.json()
  },

  async getGameResult(sessionId) {
    const response = await fetch(`${API_BASE_URL}/api/quiz/result/${sessionId}`)

    if (!response.ok) {
      throw new Error('Failed to get game result')
    }

    return response.json()
  },
}
