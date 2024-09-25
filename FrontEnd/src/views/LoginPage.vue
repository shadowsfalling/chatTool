<template>
  <v-container>
    <v-row justify="center">
      <v-col cols="12" md="4">
        <v-card>
          <v-card-title>Login</v-card-title>
          <v-card-text>
            <!-- Updated form to listen for keydown.enter -->
            <v-form>
              <v-text-field
                label="Username"
                v-model="username"
                @keydown.enter="login" 
              />
              <v-text-field
                label="Password"
                type="password"
                v-model="password"
                @keydown.enter="login" 
              />
              <v-alert v-if="errorMessage" type="error">{{ errorMessage }}</v-alert>
              <!-- Button to trigger login -->
              <v-btn color="primary" @click="login">Login</v-btn>
            </v-form>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import { ref } from 'vue'; 
import { AuthService } from '@/services/authService';
import router from '@/router';
import { useAuthStore } from '@/stores/authStore';

export default {
  setup() {
    const authStore = useAuthStore(); // Verwende Pinia oder Vuex, je nachdem was du verwendest
    const username = ref('');
    const password = ref('');
    const errorMessage = ref('');

    const login = async () => {
      try {
        await AuthService.login(username.value, password.value);
        await authStore.checkAuth(); // Überprüfe, ob der Benutzer eingeloggt ist
        router.push('/'); // Redirect to home page after successful login
      } catch (error) {
        errorMessage.value = 'Invalid credentials';
      }
    };

    return {
      username,
      password,
      errorMessage,
      login,
    };
  }
};
</script>