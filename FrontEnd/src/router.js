import { createRouter, createWebHistory } from 'vue-router';
import Home from '@/views/HomePage.vue';
import Login from '@/views/LoginPage.vue';
import Chat from '@/views/ChatPage.vue';
import Register from '@/views/RegisterPage.vue';
import Room from '@/views/RoomsPage.vue';
import { AuthService } from '@/services/authService';
import RoomAddPage from './views/RoomAddPage.vue';

const routes = [
  { path: '/', component: Home, meta: { requiresAuth: true } },
  { path: '/login', component: Login },
  { path: '/register', component: Register },
  { path: '/rooms', component: Room, meta: { requiresAuth: true } },
  { path: '/room/add', component: RoomAddPage, meta: { requiresAuth: true } },
  { path: '/room/:roomId', component: Chat, meta: { requiresAuth: true } },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// Navigation guard to check for authentication
router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
    if (!AuthService.isAuthenticated()) {
      next('/login'); // Redirect to login if not authenticated
    } else {
      next(); // Proceed to route if authenticated
    }
  } else {
    next(); // Always allow access to routes that do not require authentication
  }
});

export default router;