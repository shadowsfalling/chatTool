import { defineStore } from 'pinia';
import { AuthService } from '@/services/authService';

export const useAuthStore = defineStore('auth', {
  state: () => ({
    user: null,
    loading: false,
    error: null,
  }),

  actions: {
    async checkAuth() {
      this.loading = true;
      try {
        const user = await AuthService.getMe();
        if (user) {
          this.user = user;
        } else {
          this.user = null;
        }
      } catch (error) {
        console.error("Fehler bei der Authentifizierung:", error);
        this.error = error;
        this.user = null;
      } finally {
        this.loading = false;
      }
    },

    logout() {
      AuthService.logout();
      this.user = null;
    },
  },

  getters: {
    isAuthenticated: (state) => !!state.user,
  },
});