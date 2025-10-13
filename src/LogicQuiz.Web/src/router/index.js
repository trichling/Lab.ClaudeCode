import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue'
import Game from '../views/Game.vue'
import Result from '../views/Result.vue'

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: '/',
      name: 'home',
      component: Home
    },
    {
      path: '/game',
      name: 'game',
      component: Game
    },
    {
      path: '/result/:sessionId',
      name: 'result',
      component: Result,
      props: true
    }
  ]
})

export default router
